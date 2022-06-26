using GameNetwork;
using System.Collections.Generic;
using System.Threading;


namespace RealtimeGameServer
{
    // Game Server
    class GameServer
    {
		object operationLock;
		Queue<Packet> userOperations;

		// 로직은 하나의 스레드로만 처리
		Thread logicThread;
		AutoResetEvent loopEvent;

		public GameRoomManager roomManager { get; private set; }

		// 매칭 대기 리스트.
		List<GameUser> matchingWaitingUsers;


		public GameServer()
		{
			this.operationLock = new object();
			this.loopEvent = new AutoResetEvent(false);
			this.userOperations = new Queue<Packet>();

			// 게임 로직 관련.
			this.roomManager = new GameRoomManager();
			this.matchingWaitingUsers = new List<GameUser>();

			this.logicThread = new Thread(GameLoop);
			this.logicThread.Start();
		}


		/// <summary>
		/// 유저 패킷 처리
		/// </summary>
		void GameLoop()
		{
			while (true)
			{
				Packet packet = null;
				lock (this.operationLock)
				{
					if (this.userOperations.Count > 0)
					{
						packet = this.userOperations.Dequeue();
					}
				}

				if (packet != null)
				{
					// 패킷 처리
					ProcessReceive(packet);
				}

				// 더이상 처리할 패킷이 없으면 스레드 대기
				if (this.userOperations.Count <= 0)
				{
					this.loopEvent.WaitOne();
				}
			}
		}

		public void EnqueuePacket(Packet packet, GameUser user)
		{
			lock (this.operationLock)
			{
				this.userOperations.Enqueue(packet);
				this.loopEvent.Set();
			}
		}

		void ProcessReceive(Packet msg)
		{
			// GameUser
			msg.owner.ProcessUserOperation(msg);
		}


		/// <summary>
		/// 유저로부터 매칭 요청이 왔을 때 호출
		/// </summary>
		public void ProcessPT_MatchingReq(GameUser user)
		{
			// 대기 리스트에 중복 추가 되지 않도록 체크
			if (this.matchingWaitingUsers.Contains(user)) return;
			
			// 매칭 대기 리스트에 추가
			this.matchingWaitingUsers.Add(user);

			// 2명이 모이면 매칭 성공
			if (this.matchingWaitingUsers.Count == 2)
			{
				// 게임 방 생성
				this.roomManager.CreateRoom(this.matchingWaitingUsers[0], this.matchingWaitingUsers[1]);

				// 매칭 대기 리스트 삭제
				this.matchingWaitingUsers.Clear();
			}
		}


		public void UserDisconnected(GameUser user)
		{
			if (this.matchingWaitingUsers.Contains(user))
			{
				this.matchingWaitingUsers.Remove(user);
			}
		}
	}
}
