using GameNetwork;
using System;
using System.Collections.Generic;


namespace RealtimeGameServer
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
		Dictionary<byte, PLAYER_STATE> playerState;		// 플레이어 상태를 관리
		bool isGameOver;								// 게임이 끝났는지 체크

		public GameRoom()
		{
			this.players = new List<Player>();
			this.playerState = new Dictionary<byte, PLAYER_STATE>();
			isGameOver = false;
		}


		/// <summary>
		/// 매칭이된 플레이어들이 게임룸에 입장
		/// </summary>
		public void EnterGameroom(GameUser user1, GameUser user2)
		{
			// 플레이어들을 생성하고 인덱스 부여
			Player player1 = new Player(user1, 0);  // 1P
			Player player2 = new Player(user2, 1);  // 2P
			this.players.Clear();
			this.players.Add(player1);
			this.players.Add(player2);

			// 플레이어들의 초기 상태를 지정
			this.playerState.Clear();
			ChangePlayerstate(player1, PLAYER_STATE.ENTERED_ROOM);
			ChangePlayerstate(player2, PLAYER_STATE.ENTERED_ROOM);

			// 로딩 시작메시지 전송
			this.players.ForEach(player =>
			{
				Packet msg = Packet.Create((Int16)PROTOCOL.START_LOADING);
				msg.Push(player.playerIndex);  // 플레이어 인덱스를 
				player.Send(msg);
			});

			user1.EnterRoom(player1, this);
			user2.EnterRoom(player2, this);
		}


		public void Destroy()
		{
			Packet msg = Packet.Create((short)PROTOCOL.ROOM_REMOVED);
			SendAllPlayer(msg);
			this.players.Clear();
		}


		/// <summary>
		/// 모든 유저들에게 메시지를 전송
		/// </summary>
		void SendAllPlayer(Packet msg)
		{
			this.players.ForEach(player => player.Send(msg));
			Packet.Destroy(msg);
		}


		/// <summary>
		/// 플레이어의 상태를 변경
		/// </summary>
		void ChangePlayerstate(Player player, PLAYER_STATE state)
		{
			if (this.playerState.ContainsKey(player.playerIndex))
			{
				this.playerState[player.playerIndex] = state;
			}
			else
			{
				this.playerState.Add(player.playerIndex, state);
			}
		}


		/// <summary>
		/// 클라이언트에서 로딩이 완료되면 게임 준비가 완료됐다고 요청
		/// </summary>
		public void ProcessPT_LoadingComplete(Player player)
		{
			// 해당 플레이어를 로딩 완료 상태로 변경
			ChangePlayerstate(player, PLAYER_STATE.LOADING_COMPLETE);

			// 모든 유저가 준비 상태인지 체크
			if (!AllplayersReady(PLAYER_STATE.LOADING_COMPLETE)) return;

			// 모두 준비 되었다면 게임을 시작
			BattleStart();
		}


		/// <summary>
		/// 모든 플레이어가 특정 상태가 되었는지를 확인한다
		/// 모든 플레이어가 같은 상태에 있다면 true, 한명이라도 다른 상태에 있다면 false를 리턴
		/// </summary>
		bool AllplayersReady(PLAYER_STATE state)
		{
			foreach (KeyValuePair<byte, PLAYER_STATE> kvp in this.playerState)
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
		void BattleStart()
		{
			// 게임 시작 메시지 전송.
			Packet msg = Packet.Create((short)PROTOCOL.GAME_START);
			msg.Push((byte)this.players.Count);
			SendAllPlayer(msg);
		}


		/// <summary>
		/// Game Rule
		/// </summary>
		void GameRule(Player player1, Player player2, short protocol)  // 0: Not Fin, 1: Player Win, 2: Rival Win, 3: Draw
		{
			if (protocol == (short)PROTOCOL.MODIFIED_SCORE)
			{
				int check_highest = 128;//2048;

				if (player1.highestNodeValue >= check_highest || player2.highestNodeValue >= check_highest)
				{
					Packet msg_player1 = Packet.Create((short)PROTOCOL.GAME_OVER);
					Packet msg_player2 = Packet.Create((short)PROTOCOL.GAME_OVER);

					if (player1.highestNodeValue >= check_highest && player2.highestNodeValue >= check_highest)
					{
						msg_player1.Push(3);
						msg_player2.Push(3);
					}
					else if (player1.highestNodeValue >= check_highest)
					{
						msg_player1.Push(1);
						msg_player2.Push(2);
					}
					else if (player2.highestNodeValue >= check_highest)
					{
						msg_player1.Push(2);
						msg_player2.Push(1);
					}

					player1.Send(msg_player1);
					player2.Send(msg_player2);
					isGameOver = true;
				}
			}
			else if (protocol == (short)PROTOCOL.GIVE_UP_GAME)
			{
				Packet msg_player1 = Packet.Create((short)PROTOCOL.GAME_OVER);
				Packet msg_player2 = Packet.Create((short)PROTOCOL.GAME_OVER);

				if (player1.giveUp && player2.giveUp)
				{
					msg_player1.Push(3);
					msg_player2.Push(3);
				}
				else if(player1.giveUp)
                {
					msg_player1.Push(2);
					msg_player2.Push(1);
				}
				else if (player2.giveUp)
				{
					msg_player1.Push(1);
					msg_player2.Push(2);
				}

				player1.Send(msg_player1);
				player2.Send(msg_player2);
				isGameOver = true;
			}
			
			return;
		}


		/// <summary>
		/// 플레이어의 경쟁 상대를 리턴
		/// </summary>
		private Player GetRival(Player player)
        {
			return (player.playerIndex == 0 ? players[1] : players[0]);
        }


		// 전송받은 Protocol에 맞는 매서드 구현
		public void ProcessPT_ModifiedScore(Player sender, int curr, int highest) 
		{
			if (isGameOver) return;
			sender.UpdateScore(curr, highest);
			GameRule(sender, GetRival(sender), (short)PROTOCOL.MODIFIED_SCORE);
		}

		public void ProcessPT_MovedNode(Player sender, int dir)
		{
			if (isGameOver) return;
			Packet msg = Packet.Create((short)PROTOCOL.MOVED_NODE);
			msg.Push(dir);
			GetRival(sender).Send(msg);
		}

		public void ProcessPT_CreatedNewNode(Player sender, int x, int y)
		{
			if (isGameOver) return;
			Packet msg = Packet.Create((short)PROTOCOL.CREATED_NEW_NODE);
			msg.Push(x);
			msg.Push(y);
			GetRival(sender).Send(msg);
		}

		public void ProcessPT_GiveUpGame(Player sender)
        {
			if (isGameOver) return;
			sender.GiveUp();
			GameRule(sender, GetRival(sender), (short)PROTOCOL.GIVE_UP_GAME);
		}

		public void ProcessPT_ExchangeNickName(Player sender, string nickName)
        {
			if (isGameOver) return;
			Packet msg = Packet.Create((short)PROTOCOL.EXCHANGE_NICKNAME);
			msg.Push(nickName);
			GetRival(sender).Send(msg);
		}
	}
}
