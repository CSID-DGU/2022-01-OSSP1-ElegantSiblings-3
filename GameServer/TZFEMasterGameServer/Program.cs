using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Threading;
using System.Net.Sockets;
using System.Net;
using FreeNet;

namespace TZFEMasterGameServer
{
    class Program
    {
        static List<GameUser> userlist;
        public static GameServer game_main = new GameServer();

        static void Main(string[] args)
        {
			CPacketBufferManager.initialize(2000);
			userlist = new List<GameUser>();

			// 콜백 매소드 설정 및 초기화
			CNetworkService service = new CNetworkService();			
			service.session_created_callback += on_session_created;
			service.initialize();
			service.listen("0.0.0.0", 7979, 100);

			Console.WriteLine("Started!");
			while (true)
			{
				string input = Console.ReadLine();
				Thread.Sleep(1000);
			}

			Console.ReadKey();
		}

		/// <summary>
		/// 클라이언트가 접속 완료 하였을 때 호출된다. n개의 스레드에서 호출될 수 있으므로 공유 자원 접근시 동기화 처리
		/// </summary>
		static void on_session_created(CUserToken token)
		{
			GameUser user = new GameUser(token);
			lock (userlist)
			{
				userlist.Add(user);
			}
		}

		public static void remove_user(GameUser user)
		{
			lock (userlist)
			{
				userlist.Remove(user);
				game_main.user_disconnected(user);

				GameRoom room = user.battle_room;
				if (room != null)
				{
					game_main.room_manager.remove_room(user.battle_room);
				}
			}
		}
	}
}
