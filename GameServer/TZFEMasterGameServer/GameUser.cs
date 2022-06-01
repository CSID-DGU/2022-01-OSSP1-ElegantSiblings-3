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
	// 접속한 유저 데이터: 유저의 요청 이벤트 관리 및 현재 속한 GameRoom을 멤버 변수로 저장 
	public class GameUser : IPeer
    {
		CUserToken token;
		
		public GameRoom battle_room { get; private set; }

		Player player;

		public GameUser(CUserToken token)
		{
			this.token = token;
			this.token.set_peer(this);
		}

		void IPeer.on_message(Const<byte[]> buffer)
		{
			byte[] clone = new byte[1024];
			Array.Copy(buffer.Value, clone, buffer.Value.Length);
			CPacket msg = new CPacket(clone, this);
			Program.game_main.enqueue_packet(msg, this);
		}

		void IPeer.on_removed()
		{
			Console.WriteLine("The client disconnected.");

			Program.remove_user(this);
		}

		public void send(CPacket msg)
		{
			this.token.send(msg);
		}

		void IPeer.disconnect()
		{
			this.token.socket.Disconnect(false);
		}

		void IPeer.process_user_operation(CPacket msg)
		{
			PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
			Console.WriteLine("protocol id " + protocol);

			switch (protocol)
			{
				case PROTOCOL.ENTER_GAME_ROOM_REQ:
					Program.game_main.matching_req(this);
					break;

				case PROTOCOL.LOADING_COMPLETED:
					this.battle_room.loading_complete(player);
					break;


				case PROTOCOL.MODIFIED_SCORE:
					//Console.WriteLine("Sended by Player" + player.player_index);
					int curr = msg.pop_int32();
					int highest = msg.pop_int32();
					//Console.WriteLine("Modified score is (" + curr.ToString() + ", " + highest.ToString() + ")");
					this.battle_room.On_Modified_Score(player, curr, highest);
					break;

				case PROTOCOL.MOVED_NODE:
					//Console.WriteLine("Sended by Player" + player.player_index);
					int dir = msg.pop_int32();
					//Console.WriteLine("Mode To " + dir.ToString());
					this.battle_room.On_Moved_Node(player, dir);
					break;

				case PROTOCOL.CREATED_NEW_NODE:
					//Console.WriteLine("Sended by Player" + player.player_index);
					int x = msg.pop_int32();
					int y = msg.pop_int32();
					//Console.WriteLine("New node location (" + x.ToString() + ", " + y.ToString() + ")");
					this.battle_room.On_Created_New_Node(player, x, y);
					break;
			}
		}

		public void enter_room(Player player, GameRoom room)
		{
			this.player = player;
			this.battle_room = room;
		}
	}
}
