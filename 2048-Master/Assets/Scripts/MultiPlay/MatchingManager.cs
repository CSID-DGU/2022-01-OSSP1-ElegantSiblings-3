using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public class MatchingManager : MonoBehaviour
{
	enum USER_STATE
	{
		NOT_CONNECTED,
		CONNECTED,
		WAITING_MATCHING
	}


	NetworkManager network_manager;
	USER_STATE user_state;
	BattleRoom battle_room;

	string theme_name;
	Texture matching_bg;
	List<Texture> waiting_img;
	int waiting_count;


	// Use this for initialization
	void Start()
	{
		this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.user_state = USER_STATE.NOT_CONNECTED;

		this.battle_room = GameObject.Find("BattleRoom").GetComponent<BattleRoom>();
		this.battle_room.gameObject.SetActive(false);

		// TODO: theme_name load
		theme_name = "_Theme3";

		this.matching_bg = Resources.Load("theme3/Scene_GameRoom_Background_Matching" + theme_name) as Texture;
		this.waiting_img = new List<Texture>
		{
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting0" + theme_name) as Texture,
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting1" + theme_name) as Texture,
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting2" + theme_name) as Texture,
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting3" + theme_name) as Texture
		};
		this.waiting_count = 0;

		this.user_state = USER_STATE.NOT_CONNECTED;
		enter();
	}


	public void enter()
	{
		StopCoroutine("after_connected");

		this.network_manager.message_receiver = this;

		if (!this.network_manager.is_connected())
		{
			this.user_state = USER_STATE.CONNECTED;
			this.network_manager.connect();
		}
		else
		{
			on_connected();
		}
	}



	IEnumerator after_connected()
	{
		yield return new WaitForEndOfFrame();

		while (true)
		{
			if (this.user_state == USER_STATE.CONNECTED)
			{
				this.user_state = USER_STATE.WAITING_MATCHING;

				// ������ ������ �Ϸ�Ǿ����� ���ӷ� ���� ��û
				CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
				this.network_manager.send(msg);
				StopCoroutine("after_connected");
			}

			yield return 0;
		}
	}

	void OnGUI()
	{
		switch (this.user_state)
		{
			case USER_STATE.NOT_CONNECTED:
				break;

			case USER_STATE.CONNECTED:
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.matching_bg);
				break;

			case USER_STATE.WAITING_MATCHING:				
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.matching_bg);
				//GUI.DrawTexture(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 82), this.waiting_img[(waiting_count / 10) % 4]);
				GUI.DrawTexture(new Rect(Screen.width / 2 - (Screen.width / 4.35f / 2), Screen.height / 2 - (Screen.height / 6f / 2), Screen.width / 4.35f, Screen.height / 6f), this.waiting_img[(waiting_count / 10) % 4]);				
				if (++waiting_count >= 2000) waiting_count = 0;
				Thread.Sleep(50);

				break;
		}
	}


	/// <summary>
	/// ������ ������ �Ϸ�Ǹ� ȣ���.
	/// </summary>
	public void on_connected()
	{
		this.user_state = USER_STATE.CONNECTED;
		StartCoroutine("after_connected");
	}


	/// <summary>
	/// ��Ŷ�� ���� ���� �� ȣ��
	/// </summary>
	public void on_recv(CPacket msg)
	{
		// ���� ���� �������� ���̵� �����´�.
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		Debug.Log(PROTOCOL.START_LOADING.ToString());

		switch (protocol_id)
		{
			case PROTOCOL.START_LOADING:
				{
					byte player_index = msg.pop_byte();
					this.battle_room.gameObject.SetActive(true);
					this.battle_room.start_loading(player_index);
					this.gameObject.SetActive(false);
				}
				break;
		}
	}

	private void Cancel_Matching()
    {
		network_manager.Disconnect();
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Backspace)) Cancel_Matching();

		if (Application.platform == RuntimePlatform.Android)
			if (Input.GetKey(KeyCode.Escape)) Cancel_Matching();
	}
}
