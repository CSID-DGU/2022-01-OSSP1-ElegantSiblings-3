using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;

public class NetworkManager : MonoBehaviour
{
	CFreeNetUnityService gameserver;
	string received_msg;

	public MonoBehaviour message_receiver;

	void Awake()
	{
		this.received_msg = "";

		// ��Ʈ��ũ ����� ���� CFreeNetUnityService��ü �߰�
		this.gameserver = gameObject.AddComponent<CFreeNetUnityService>();

		// ���� ��ȭ(����, �����)�� �뺸 ���� ��������Ʈ
		this.gameserver.appcallback_on_status_changed += on_status_changed;

		// ��Ŷ ���� ��������Ʈ
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

	/// <summary>
	/// ��Ʈ��ũ ���� ����� ȣ��� �ݹ� �żҵ�.
	/// </summary>
	void on_status_changed(NETWORK_EVENT status)
	{
		switch (status)
		{
			// ���� ����
			case NETWORK_EVENT.connected:
				{
					LogManager.log("on connected");
					this.received_msg += "on connected\n";
					GameObject.Find("MatchingManager").GetComponent<MatchingManager>().on_connected();
				}
				break;

			// ���� ����
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
