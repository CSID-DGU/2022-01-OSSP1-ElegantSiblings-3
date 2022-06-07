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
	// GameClient와 통신을 위한 protocol을 정의
	public enum PROTOCOL : short
	{
		BEGIN = 0,

		// 로딩
		START_LOADING = 1,

		// 로딩 완료
		LOADING_COMPLETED = 2,

		// 게임 시작
		GAME_START = 3,

		// 점수 갱신 (누적 점수, 최대 크기 노드)
		MODIFIED_SCORE = 4,

		// 노드가 이동됨
		MOVED_NODE = 5,

		// 노드(랜덤 노드)가 생성됨
		CREATED_NEW_NODE = 6,

		// 게임 포기
		GIVE_UP_GAME = 7,

		// 게임 규칙에 의해 게임 종료 (플레이어 중 2048을 달성한 사람이 있는가? 항복을 한 사람이 있는가?)
		GAME_OVER = 8,

		// 게임룸 삭제
		ROOM_REMOVED = 9,

		// 게임룸 입장 요청
		ENTER_GAME_ROOM_REQ = 10,

		// 닉네임 교환
		EXCHANGE_NICKNAME = 11,

		END
	}
}
