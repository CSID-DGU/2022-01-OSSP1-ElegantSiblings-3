using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
	CFreeNetUnityService gameserver;
	string received_msg;

	public MonoBehaviour message_receiver;

	void Awake()
	{
		this.received_msg = "";

		// 네트워크 통신을 위해 CFreeNetUnityService객체 추가
		this.gameserver = gameObject.AddComponent<CFreeNetUnityService>();

		// 상태 변화(접속, 끊김등)를 통보 받을 델리게이트
		this.gameserver.appcallback_on_status_changed += on_status_changed;

		// 패킷 수신 델리게이트
		this.gameserver.appcallback_on_message += on_message;
	}


	public void connect()
	{
		this.gameserver.connect("220.116.117.78", 7979);
	}

	public bool is_connected()
	{
		return this.gameserver.is_connected();
	}

	public void Disconnect()
    {
		this.gameserver.Disconnect();
		SceneManager.LoadScene("Scene_MultiPlay");
	}

	/// <summary>
	/// 네트워크 상태 변경시 호출될 콜백 매소드.
	/// </summary>
	void on_status_changed(NETWORK_EVENT status)
	{
		switch (status)
		{
			// 접속 성공
			case NETWORK_EVENT.connected:
				{
					LogManager.log("on connected");
					this.received_msg += "on connected\n";
					GameObject.Find("MatchingManager").GetComponent<MatchingManager>().on_connected();
				}
				break;

			// 연결 끊김
			case NETWORK_EVENT.disconnected:
				LogManager.log("disconnected");
				this.received_msg += "disconnected\n";
				break;
		}
	}

	void on_message(CPacket msg)
	{
		this.message_receiver.SendMessage("on_recv", msg);
	}

	public void send(CPacket msg)
	{
		this.gameserver.send(msg);
	}
}
