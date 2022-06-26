using GameNetwork;
using System;


namespace RealtimeGameServer
{
    // 접속한 유저 데이터: 유저의 요청 이벤트 관리 및 현재 속한 GameRoom을 멤버 변수로 저장 
    public class GameUser : IPeer
    {
		UserToken token;
		
		public GameRoom battleRoom { get; private set; }

		Player player;

		public GameUser(UserToken token)
		{
			this.token = token;
			this.token.SetPeer(this);
		}

		void IPeer.OnMessage(Const<byte[]> buffer)
		{
			byte[] clone = new byte[1024];
			Array.Copy(buffer.Value, clone, buffer.Value.Length);
			Packet msg = new Packet(clone, this);
			Program.gameMain.EnqueuePacket(msg, this);
		}

		void IPeer.OnRemoved()
		{
			Console.WriteLine("The client disconnected.");
			if (battleRoom != null) this.battleRoom.ProcessPT_GiveUpGame(player);
			Program.RemoveUser(this);
		}

		public void Send(Packet msg)
		{
			this.token.Send(msg);
		}

		void IPeer.Disconnect()
		{
			this.token.socket.Disconnect(false);
		}

		public void EnterRoom(Player player, GameRoom room)
		{
			this.player = player;
			this.battleRoom = room;
		}

		void IPeer.ProcessUserOperation(Packet msg)
		{
			PROTOCOL protocol = (PROTOCOL)msg.PopProtocol_ID();
			Console.WriteLine("protocol id " + protocol);

			switch (protocol)
			{
				case PROTOCOL.ENTER_GAME_ROOM_REQ:
					Program.gameMain.ProcessPT_MatchingReq(this);
					break;

				case PROTOCOL.LOADING_COMPLETED:
					this.battleRoom.ProcessPT_LoadingComplete(player);
					break;

				case PROTOCOL.EXCHANGE_NICKNAME:

					string nickName = msg.PopString();
					Console.WriteLine("Recv NickName : " + nickName);
					this.battleRoom.ProcessPT_ExchangeNickName(player, nickName);
					break;

				case PROTOCOL.MODIFIED_SCORE:
					int curr = msg.PopInt32();
					int highest = msg.PopInt32();
					this.battleRoom.ProcessPT_ModifiedScore(player, curr, highest);
					break;

				case PROTOCOL.MOVED_NODE:
					int dir = msg.PopInt32();
					this.battleRoom.ProcessPT_MovedNode(player, dir);
					break;

				case PROTOCOL.CREATED_NEW_NODE:
					int x = msg.PopInt32();
					int y = msg.PopInt32();
					this.battleRoom.ProcessPT_CreatedNewNode(player, x, y);
					break;

				case PROTOCOL.GIVE_UP_GAME:
					this.battleRoom.ProcessPT_GiveUpGame(player);
					break;
			}
		}
	}
}
