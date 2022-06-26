using GameNetwork;
using System;
using System.Net;
using UnityEngine;


/// <summary>
/// GameNetwork�� ����Ƽ ���� �̾��ִ� Ŭ����
/// </summary>
public class UnityGameNetworkService : MonoBehaviour
{
	GameNetworkEventManager eventManager;

	// ����� ���� ���� ��ü
	IPeer gameServer;

	// TCP����� ���� ���� ��ü
	NetworkService service;

	// ���� �Ϸ�� ȣ��Ǵ� ��������Ʈ (�ۿ��� �ݹ� �żҵ带 �����Ͽ� ���)
	public delegate void StatusChangedHandler(NETWORK_EVENT status);
	public StatusChangedHandler appcallbackOnStatusChanged;

	// ��Ʈ��ũ �޽��� ���Ž� ȣ��Ǵ� ��������Ʈ (�ۿ��� �ݹ� �żҵ带 �����Ͽ� ���)
	public delegate void MessageHandler(Packet msg);
	public MessageHandler appcallbackOnMessage;

	void Awake()
	{
		PacketBufferManager.Initialize(10);
		this.eventManager = new GameNetworkEventManager();
	}

	public void Connect(string host, int port)
	{
		if (this.service != null)
		{
			Debug.LogError("Already connected.");
			return;
		}

		this.service = new NetworkService();

		Connector connector = new Connector(service);
		connector.connectedCallback += OnConnectedGameserver;
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
		connector.Connect(endpoint);
	}


	public bool IsConnected()
	{
		return this.gameServer != null;
	}

	public void Disconnect()
	{
		if (this.gameServer != null)
		{
			((RemoteServerPeer)this.gameServer).token.Disconnect();
		}
	}


	/// <summary>
	/// ���� ������ ȣ��Ǵ� �ݹ� �żҵ�
	/// </summary>
	void OnConnectedGameserver(UserToken server_token)
	{
		this.gameServer = new RemoteServerPeer(server_token);
		((RemoteServerPeer)this.gameServer).set_eventmanager(this.eventManager);

		// ����Ƽ ���ø����̼����� �̺�Ʈ�� �Ѱ��ֱ� ���ؼ� �Ŵ����� ť�� ���� �ش�.
		this.eventManager.EnqueueNetworkEvent(NETWORK_EVENT.connected);
	}

	/// <summary>
	/// ��Ʈ��ũ���� �߻��ϴ� ��� �̺�Ʈ�� Ŭ���̾�Ʈ���� �˷��ִ� �żҵ�
	/// </summary>
	void Update()
	{
		// ���ŵ� �޽����� ���� �ݹ�.
		if (this.eventManager.ExistMessage())
		{
			Packet msg = this.eventManager.DequeueNetworkMessage();
			if (this.appcallbackOnMessage != null)
			{
				this.appcallbackOnMessage(msg);
			}
		}

		// ��Ʈ��ũ �߻� �̺�Ʈ�� ���� �ݹ�.
		if (this.eventManager.ExistEvent())
		{
			NETWORK_EVENT status = this.eventManager.DequeueNetworkEvent();
			if (this.appcallbackOnStatusChanged != null)
			{
				this.appcallbackOnStatusChanged(status);
			}
		}
	}

	public void send(Packet msg)
	{
		try
		{
			this.gameServer.Send(msg);
			Packet.Destroy(msg);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	/// <summary>
	/// ������������ ����� OnApplicationQuit�żҵ忡�� Disconnect�� ȣ��
	/// </summary>
	void OnApplicationQuit()
	{
		if (this.gameServer != null)
		{
			((RemoteServerPeer)this.gameServer).token.Disconnect();
		}
	}
}
