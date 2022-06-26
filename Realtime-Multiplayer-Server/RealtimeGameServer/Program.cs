using GameNetwork;
using System;
using System.Collections.Generic;
using System.Threading;


namespace RealtimeGameServer
{
    class Program
    {
        static List<GameUser> userList;
        public static GameServer gameMain = new GameServer();

        static void Main(string[] args)
        {
			PacketBufferManager.Initialize(2000);
			userList = new List<GameUser>();

			// 콜백 매소드 설정 및 초기화
			NetworkService service = new NetworkService();			
			service.sessionCreatedCallback += OnSessionCreated;
			service.Initialize();
			service.Listen("0.0.0.0", 7979, 100);

			Console.WriteLine("Started!");
			while (true)
			{
				string input = Console.ReadLine();
				Thread.Sleep(1000);
			}

			Console.ReadKey();
		}

		/// <summary>
		/// 클라이언트가 접속 완료 하였을 때 호출된다.
		/// </summary>
		static void OnSessionCreated(UserToken token)
		{
			// n개의 스레드에서 호출될 수 있으므로 공유 자원 접근시 동기화 처리
			GameUser user = new GameUser(token);
			lock (userList)
			{
				userList.Add(user);
			}
		}

		public static void RemoveUser(GameUser user)
		{
			lock (userList)
			{
				userList.Remove(user);
				gameMain.UserDisconnected(user);

				GameRoom room = user.battleRoom;
				if (room != null)
				{
					gameMain.roomManager.RemoveRoom(user.battleRoom);
				}
			}
		}
	}
}
