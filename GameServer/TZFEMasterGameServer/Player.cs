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

		public short highest_node_num { get; private set; }


		public Player(GameUser user, byte player_index)
		{
			this.owner = user;
			this.player_index = player_index;
			this.highest_node_num = 0;
		}

		public void reset()
		{
			this.highest_node_num = 0;
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
	}
}
