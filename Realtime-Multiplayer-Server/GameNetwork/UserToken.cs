using System;
using System.Collections.Generic;
using System.Net.Sockets;


namespace GameNetwork
{
    public class UserToken
	{
		public Socket socket { get; set; }

		public SocketAsyncEventArgs receiveEventArgs { get; private set; }
		public SocketAsyncEventArgs sendEventArgs { get; private set; }

		// 바이트를 패킷 형식으로 해석해주는 해석기
		MessageResolver messageResolver;

		// session객체. 앱(Client)에서 구현해야 함
		IPeer peer;

		// 전송할 패킷을 보관해놓는 큐
		Queue<Packet> sendingQueue;

		// sending_queue lock처리에 사용되는 객체
		private object csSendingQueue;

		public UserToken()
		{
			this.csSendingQueue = new object();
			this.messageResolver = new MessageResolver();
			this.peer = null;
			this.sendingQueue = new Queue<Packet>();
		}

		public void SetPeer(IPeer peer)
		{
			this.peer = peer;
		}

		public void SetEventArgs(SocketAsyncEventArgs receiveEventArgs, SocketAsyncEventArgs sendEventArgs)
		{
			this.receiveEventArgs = receiveEventArgs;
			this.sendEventArgs = sendEventArgs;
		}


		public void OnReceive(byte[] buffer, int offset, int transfered)
		{
			this.messageResolver.OnReceive(buffer, offset, transfered, OnMessage);
		}

		void OnMessage(Const<byte[]> buffer)
		{
			if (this.peer != null)
			{
				this.peer.OnMessage(buffer);
			}
		}

		public void OnRemoved()
		{
			this.sendingQueue.Clear();

			if (this.peer != null)
			{
				this.peer.OnRemoved();
			}
		}

		/// <summary>
		/// 패킷 전송
		/// </summary>
		public void Send(Packet msg)
		{
			Packet clone = new Packet();
			msg.CopyTo(clone);

			lock (this.csSendingQueue)
			{
				// 큐가 비어 있다면 큐에 추가하고 바로 비동기 전송 매소드를 호출
				if (this.sendingQueue.Count <= 0)
				{
					this.sendingQueue.Enqueue(clone);
					StartSend();
					return;
				}

				// 큐에 무언가가 들어 있다면 아직 이전 전송이 완료되지 않은 상태이므로 큐에 추가만 하고 리턴
				Console.WriteLine("Queue is not empty. Copy and Enqueue a msg. protocol id : " + msg.protocol_id);
				this.sendingQueue.Enqueue(clone);
			}
		}

		/// <summary>
		/// 비동기 전송을 시작
		/// </summary>
		void StartSend()
		{
			lock (this.csSendingQueue)
			{
				Packet msg = this.sendingQueue.Peek();
				msg.RecordSize();

				this.sendEventArgs.SetBuffer(this.sendEventArgs.Offset, msg.position);
				Array.Copy(msg.buffer, 0, this.sendEventArgs.Buffer, this.sendEventArgs.Offset, msg.position);

				// 비동기 전송 시작
				bool pending = this.socket.SendAsync(this.sendEventArgs);
				if (!pending)
				{
					ProcessSend(this.sendEventArgs);
				}
			}
		}

		static int sentCount = 0;
		static object csCount = new object();
		/// <summary>
		/// 비동기 전송 완료시 호출되는 콜백 매소드
		/// </summary>
		public void ProcessSend(SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
			{
				//Console.WriteLine(string.Format("Failed to send. error {0}, transferred {1}", e.SocketError, e.BytesTransferred));
				return;
			}

			lock (this.csSendingQueue)
			{
				if (this.sendingQueue.Count <= 0)
				{
					throw new Exception("Sending queue count is less than zero!");
				}

				int size = this.sendingQueue.Peek().position;
				if (e.BytesTransferred != size)
				{
					string error = string.Format("Need to send more! transferred {0},  packet size {1}", e.BytesTransferred, size);
					Console.WriteLine(error);
					return;
				}

				lock (csCount)
				{
					++sentCount;
					{
						Console.WriteLine(string.Format("process send : {0}, transferred {1}, sent count {2}",
							e.SocketError, e.BytesTransferred, sentCount));
					}
				}

				this.sendingQueue.Dequeue();

				// 아직 전송하지 않은 대기중인 패킷이 있다면 전송 재요청
				if (this.sendingQueue.Count > 0)
				{
					StartSend();
				}
			}
		}


		public void Disconnect()
		{
			try
			{
				this.socket.Shutdown(SocketShutdown.Send);
			}

			catch (Exception) { }
			this.socket.Close();
		}

		public void StartKeepalive()
		{
			System.Threading.Timer keepalive = new System.Threading.Timer((object e) =>
			{
				Packet msg = Packet.Create(0);
				msg.Push(0);
				Send(msg);
			}, null, 0, 3000);
		}
	}
}
