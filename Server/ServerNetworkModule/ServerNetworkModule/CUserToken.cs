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
	/// <summary>
	/// 접속자 수 만큼 CUserToken가 생성된다
	/// CUserToken로 어느 Client가 전송했는지, 어떤 Client에 전송해야하는지 알 수 있다
	/// </summary>
	/// 
	public class CUserToken
	{
		public Socket socket { get; set; }

		public SocketAsyncEventArgs receive_event_args { get; private set; }
		public SocketAsyncEventArgs send_event_args { get; private set; }
		
		CMessageResolver message_resolver;		// 바이트를 패킷 형식으로 해석
		IPeer peer;								// session객체. 어플리케이션 딴에서 구현하여 사용.
		Queue<CPacket> sending_queue;			// 전송할 패킷을 보관해놓는 큐. 1-Send로 처리하기 위한 큐이다.
		private object cs_sending_queue;		// sending_queue lock처리에 사용되는 객체.

		public CUserToken()
		{
			this.cs_sending_queue = new object();

			this.message_resolver = new CMessageResolver();
			this.peer = null;
			this.sending_queue = new Queue<CPacket>();
		}

		public void set_peer(IPeer peer)
		{
			this.peer = peer;
		}

		public void set_event_args(SocketAsyncEventArgs receive_event_args, SocketAsyncEventArgs send_event_args)
		{
			this.receive_event_args = receive_event_args;
			this.send_event_args = send_event_args;
		}


		public void on_receive(byte[] buffer, int offset, int transfered)
		{
			this.message_resolver.on_receive(buffer, offset, transfered, on_message);
		}


		void on_message(Const<byte[]> buffer)
		{
			if (this.peer != null)
			{
				this.peer.on_message(buffer);
			}
		}


		public void on_removed()
		{
			this.sending_queue.Clear();

			if (this.peer != null)
			{
				this.peer.on_removed();
			}
		}

		/// <summary>
		/// 코드 수정이 필요할 수 있음!!!!!!!!!!!!!!!!!!!!!!
		/// 패킷을 전송
		/// 큐가 비어 있을 경우에는 큐에 추가한 뒤 바로 SendAsync매소드를 호출하고, 데이터가 들어있을 경우에는 새로 추가만 한다.
		/// 큐잉된 패킷의 전송 시점: 현재 진행중인 SendAsync가 완료되었을 때 큐를 검사하여 나머지 패킷을 전송한다.
		/// </summary>

		public void send(CPacket msg)
		{
			CPacket clone = new CPacket();
			msg.copy_to(clone);

			lock (this.cs_sending_queue)
			{
				// 큐가 비어 있다면 큐에 추가하고 바로 비동기 전송 매소드를 호출한다.
				if (this.sending_queue.Count <= 0)
				{
					this.sending_queue.Enqueue(clone);
					//this.sending_queue.Enqueue(msg);

					start_send();
					return;
				}

				//Console.WriteLine("Queue is not empty. Copy and Enqueue a msg. protocol id : " + msg.protocol_id);
				this.sending_queue.Enqueue(clone);
				//this.sending_queue.Enqueue(msg);
			}
		}

		/// <summary>
		/// 비동기 전송을 시작
		/// </summary>
		void start_send()
		{
			lock (this.cs_sending_queue)
			{
				CPacket msg = this.sending_queue.Peek();	// 전송이 완료된 상태가 아니므로 데이터만 가져오고 큐에서 제거하진 않는다
				msg.record_size();							// 헤더에 패킷 사이즈를 기록

				// 보낼 패킷 사이즈 만큼 버퍼 크기를 설정
				this.send_event_args.SetBuffer(this.send_event_args.Offset, msg.position);

				// 패킷 내용을 SocketAsyncEventArgs버퍼에 복사
				Array.Copy(msg.buffer, 0, this.send_event_args.Buffer, this.send_event_args.Offset, msg.position);

				// 비동기 전송 시작.
				bool pending = this.socket.SendAsync(this.send_event_args);
				if (!pending) process_send(this.send_event_args);			
			}
		}

		static int sent_count = 0;
		static object cs_count = new object();
		/// <summary>
		/// 비동기 전송 완료시 호출되는 콜백 매소드.
		/// </summary>
		/// <param name="e"></param>
		public void process_send(SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
			{
				//Console.WriteLine(string.Format("Failed to send. error {0}, transferred {1}", e.SocketError, e.BytesTransferred));
				return;
			}

			lock (this.cs_sending_queue)
			{
				if (this.sending_queue.Count <= 0) throw new Exception("Sending queue count is less than zero!");
				
				int size = this.sending_queue.Peek().position;
				if (e.BytesTransferred != size)
				{
					string error = string.Format("Need to send more! transferred {0},  packet size {1}", e.BytesTransferred, size);
					Console.WriteLine(error);
					return;
				}

				//System.Threading.Interlocked.Increment(ref sent_count);
				lock (cs_count)
				{
					++sent_count;
					//if (sent_count % 20000 == 0)
					{
						Console.WriteLine(string.Format("process send : {0}, transferred {1}, sent count {2}",
							e.SocketError, e.BytesTransferred, sent_count));
					}
				}

				//Console.WriteLine(string.Format("process send : {0}, transferred {1}, sent count {2}", e.SocketError, e.BytesTransferred, sent_count));
			
				this.sending_queue.Dequeue();	// 전송 완료된 패킷을 큐에서 제거
			
				if (this.sending_queue.Count > 0) start_send();		// 아직 전송하지 않은 대기중인 패킷이 있다면 다시한번 전송을 요청
			}
		}

		//void send_directly(CPacket msg)
		//{
		//	msg.record_size();
		//	this.send_event_args.SetBuffer(this.send_event_args.Offset, msg.position);
		//	Array.Copy(msg.buffer, 0, this.send_event_args.Buffer, this.send_event_args.Offset, msg.position);
		//	bool pending = this.socket.SendAsync(this.send_event_args);
		//	if (!pending)
		//	{
		//		process_send(this.send_event_args);
		//	}
		//}

		public void disconnect()
		{
			// close the socket associated with the client
			try
			{
				this.socket.Shutdown(SocketShutdown.Send);
			}
			// throws if client process has already closed
			catch (Exception) { }
			this.socket.Close();
		}

		public void start_keepalive()
		{
			System.Threading.Timer keepalive = new System.Threading.Timer((object e) =>
			{
				CPacket msg = CPacket.create(0);
				msg.push(0);
				send(msg);
			}, null, 0, 3000);
		}
	}
}
