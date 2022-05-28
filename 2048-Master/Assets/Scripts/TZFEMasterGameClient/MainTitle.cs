using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public class MainTitle : MonoBehaviour
{
	enum USER_STATE
	{
		NOT_CONNECTED,
		CONNECTED,
		WAITING_MATCHING
	}

	Texture bg;
	BattleRoom battle_room;

	NetworkManager network_manager;
	USER_STATE user_state;

	Texture waiting_img;

	// Use this for initialization
	void Start()
	{
		this.user_state = USER_STATE.NOT_CONNECTED;
		//this.bg = Resources.Load("images/title_blue") as Texture;
		//this.battle_room = GameObject.Find("BattleRoom").GetComponent<BattleRoom>();
		//this.battle_room.gameObject.SetActive(false);

		this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

		//this.waiting_img = Resources.Load("images/waiting") as Texture;

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
				if (Input.GetMouseButtonDown(0))
				{
					this.user_state = USER_STATE.WAITING_MATCHING;

					CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
					this.network_manager.send(msg);

					StopCoroutine("after_connected");
				}
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

				//GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.bg);
				break;

			case USER_STATE.WAITING_MATCHING:
				//GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.bg);
				//GUI.DrawTexture(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 82),this.waiting_img);
				break;
		}
	}


	/// <summary>
	/// 서버에 접속이 완료되면 호출됨.
	/// </summary>
	public void on_connected()
	{
		this.user_state = USER_STATE.CONNECTED;

		StartCoroutine("after_connected");
	}


	/// <summary>
	/// 패킷을 수신 했을 때 호출
	/// </summary>
	public void on_recv(CPacket msg)
	{
		// 제일 먼저 프로토콜 아이디를 꺼내온다.
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		switch (protocol_id)
		{
			case PROTOCOL.START_LOADING:
				{
					byte player_index = msg.pop_byte();

					//this.battle_room.gameObject.SetActive(true);
					//this.battle_room.start_loading(player_index);
					gameObject.SetActive(false);
				}
				break;
		}
	}
}
