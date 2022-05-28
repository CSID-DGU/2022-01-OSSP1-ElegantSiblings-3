using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public class CNetworkManager : MonoBehaviour
{
	CFreeNetUnityService gameserver;


	private void Awake()
	{
		// 네트워크 통신을 위한 객체
		this.gameserver = gameObject.AddComponent<CFreeNetUnityService>();

		// 상태 변화(접속, 끊김등)를 통보 받을 델리게이트 설정
		this.gameserver.appcallback_on_status_changed += on_status_changed;

		// 패킷 수신 델리게이트 설정
		this.gameserver.appcallback_on_message += on_message;
	}

	private void Start()
    {
		connect();
	}


	public void connect()
	{
		this.gameserver.connect("127.0.0.1", 7979);
	}


	void on_status_changed(NETWORK_EVENT status)
	{
		switch (status)
		{
			// 접속 성공.
			case NETWORK_EVENT.connected:
				{
					Debug.Log("on connected");

					CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
					msg.push("Hello!!!!");
					this.gameserver.send(msg);
				}
				break;

			// 연결 끊김.
			case NETWORK_EVENT.disconnected:
				Debug.Log("disconnected");
				break;
		}
	}

	void on_message(CPacket msg)
	{
		// protocol id를 꺼냄
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		switch (protocol_id)
        {
			case PROTOCOL.CHAT_MSG_ACK:
                {
					string text = msg.pop_string();
					GameObject.Find("GameMain").GetComponent<CGameMain>().on_receive_chat_msg(text);
                }
				break;
        }
	}

	public void send(CPacket msg)
	{
		this.gameserver.send(msg);
	}
}
