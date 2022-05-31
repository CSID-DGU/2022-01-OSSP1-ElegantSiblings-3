using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public class BattleRoom : MonoBehaviour
{
	enum GAME_STATE
	{
		READY = 0,
		STARTED
	}


	NetworkManager network_manager;		// 데이터 송,수신을 위한 네트워크 매니저
	GAME_STATE game_state;              // 게임 상태를 나타냄

	byte player_me_index;               // 플레이어 번호(인덱스)
	bool is_game_finished;              // 게임이 종료되었는지 확인하는 변수

	Board_1P board_player;				// 플레이어 게임 보드
	Board_2P board_rival;				// 상대방 게임 보드


	private void Awake()
    {
		this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.game_state = GAME_STATE.READY;

		this.board_player = GameObject.Find("NodeBoard_1P").GetComponent<Board_1P>();
		this.board_rival = GameObject.Find("NodeBoard_2P").GetComponent<Board_2P>();
	}


	void clear()
	{
		this.is_game_finished = false;
	}


	public void start_loading(byte player_me_index)
    {
		clear();

		this.network_manager.message_receiver = this;
		this.player_me_index = player_me_index;

		CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);
		this.network_manager.send(msg);

		Debug.Log("3");
		Thread.Sleep(1000);

		Debug.Log("2");
		Thread.Sleep(1000);

		Debug.Log("1");
		Thread.Sleep(1000);

		Debug.Log("Hello~~ Player" + player_me_index);
	}


	/// <summary>
	/// Server로 패킷 전송
	/// </summary>
	public void On_Send(CPacket msg)
	{
		// 게임이 시작된 상태에서만 게임 이벤트 발생
		if (this.game_state == GAME_STATE.STARTED)
		{
			this.network_manager.send(msg);
		}
	}


	/// <summary>
	/// CServer로부터 패킷 수신
	/// </summary>
	void on_recv(CPacket msg)
	{
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		switch (protocol_id)
		{
			case PROTOCOL.GAME_START:
				on_game_start(msg);
				break;

			case PROTOCOL.MODIFIED_SCORE:
				On_Modified_Score(msg);
				break;

			case PROTOCOL.MOVED_NODE:
				On_Moved_Node(msg);
				break;

			case PROTOCOL.CREATED_NEW_NODE:
				On_Created_New_Node(msg);
				break;

			case PROTOCOL.ROOM_REMOVED:
				on_room_removed();
				break;

			case PROTOCOL.GAME_OVER:
				on_game_over(msg);
				break;
		}
	}


	private void on_game_start(CPacket msg)  // 게임 시작
	{
		this.game_state = GAME_STATE.STARTED;
		board_player.On_Game_Start();
	}

	private void On_Modified_Score(CPacket msg)  // 플레이어가 블록을 이동시킴
	{

	}

	private void On_Moved_Node(CPacket msg)  // 플레이어가 블록을 이동시킴
	{

	}

	private void On_Created_New_Node(CPacket msg)  // 플레이어가 블록을 이동시킴
	{

	}


	private void on_room_removed()  // 게임룸 삭제
	{

	}

	private void on_game_over(CPacket msg)  // 게임 종료
	{

	}
}
