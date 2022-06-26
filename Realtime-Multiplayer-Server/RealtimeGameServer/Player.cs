using GameNetwork;


namespace RealtimeGameServer
{
    // 플레이어에 관련된 정보를 담고 있는 클래스: 플레이어의 board state, score를 관리
    public class Player
    {
		GameUser owner;

		public byte playerIndex { get; private set; }

		public int currScore { get; private set; }

		public int highestNodeValue { get; private set; }

		public bool giveUp { get; private set; }

		public Player(GameUser user, byte index)
		{
			this.owner = user;
			this.playerIndex = index;
			this.currScore = 0;
			this.highestNodeValue = 0;
			giveUp = false;
		}

		public void Reset()
		{
			this.currScore = 0;
			this.highestNodeValue = 0;
			giveUp = false;
		}

		public void Send(Packet msg)
		{
			this.owner.Send(msg);
			Packet.Destroy(msg);
		}

		public void UpdateScore(int curr, int highest)
        {
			currScore = curr;
			highestNodeValue = highest;
        }

		public void GiveUp()
        {
			giveUp = true;
        }
	}
}
