using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
//
using TMPro;
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

	Board_Player board_player;			// �÷��̾� ���� ����
	Board_Rival board_rival;            // ���� ���� ����

	private void Awake()
    {
		GameObject.Find("BackGround").transform.Find("Messagebox_Result").gameObject.SetActive(false);
		GameObject.Find("BackGround").transform.Find("Messagebox_Start").gameObject.SetActive(false);
		GameObject.Find("Text_RivalNickName").GetComponent<TextMeshProUGUI>().text = "";
		GameObject.Find("Text_PlayerNickName").GetComponent<TextMeshProUGUI>().text = "";

		this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.game_state = GAME_STATE.READY;

		this.board_player = GameObject.Find("NodeBoard_1P").GetComponent<Board_Player>();
		this.board_rival = GameObject.Find("NodeBoard_2P").GetComponent<Board_Rival>();
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
	}


	public void Disconnect()
    {
		network_manager.Disconnect();
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
	/// Server�κ��� ��Ŷ ����
	/// </summary>
	void on_recv(CPacket msg)
	{
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
		//Debug.Log(">> Recv protocol id " + protocol_id);

		switch (protocol_id)
		{
			case PROTOCOL.GAME_START:
				On_Game_Start(msg);
				break;

			case PROTOCOL.EXCHANGE_NICKNAME:
				On_Exchange_NickName(msg);
				break;

			case PROTOCOL.MOVED_NODE:
				On_Moved_Node(msg);
				break;

			case PROTOCOL.CREATED_NEW_NODE:
				On_Created_New_Node(msg);
				break;

			case PROTOCOL.GAME_OVER:
				On_Game_Over(msg);
				break;
		}
	}

	private void On_Game_Start(CPacket msg)  // ���� ���� 
	{
		StartCoroutine(Game_Ready());

		this.game_state = GAME_STATE.STARTED;
		board_player.On_Game_Start();

		CPacket nickName = CPacket.create((short)PROTOCOL.EXCHANGE_NICKNAME);
		nickName.push(PlayerManager.Instance.nickName);
		On_Send(nickName);
	}

	private void On_Exchange_NickName(CPacket msg)
    {
        string rivalNickName = msg.pop_string();
		GameObject.Find("Text_RivalNickName").GetComponent<TextMeshProUGUI>().text = rivalNickName;
		GameObject.Find("Text_PlayerNickName").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.nickName;
	}

    private void On_Moved_Node(CPacket msg)  
	{
		board_rival.recv_game_event.Receive_Moved_Direction(msg);
	}

	private void On_Created_New_Node(CPacket msg) 
	{
		board_rival.recv_game_event.Receive_Created_Node_Location(msg);
	}

	private void On_Game_Over(CPacket msg)  // ���� ��� �� ���� (1:Win, 2:Lose, 3:Draw)
	{
		int result = msg.pop_int32();

		if (result != 0)
		{
			var player = PlayerManager.Instance;
			var query = new List<KeyValuePair<DatabaseManager.ATTRIBUTE, string>> { new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.games, (player.games + 1).ToString()) };

			if (result == 1)
			{
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.win, (player.win + 1).ToString()));
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.exp, (player.exp + 2).ToString()));
			}
			else if (result == 2)
			{
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.lose, (player.lose + 1).ToString()));
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.exp, (player.exp + 1).ToString()));
			}
			else if(result == 3)
            {
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.exp, (player.exp + 1).ToString()));
			}

			DatabaseManager.Update(query, player.id);
			board_player.game_start = false;
			StartCoroutine(Destroy_Room(result));
		}
	}


	// Delay�� ���� ���� �ż���
	private IEnumerator Game_Ready()
    {
		WaitForSeconds wait = new WaitForSeconds(1f);

		GameObject.Find("BackGround").transform.Find("Messagebox_Start").gameObject.SetActive(true);
		string theme = "_Theme3";

		for (int i = 3; i > 0; i--)
		{	
			GameObject.Find("Messagebox_Start").GetComponent<Image>().sprite
				= Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Start" + i.ToString() + theme);
			yield return wait;
		}

		GameObject.Find("BackGround").transform.Find("Messagebox_Start").gameObject.SetActive(false);
		board_player.game_start = true;
	}

	private IEnumerator Destroy_Room(int game_result)
	{
		WaitForSeconds wait = new WaitForSeconds(1f);

		GameObject.Find("BackGround").transform.Find("Messagebox_Result").gameObject.SetActive(true);
		Sprite sprite = null;
		string theme = "_Theme3";

		for (int i = 3; i > 0; i--)
		{
			if (game_result == 1)
			{
				sprite = Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Victory" + i.ToString() + theme);
			}
			else if (game_result == 2)
			{
				sprite = Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Defeated" + i.ToString() + theme);
			}
			else if (game_result == 3)
			{
				sprite = Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Draw" + i.ToString() + theme);
			}

			GameObject.Find("Messagebox_Result").GetComponent<Image>().sprite = sprite;
			yield return wait;
		}

		network_manager.Disconnect();
	}
}
