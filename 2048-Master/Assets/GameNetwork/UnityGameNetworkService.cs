using GameNetwork;
using System;
using System.Net;
using UnityEngine;


/// <summary>
/// GameNetwork과 유니티 앱을 이어주는 클래스
/// </summary>
public class UnityGameNetworkService : MonoBehaviour
{
	GameNetworkEventManager eventManager;

	// 연결된 게임 서버 객체
	IPeer gameServer;

	// TCP통신을 위한 서비스 객체
	NetworkService service;

	// 접속 완료시 호출되는 델리게이트 (앱에서 콜백 매소드를 설정하여 사용)
	public delegate void StatusChangedHandler(NETWORK_EVENT status);
	public StatusChangedHandler appcallbackOnStatusChanged;

	// 네트워크 메시지 수신시 호출되는 델리게이트 (앱에서 콜백 매소드를 설정하여 사용)
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
	/// 접속 성공시 호출되는 콜백 매소드
	/// </summary>
	void OnConnectedGameserver(UserToken server_token)
	{
		this.gameServer = new RemoteServerPeer(server_token);
		((RemoteServerPeer)this.gameServer).set_eventmanager(this.eventManager);

		// 유니티 어플리케이션으로 이벤트를 넘겨주기 위해서 매니저에 큐잉 시켜 준다.
		this.eventManager.EnqueueNetworkEvent(NETWORK_EVENT.connected);
	}

	/// <summary>
	/// 네트워크에서 발생하는 모든 이벤트를 클라이언트에게 알려주는 매소드
	/// </summary>
	void Update()
	{
		// 수신된 메시지에 대한 콜백.
		if (this.eventManager.ExistMessage())
		{
			Packet msg = this.eventManager.DequeueNetworkMessage();
			if (this.appcallbackOnMessage != null)
			{
				this.appcallbackOnMessage(msg);
			}
		}

		// 네트워크 발생 이벤트에 대한 콜백.
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
	/// 정상적적으로 종료시 OnApplicationQuit매소드에서 Disconnect를 호출
	/// </summary>
	void OnApplicationQuit()
	{
		if (this.gameServer != null)
		{
			((RemoteServerPeer)this.gameServer).token.Disconnect();
		}
	}
}
