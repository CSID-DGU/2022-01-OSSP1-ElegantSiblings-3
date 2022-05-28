using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerNetworkModule
{
	public class CNetworkService
	{
		int connected_count;
		CListener client_listener;
		SocketAsyncEventArgsPool receive_event_args_pool;
		SocketAsyncEventArgsPool send_event_args_pool;
		BufferManager buffer_manager;

		public delegate void SessionHandler(CUserToken token);
		public SessionHandler session_created_callback { get; set; }


		int max_connections;
		int buffer_size;
		readonly int pre_alloc_count = 2;       // read, write

		public CNetworkService()
		{
			this.connected_count = 0;
			this.session_created_callback = null;
		}

		public void initialize()
		{
			// 버퍼 전체의 크기 = 최대 동시 접속 수 x 버퍼 하나의 크기 x (send + receive)
			this.max_connections = 10000;
			this.buffer_size = 1024;
			this.buffer_manager = new BufferManager(this.max_connections * this.buffer_size * this.pre_alloc_count, this.buffer_size);
			this.buffer_manager.InitBuffer();


			// SocketAsyncEventArgs 객체를 미리 생성하여 pool에 저장
			SocketAsyncEventArgs arg;
			this.receive_event_args_pool = new SocketAsyncEventArgsPool(this.max_connections);
			this.send_event_args_pool = new SocketAsyncEventArgsPool(this.max_connections);
		
			// 최대 동시 접속 수 만큼 객체 생성
			for (int i = 0; i < this.max_connections; i++)
			{
				// 동일한 Socket에서 send, receive를 하므로
				// user token은 세션별로 하나씩 만들고 receive, send EventArgs에서 동일한 token을 참조한다
				CUserToken token = new CUserToken();

				// receive pool
				{
					arg = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(receive_completed);
					arg.UserToken = token;
					this.buffer_manager.SetBuffer(arg);
					this.receive_event_args_pool.Push(arg);
				}

				// send pool
				{
					arg = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(send_completed);
					arg.UserToken = token;
					this.buffer_manager.SetBuffer(arg);
					this.send_event_args_pool.Push(arg);
				}
			}
		}

		// Listener 생성
		public void listen(string host, int port, int backlog)
		{
			this.client_listener = new CListener();
			this.client_listener.callback_on_newclient += on_new_client;
			this.client_listener.start(host, port, backlog);  // Client의 접속을 기다림
		}

		/// 원격 서버에 접속 성공 했을 때 호출됩니다.
		public void on_connect_completed(Socket socket, CUserToken token)
		{
			// SocketAsyncEventArgsPool에서 빼오지 않고 그때 그때 할당해서 사용한다.
			// 풀은 서버에서 클라이언트와의 통신용으로만 쓰려고 만든것이기 때문이다.
			// 클라이언트 입장에서 서버와 통신을 할 때는 접속한 서버당 두개의 EventArgs만 있으면 되기 때문에 그냥 new해서 쓴다.
			// 서버간 연결에서도 마찬가지이다.
			// 풀링처리를 하려면 c->s로 가는 별도의 풀을 만들어서 써야 한다.
			SocketAsyncEventArgs receive_event_arg = new SocketAsyncEventArgs();
			receive_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(receive_completed);
			receive_event_arg.UserToken = token;
			receive_event_arg.SetBuffer(new byte[1024], 0, 1024);

			SocketAsyncEventArgs send_event_arg = new SocketAsyncEventArgs();
			send_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(send_completed);
			send_event_arg.UserToken = token;
			send_event_arg.SetBuffer(new byte[1024], 0, 1024);

			begin_receive(socket, receive_event_arg, send_event_arg);
		}




		// 새로운 클라이언트가 접속 성공 했을 때 호출
		void on_new_client(Socket client_socket, object token)
		{
			//todo: peer list처리

			Interlocked.Increment(ref this.connected_count);

			Console.WriteLine(string.Format("[{0}] A client connected. handle {1},  count {2}", Thread.CurrentThread.ManagedThreadId, client_socket.Handle, this.connected_count));

			// receive_args, send_args pool에서 하나 꺼냄
			SocketAsyncEventArgs receive_args = this.receive_event_args_pool.Pop();
			SocketAsyncEventArgs send_args = this.send_event_args_pool.Pop();
			CUserToken user_token = null;

			// SocketAsyncEventArgs를 생성할 때 만든 CUserToken을 콜백 메서드로 넘김
			if (this.session_created_callback != null)
			{
				user_token = receive_args.UserToken as CUserToken;
				this.session_created_callback(user_token);
			}

			// Client로부터 데이터를 수신할 준비를 한다
			begin_receive(client_socket, receive_args, send_args);
			//user_token.start_keepalive();
		}


		// Client로부터 데이터를 수신할 때 호출
		void begin_receive(Socket socket, SocketAsyncEventArgs receive_args, SocketAsyncEventArgs send_args)
		{
			// receive_args, send_args 아무곳에서 꺼내도 된다 (동일한 CUserToken을 갖고 있음)
			CUserToken token = receive_args.UserToken as CUserToken;

			// 생성된 Client Socket을 보관하고 통신할 때 사용
			token.set_event_args(receive_args, send_args);
			token.socket = socket;

			bool pending = socket.ReceiveAsync(receive_args);
			if (!pending)
			{
				process_receive(receive_args);
			}
		}


		// receiveAsync 콜백 메서드
		void receive_completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.LastOperation == SocketAsyncOperation.Receive)
			{
				process_receive(e);
				return;
			}

			throw new ArgumentException("The last operation completed on the socket was not a receive.");
		}

		// sendAsnc 콜백 메서드
		void send_completed(object sender, SocketAsyncEventArgs e)
		{
			CUserToken token = e.UserToken as CUserToken;
			token.process_send(e);
		}


		// 비동기 수신이 완료되었을 때 호출
		private void process_receive(SocketAsyncEventArgs e)
		{
			CUserToken token = e.UserToken as CUserToken;

			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				token.on_receive(e.Buffer, e.Offset, e.BytesTransferred);  // 수신된 byte배열, 데이터의 시작 위치, 수신된 바이트 수

				bool pending = token.socket.ReceiveAsync(e);
				if (!pending)
				{
					// pending 상태가 아니면, 계속 데이터를 받을 수 있도록 함
					process_receive(e);
				}
			}
			else  // 호스트 연결이 끊기면 소켓이 닫힌다
			{
				Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
				close_clientsocket(token);
			}
		}

		// Socket을 닫을 때 호출
		public void close_clientsocket(CUserToken token)
		{
			token.on_removed();

			// 버퍼는 반환할 필요가 없다. SocketAsyncEventArg가 버퍼를 가지고 있기 떄문에, 재사용 시 저장된 버퍼를 그대로 사용하면 되기 때문이다.
			if (this.receive_event_args_pool != null)
			{
				this.receive_event_args_pool.Push(token.receive_event_args);
			}

			if (this.send_event_args_pool != null)
			{
				this.send_event_args_pool.Push(token.send_event_args);
			}
		}
	}
}
