using System;
using System.Net;
using System.Net.Sockets;


namespace GameNetwork
{
    /// <summary>
    /// Endpoint정보를 받아서 서버에 접속한다. (접속하려는 서버 하나당 인스턴스 한 개씩 생성)
    /// </summary>

    public class Connector
	{
		public delegate void ConnectedHandler(UserToken token);
		public ConnectedHandler connectedCallback { get; set; }

		// 서버와의 연결을 위한 소켓
		Socket client;
		NetworkService networkService;

		public Connector(NetworkService network_service)
		{
			this.networkService = network_service;
			this.connectedCallback = null;
		}

		public void Connect(IPEndPoint remote_endpoint)
		{
			this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			// 비동기 접속을 위한 event args
			SocketAsyncEventArgs event_arg = new SocketAsyncEventArgs();
			event_arg.Completed += OnConnectCompleted;
			event_arg.RemoteEndPoint = remote_endpoint;
			bool pending = this.client.ConnectAsync(event_arg);
			if (!pending)
			{
				OnConnectCompleted(null, event_arg);
			}
		}

		void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				UserToken token = new UserToken();

				// 데이터 수신 준비
				this.networkService.OnConnectCompleted(this.client, token);

				if (this.connectedCallback != null)
				{
					this.connectedCallback(token);
				}
			}
			else
			{
				// failed.
				Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
			}
		}
	}
}
