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


	NetworkManager network_manager;		// ������ ��,������ ���� ��Ʈ��ũ �Ŵ���
	GAME_STATE game_state;              // ���� ���¸� ��Ÿ��

	byte player_me_index;               // �÷��̾� ��ȣ(�ε���)
	bool is_game_finished;              // ������ ����Ǿ����� Ȯ���ϴ� ����

	Board_1P board_player;				// �÷��̾� ���� ����
	Board_2P board_rival;				// ���� ���� ����


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
	/// Server�� ��Ŷ ����
	/// </summary>
	public void On_Send(CPacket msg)
	{
		// ������ ���۵� ���¿����� ���� �̺�Ʈ �߻�
		if (this.game_state == GAME_STATE.STARTED)
		{
			this.network_manager.send(msg);
		}
	}


	/// <summary>
	/// CServer�κ��� ��Ŷ ����
	/// </summary>
	void on_recv(CPacket msg)
	{
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
		Debug.Log(">> Recv protocol id " + protocol_id);

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


	private void on_game_start(CPacket msg)  // ���� ����
	{
		this.game_state = GAME_STATE.STARTED;
		board_player.On_Game_Start();
	}

	private void On_Modified_Score(CPacket msg)  
	{
		board_rival.recv_game_event.Modified_Score(msg);
	}

	private void On_Moved_Node(CPacket msg)  
	{
		board_rival.recv_game_event.Moved_Node(msg);
	}

	private void On_Created_New_Node(CPacket msg) 
	{
		board_rival.recv_game_event.Create_Random_Node(msg);
	}


	private void on_room_removed()  // ���ӷ� ����
	{

	}

	private void on_game_over(CPacket msg)  // ���� ����
	{

	}
}
