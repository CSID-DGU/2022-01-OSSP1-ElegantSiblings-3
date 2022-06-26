using System;
using System.Net.Sockets;
using System.Threading;


namespace GameNetwork
{
    public class NetworkService
    {
		int connectedCount;
		Listener clientListener;
		SocketAsyncEventArgsPool receiveEventArgsPool;
		SocketAsyncEventArgsPool sendEventArgsPool;
		BufferManager bufferManager;

		public delegate void SessionHandler(UserToken token);
		public SessionHandler sessionCreatedCallback { get; set; }

		// configs.
		int maxConnections;
		int bufferSize;
		readonly int preAllocCount = 2;		// read, write

		public NetworkService()
		{
			this.connectedCount = 0;
			this.sessionCreatedCallback = null;
		}


		public void Initialize()
		{
			this.maxConnections = 10000;
			this.bufferSize = 1024;
			this.bufferManager = new BufferManager(this.maxConnections * this.bufferSize * this.preAllocCount, this.bufferSize);
			this.receiveEventArgsPool = new SocketAsyncEventArgsPool(this.maxConnections);
			this.sendEventArgsPool = new SocketAsyncEventArgsPool(this.maxConnections);
			this.bufferManager.InitBuffer();

			SocketAsyncEventArgs arg;

			for (int i = 0; i < this.maxConnections; i++)
			{
				UserToken token = new UserToken();

				// receive pool
				{
					//Pre-allocate a set of reusable SocketAsyncEventArgs
					arg = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveCompleted);
					arg.UserToken = token;

					// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
					this.bufferManager.SetBuffer(arg);

					// add SocketAsyncEventArg to the pool
					this.receiveEventArgsPool.Push(arg);
				}

				// send pool
				{
					//Pre-allocate a set of reusable SocketAsyncEventArgs
					arg = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(SendCompleted);
					arg.UserToken = token;

					// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
					this.bufferManager.SetBuffer(arg);

					// add SocketAsyncEventArg to the pool
					this.sendEventArgsPool.Push(arg);
				}
			}
		}

		public void Listen(string host, int port, int backlog)
		{
			this.clientListener = new Listener();
			this.clientListener.callbackOnNewclient += OnNewClient;
			this.clientListener.Start(host, port, backlog);
		}

		/// <summary>
		/// 원격 서버에 접속 성공 했을 때 호출된다
		/// </summary>
		public void OnConnectCompleted(Socket socket, UserToken token)
		{
			SocketAsyncEventArgs receiveEventArg = new SocketAsyncEventArgs();
			receiveEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveCompleted);
			receiveEventArg.UserToken = token;
			receiveEventArg.SetBuffer(new byte[1024], 0, 1024);

			SocketAsyncEventArgs sendEventArg = new SocketAsyncEventArgs();
			sendEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(SendCompleted);
			sendEventArg.UserToken = token;
			sendEventArg.SetBuffer(new byte[1024], 0, 1024);

			BeginReceive(socket, receiveEventArg, sendEventArg);
		}

        /// <summary>
        /// 새로운 클라이언트가 접속 성공 했을 때 호출된다.
        /// </summary>
		void OnNewClient(Socket clientSocket, object token)
		{
			Interlocked.Increment(ref this.connectedCount);

			Console.WriteLine(string.Format("[{0}] A client connected. handle {1},  count {2}",
				Thread.CurrentThread.ManagedThreadId, clientSocket.Handle,
				this.connectedCount));

			SocketAsyncEventArgs receiveArgs = this.receiveEventArgsPool.Pop();
			SocketAsyncEventArgs sendArgs = this.sendEventArgsPool.Pop();

			UserToken userToken = null;
			if (this.sessionCreatedCallback != null)
			{
				userToken = receiveArgs.UserToken as UserToken;
				this.sessionCreatedCallback(userToken);
			}

			BeginReceive(clientSocket, receiveArgs, sendArgs);
		}

		void BeginReceive(Socket socket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
		{
			UserToken token = receiveArgs.UserToken as UserToken;
			token.SetEventArgs(receiveArgs, sendArgs);
			token.socket = socket;

			bool pending = socket.ReceiveAsync(receiveArgs);
			if (!pending)
			{
				ProcessReceive(receiveArgs);
			}
		}


		void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.LastOperation == SocketAsyncOperation.Receive)
			{
				ProcessReceive(e);
				return;
			}

			throw new ArgumentException("The last operation completed on the socket was not a receive.");
		}


		void SendCompleted(object sender, SocketAsyncEventArgs e)
		{
			UserToken token = e.UserToken as UserToken;
			token.ProcessSend(e);
		}


		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			UserToken token = e.UserToken as UserToken;
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				token.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);

				bool pending = token.socket.ReceiveAsync(e);
				if (!pending)
				{
					ProcessReceive(e);
				}
			}
			else
			{
				Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
				CloseClientsocket(token);
			}
		}

		public void CloseClientsocket(UserToken token)
		{
			token.OnRemoved();

			if (this.receiveEventArgsPool != null)
			{
				this.receiveEventArgsPool.Push(token.receiveEventArgs);
			}

			if (this.sendEventArgsPool != null)
			{
				this.sendEventArgsPool.Push(token.sendEventArgs);
			}
		}
    }
}
