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
    // 플레이어에 관련된 정보를 담고 있는 클래스: 플레이어의 board state, score를 관리
    public class Player
    {
		GameUser owner;

		public byte player_index { get; private set; }

		public int curr_score { get; private set; }

		public int highest_node_value { get; private set; }

		public bool give_up { get; private set; }

		public Player(GameUser user, byte player_index)
		{
			this.owner = user;
			this.player_index = player_index;
			this.curr_score = 0;
			this.highest_node_value = 0;
			give_up = false;
		}

		public void reset()
		{
			this.curr_score = 0;
			this.highest_node_value = 0;
			give_up = false;
		}

		public void send(CPacket msg)
		{
			this.owner.send(msg);
			CPacket.destroy(msg);
		}

		public void send_for_broadcast(CPacket msg)
		{
			this.owner.send(msg);
		}

		public void Update_Score(int curr, int highest)
        {
			curr_score = curr;
			highest_node_value = highest;
        }

		public void Give_Up()
        {
			give_up = true;
        }
	}
}
