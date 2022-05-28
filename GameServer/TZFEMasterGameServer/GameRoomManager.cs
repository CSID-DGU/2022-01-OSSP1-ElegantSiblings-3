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
        public void create_room(GameUser user1, GameUser user2)
        {
            // 게임 방을 생성하여 2명 입장
            GameRoom battleroom = new GameRoom();
            battleroom.enter_gameroom(user1, user2);

            // 방 리스트에 추가 하여 관리
            this.rooms.Add(battleroom);
        }

        public void remove_room(GameRoom room)
        {
            room.destroy();
            this.rooms.Remove(room);
        }
    }
}
