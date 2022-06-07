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
	// 게임 로직을 구현한 GameRoom
	public class GameRoom
	{
		enum PLAYER_STATE : byte
		{
			ENTERED_ROOM,           // 방에 입장한 상태  		
			LOADING_COMPLETE,       // 로딩을 완료한 상태	
		}

		List<Player> players;							// 게임을 진행하는 플레이어 (1P, 2P)
		Dictionary<byte, PLAYER_STATE> player_state;    // 플레이어 상태를 관리
		bool is_game_over;

		public GameRoom()
		{
			this.players = new List<Player>();
			this.player_state = new Dictionary<byte, PLAYER_STATE>();
			is_game_over = false;
		}


		/// <summary>
		/// 매칭이된 플레이어들이 게임룸에 입장
		/// </summary>
		public void enter_gameroom(GameUser user1, GameUser user2)
		{
			// 플레이어들을 생성하고 인덱스 부여
			Player player1 = new Player(user1, 0);  // 1P
			Player player2 = new Player(user2, 1);  // 2P
			this.players.Clear();
			this.players.Add(player1);
			this.players.Add(player2);

			// 플레이어들의 초기 상태를 지정해 준다.
			this.player_state.Clear();
			change_playerstate(player1, PLAYER_STATE.ENTERED_ROOM);
			change_playerstate(player2, PLAYER_STATE.ENTERED_ROOM);

			// 로딩 시작메시지 전송.
			this.players.ForEach(player =>
			{
				CPacket msg = CPacket.create((Int16)PROTOCOL.START_LOADING);
				msg.push(player.player_index);  // 플레이어 인덱스를 
				player.send(msg);
			});

			user1.Enter_Room(player1, this);
			user2.Enter_Room(player2, this);
		}


		public void destroy()
		{
			CPacket msg = CPacket.create((short)PROTOCOL.ROOM_REMOVED);
			broadcast(msg);
			this.players.Clear();
		}


		/// <summary>
		/// 모든 유저들에게 메시지를 전송한다.
		/// </summary>
		void broadcast(CPacket msg)
		{
			this.players.ForEach(player => player.send_for_broadcast(msg));
			CPacket.destroy(msg);
		}


		/// <summary>
		/// 플레이어의 상태를 변경한다.
		/// </summary>
		void change_playerstate(Player player, PLAYER_STATE state)
		{
			if (this.player_state.ContainsKey(player.player_index))
			{
				this.player_state[player.player_index] = state;
			}
			else
			{
				this.player_state.Add(player.player_index, state);
			}
		}


		/// <summary>
		/// 클라이언트에서 로딩이 완료되면 게임 준비가 완료됐다고 요청을 보낸다
		/// </summary>
		public void loading_complete(Player player)
		{
			// 해당 플레이어를 로딩 완료 상태로 변경
			change_playerstate(player, PLAYER_STATE.LOADING_COMPLETE);

			// 모든 유저가 준비 상태인지 체크
			if (!allplayers_ready(PLAYER_STATE.LOADING_COMPLETE)) return;

			// 모두 준비 되었다면 게임을 시작
			battle_start();
		}


		/// <summary>
		/// 모든 플레이어가 특정 상태가 되었는지를 확인한다
		/// 모든 플레이어가 같은 상태에 있다면 true, 한명이라도 다른 상태에 있다면 false를 리턴
		/// </summary>
		bool allplayers_ready(PLAYER_STATE state)
		{
			foreach (KeyValuePair<byte, PLAYER_STATE> kvp in this.player_state)
			{
				if (kvp.Value != state)
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>
		/// 게임 시작
		/// </summary>
		void battle_start()
		{
			// 게임 시작 메시지 전송.
			CPacket msg = CPacket.create((short)PROTOCOL.GAME_START);
			msg.push((byte)this.players.Count);
			broadcast(msg);
		}



		/// <summary>
		/// Game Rule
		/// </summary>
		void Game_Rule(Player player1, Player player2, short protocol)  // 0: Not Fin, 1: Player Win, 2: Rival Win, 3: Draw
		{
			if (protocol == (short)PROTOCOL.MODIFIED_SCORE)
			{
				int check_highest = 128;//2048;

				if (player1.highest_node_value >= check_highest || player2.highest_node_value >= check_highest)
				{
					CPacket msg_player1 = CPacket.create((short)PROTOCOL.GAME_OVER);
					CPacket msg_player2 = CPacket.create((short)PROTOCOL.GAME_OVER);

					if (player1.highest_node_value >= check_highest && player2.highest_node_value >= check_highest)
					{
						msg_player1.push(3);
						msg_player2.push(3);
					}
					else if (player1.highest_node_value >= check_highest)
					{
						msg_player1.push(1);
						msg_player2.push(2);
					}
					else if (player2.highest_node_value >= check_highest)
					{
						msg_player1.push(2);
						msg_player2.push(1);
					}

					player1.send(msg_player1);
					player2.send(msg_player2);
					is_game_over = true;
				}
			}
			else if (protocol == (short)PROTOCOL.GIVE_UP_GAME)
			{
				CPacket msg_player1 = CPacket.create((short)PROTOCOL.GAME_OVER);
				CPacket msg_player2 = CPacket.create((short)PROTOCOL.GAME_OVER);

				if (player1.give_up && player2.give_up)
				{
					msg_player1.push(3);
					msg_player2.push(3);
				}
				else if(player1.give_up)
                {
					msg_player1.push(2);
					msg_player2.push(1);
				}
				else if (player2.give_up)
				{
					msg_player1.push(1);
					msg_player2.push(2);
				}

				player1.send(msg_player1);
				player2.send(msg_player2);
				is_game_over = true;
			}
			
			return;
		}


		/// <summary>
		/// 플레이어의 경쟁 상대를 리턴
		/// </summary>
		private Player Get_Rival(Player player)
        {
			return (player.player_index == 0 ? players[1] : players[0]);
        }


		public void On_Modified_Score(Player sender, int curr, int highest) 
		{
			if (is_game_over) return;
			sender.Update_Score(curr, highest);
			Game_Rule(sender, Get_Rival(sender), (short)PROTOCOL.MODIFIED_SCORE);
		}


		public void On_Moved_Node(Player sender, int dir)
		{
			if (is_game_over) return;
			CPacket msg = CPacket.create((short)PROTOCOL.MOVED_NODE);
			msg.push(dir);
			Get_Rival(sender).send(msg);
		}

		public void On_Created_New_Node(Player sender, int x, int y)
		{
			if (is_game_over) return;
			CPacket msg = CPacket.create((short)PROTOCOL.CREATED_NEW_NODE);
			msg.push(x);
			msg.push(y);
			Get_Rival(sender).send(msg);
		}

		public void On_Give_Up_Game(Player sender)
        {
			if (is_game_over) return;
			sender.Give_Up();
			Game_Rule(sender, Get_Rival(sender), (short)PROTOCOL.GIVE_UP_GAME);
		}

		public void On_Exchange_NickName(Player sender, string nickName)
        {
			if (is_game_over) return;
			CPacket msg = CPacket.create((short)PROTOCOL.EXCHANGE_NICKNAME);
			msg.push(nickName);
			Get_Rival(sender).send(msg);
		}
	}
}
