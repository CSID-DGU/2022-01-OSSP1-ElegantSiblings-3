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
	class CListener
	{		
		SocketAsyncEventArgs accept_args;   // 비동기 Accept를 위한 EventArgs
		Socket listen_socket;  				// 클라이언트의 접속을 처리할 Socket
		AutoResetEvent flow_control_event;  // Accept처리의 순서를 제어하기 위한 이벤트 변수

		// 새로운 클라이언트가 접속했을 때 호출되는 콜백.
		public delegate void NewclientHandler(Socket client_socket, object token);
		public NewclientHandler callback_on_newclient;

		public CListener()
		{
			this.callback_on_newclient = null;
		}

		public void start(string host, int port, int backlog)
		{
			// Socket 생성
			this.listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			IPAddress address;
			if (host == "0.0.0.0")
			{
				address = IPAddress.Any;
			}
			else
			{
				address = IPAddress.Parse(host);
			}
			IPEndPoint endpoint = new IPEndPoint(address, port);

			try
			{
				// Socket에 host정보를 bind하고 Listen 메서드 호출
				listen_socket.Bind(endpoint);
				listen_socket.Listen(backlog);

				this.accept_args = new SocketAsyncEventArgs();
				this.accept_args.Completed += new EventHandler<SocketAsyncEventArgs>(on_accept_completed);

				// 하나의 접속 처리가 완료된 후 다음 accept를 수행하기 위해서 Thread룰 사용
				Thread listen_thread = new Thread(do_listen);
				listen_thread.Start();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		void do_listen()
		{
			this.flow_control_event = new AutoResetEvent(false);

			// 루프를 돌며 Client를 받음
			while (true)	
			{
				// Accept 처리 제어를 위한 객체, 재사용 하기 위해서 null로 초기화
				this.accept_args.AcceptSocket = null;

				bool pending = true;
				try
				{
					// 비동기 accept를 호출하여 CLient의 접속을 받음
					// 동기적으로 수행이 완료될 경우도 있으니 리턴값을 확인하여 분기시킴
					pending = listen_socket.AcceptAsync(this.accept_args);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}

				// 즉시 완료(리턴값이 false)일 경우 이벤트 발생X -> 콜백 매소드를 직접 호출
				// pending상태라면 비동기 요청 상태이므로 콜백 매소드를 기다리면 됩니다.
				if (!pending)
				{
					on_accept_completed(null, this.accept_args);
				}

				// Client 접속 처리가 완료되면 이벤트 객체의 신호를 전달받아 다시 루프를 수행
				this.flow_control_event.WaitOne();
			}
		}


		// 비동기 Accept의 콜백 매소드
		void on_accept_completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				// 새로 생긴 Socket을 보관
				Socket client_socket = e.AcceptSocket;

				// 다음 연결을 받아들인다.
				this.flow_control_event.Set();

				// accept까지의 역할만 수행하고 클라이언트의 접속 이후의 처리는 외부로 넘기기 위해서 콜백 매소드를 호출
				// Socket Accept부분은 변경이 적기 때문에 소켓 처리부와 컨텐츠 구현부를 분리
				if (this.callback_on_newclient != null)
				{
					this.callback_on_newclient(client_socket, e.UserToken);
				}

				return;
			}
			else
			{
				//todo:Accept 실패 처리.
				Console.WriteLine("Failed to accept client.");
			}

			// 다음 연결을 받음
			this.flow_control_event.Set();
		}
	}
}
