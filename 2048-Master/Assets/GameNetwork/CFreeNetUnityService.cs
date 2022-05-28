using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;

/// <summary>
/// FreeNet������ ����Ƽ ���ø����̼��� �̾��ִ� Ŭ����
/// </summary>

public class CFreeNetUnityService : MonoBehaviour
{
	CFreeNetEventManager event_manager;

	// ����� ���� ���� ��ü.
	IPeer gameserver;

	// TCP����� ���� ���� ��ü.
	CNetworkService service;

	// ���� �Ϸ�� ȣ��Ǵ� ��������Ʈ. ���ø����̼ǿ��� �ݹ� �żҵ带 �����Ͽ� ����Ѵ�.
	public delegate void StatusChangedHandler(NETWORK_EVENT status);
	public StatusChangedHandler appcallback_on_status_changed;

	// ��Ʈ��ũ �޽��� ���Ž� ȣ��Ǵ� ��������Ʈ. ���ø����̼ǿ��� �ݹ� �żҵ带 �����Ͽ� ����Ѵ�.
	public delegate void MessageHandler(CPacket msg);
	public MessageHandler appcallback_on_message;

	void Awake()
	{
		CPacketBufferManager.initialize(10);
		this.event_manager = new CFreeNetEventManager();
	}

	public void connect(string host, int port)
	{
		if (this.service != null)
		{
			Debug.LogError("Already connected.");
			return;
		}

		// CNetworkService��ü�� �޽����� �񵿱� ��,���� ó���� �����Ѵ�.
		this.service = new CNetworkService();

		// endpoint������ �����ִ� Connector����. ������ NetworkService��ü�� �־��ش�.
		CConnector connector = new CConnector(service);
		// ���� ������ ȣ��� �ݹ� �żҵ� ����.
		connector.connected_callback += on_connected_gameserver;
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
		connector.connect(endpoint);
	}


	public bool is_connected()
	{
		return this.gameserver != null;
	}


	/// <summary>
	/// ���� ������ ȣ��� �ݹ� �żҵ�.
	/// </summary>
	/// <param name="server_token"></param>
	void on_connected_gameserver(CUserToken server_token)
	{
		this.gameserver = new CRemoteServerPeer(server_token);
		((CRemoteServerPeer)this.gameserver).set_eventmanager(this.event_manager);

		// ����Ƽ ���ø����̼����� �̺�Ʈ�� �Ѱ��ֱ� ���ؼ� �Ŵ����� ť�� ���� �ش�.
		this.event_manager.enqueue_network_event(NETWORK_EVENT.connected);
	}

	/// <summary>
	/// ��Ʈ��ũ���� �߻��ϴ� ��� �̺�Ʈ�� Ŭ���̾�Ʈ���� �˷��ִ� ������ Update���� �����Ѵ�.
	/// FreeNet������ �޽��� �ۼ��� ó���� ��Ŀ�����忡�� ��������� ����Ƽ�� ���� ó���� ���� �����忡�� ����ǹǷ�
	/// ť��ó���� ���Ͽ� ���� �����忡�� ��� ���� ó���� �̷�������� �����Ͽ���.
	/// </summary>
	void Update()
	{
		// ���ŵ� �޽����� ���� �ݹ�.
		if (this.event_manager.has_message())
		{
			CPacket msg = this.event_manager.dequeue_network_message();
			if (this.appcallback_on_message != null)
			{
				this.appcallback_on_message(msg);
			}
		}

		// ��Ʈ��ũ �߻� �̺�Ʈ�� ���� �ݹ�.
		if (this.event_manager.has_event())
		{
			NETWORK_EVENT status = this.event_manager.dequeue_network_event();
			if (this.appcallback_on_status_changed != null)
			{
				this.appcallback_on_status_changed(status);
			}
		}
	}

	public void send(CPacket msg)
	{
		try
		{
			this.gameserver.send(msg);
			CPacket.destroy(msg);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	/// <summary>
	/// �������� ����ÿ��� OnApplicationQuit�żҵ忡�� disconnect�� ȣ���� ��� ����Ƽ�� hang���� �ʴ´�.
	/// </summary>
	void OnApplicationQuit()
	{
		if (this.gameserver != null)
		{
			((CRemoteServerPeer)this.gameserver).token.disconnect();
		}
	}
}