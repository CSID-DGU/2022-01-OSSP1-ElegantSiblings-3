using System.Collections.Generic;


namespace RealtimeGameServer
{
    // GameRoom 관리
    public class GameRoomManager
    {
        List<GameRoom> rooms;

        public GameRoomManager()
        {
            this.rooms = new List<GameRoom>();
        }

        /// <summary>
        /// 매칭을 요청한 유저들을 넘겨 받아 게임 방을 생성한다.
        /// </summary>
        public void CreateRoom(GameUser user1, GameUser user2)
        {
            // 게임 방을 생성하여 2명 입장
            GameRoom battleroom = new GameRoom();
            battleroom.EnterGameroom(user1, user2);

            // 방 리스트에 추가 하여 관리
            this.rooms.Add(battleroom);
        }

        public void RemoveRoom(GameRoom room)
        {
            room.Destroy();
            this.rooms.Remove(room);
        }
    }
}
