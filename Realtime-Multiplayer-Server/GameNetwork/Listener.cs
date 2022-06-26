using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace GameNetwork
{
    class Listener
	{
        // 비동기 Accept를 위한 EventArgs
		SocketAsyncEventArgs acceptArgs;

		Socket listenSocket;

        // Accept처리의 순서를 제어하기 위한 이벤트 변수
		AutoResetEvent flowControlEvent;

        // 새로운 클라이언트가 접속했을 때 호출되는 콜백
		public delegate void NewclientHandler(Socket clientSocket, object token);
		public NewclientHandler callbackOnNewclient;

        public Listener()
        {
			this.callbackOnNewclient = null;
        }

		public void Start(string host, int port, int backlog)
		{
			this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
				listenSocket.Bind(endpoint);
				listenSocket.Listen(backlog);

				this.acceptArgs = new SocketAsyncEventArgs();
				this.acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

				Thread listen_thread = new Thread(DoListen);
				listen_thread.Start();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

        /// <summary>
        /// 루프를 돌며 클라이언트를 받는다.
        /// 하나의 접속 처리가 완료된 후 다음 accept를 수행하기 위해서 event객체를 통해 흐름을 제어한다.
        /// </summary>
		void DoListen()
		{
            this.flowControlEvent = new AutoResetEvent(false);

			while (true)
			{
				// SocketAsyncEventArgs를 재사용 하기 위해서 null로 만들어 준다.
				this.acceptArgs.AcceptSocket = null;

				bool pending = true;
				try
				{
					// 비동기 accept를 호출하여 클라이언트의 접속을 받는다.
					// 비동기 매소드 이지만 동기적으로 수행이 완료될 경우도 있으니 리턴값을 확인하여 분기시켜야 한다.
					pending = listenSocket.AcceptAsync(this.acceptArgs);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}

				// 즉시 완료 되면 이벤트가 발생하지 않으므로 리턴값이 false일 경우 콜백 매소드를 직접 호출
				// pending상태라면 비동기 요청이 들어간 상태이므로 콜백 매소드를 기다린다
				if (!pending)
				{
					OnAcceptCompleted(null, this.acceptArgs);
				}

				// 클라이언트 접속 처리가 완료되면 이벤트 객체의 신호를 전달받아 다시 루프를 수행
				this.flowControlEvent.WaitOne();
			}
		}

        /// <summary>
        /// AcceptAsync의 콜백 매소드
        /// </summary>
		void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
		{
            if (e.SocketError == SocketError.Success)
            {
                // 새로 생긴 소켓을 보관해 놓은뒤~
                Socket client_socket = e.AcceptSocket;

                // 다음 연결을 받아들인다.
                this.flowControlEvent.Set();

                if (this.callbackOnNewclient != null)
                {
                    this.callbackOnNewclient(client_socket, e.UserToken);
                }

				return;
            }
            else
            {
				Console.WriteLine("Failed to accept client.");
            }

			// 다음 연결을 받음
            this.flowControlEvent.Set();
		}
	}
}
