using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public class CFreeNetUnityService : MonoBehaviour   // Network Library 와 Unity Project 연결
{
    CFreeNetEventManager event_manager;
    IPeer gameserver;                   // 연결된 GameServer 객체
    CNetworkService service;            // TCP 통신을 위한 서비스 객체


    // 네트워크 상태 변경 시 호출. 앱에서 콜백 메서드를 설정
    public delegate void StatusChangedHandler(NETWORK_EVENT status);
    public StatusChangedHandler appcallback_on_status_changed;


    // 네트워크 메시지 수신 시 호출. 앱에서 콜백 메서드를 설정
    public delegate void MessageHandler(CPacket msg);
    public MessageHandler appcallback_on_message;


    private void Awake()
    {
        CPacketBufferManager.initialize(2000);
        this.event_manager = new CFreeNetEventManager();
    }


    public void connect(string host, int port)
    {
        // 메시지의 비동기 송/수신 처리
        this.service = new CNetworkService();    

        // endpoint를 가지고 있는 Connector를 생성하고, NetworkService를 넣어둠
        CConnector connector = new CConnector(service);

        // 접속 성공 시 호출될 콜백 메서드
        connector.connected_callback += on_connected_gameserver;
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        connector.connect(endpoint);
    }


    private void on_connected_gameserver(CUserToken server_token)
    {
        this.gameserver = new CRemoteServerPeer(server_token);
        ((CRemoteServerPeer)this.gameserver).set_eventmanager(this.event_manager);

        // 유니티 어플리케이션으로 이벤트를 넘겨주기 위해서 매니저에 큐잉
        this.event_manager.enqueue_network_event(NETWORK_EVENT.connected);
    }


    void Update()
    {
        // 수신된 메시지에 대한 콜백.
        if (this.event_manager.has_message())
        {
            CPacket msg = this.event_manager.dequeue_network_message();
            if (this.appcallback_on_message != null)
            {
                this.appcallback_on_message(msg);
            }
        }

        // 네트워크 발생 이벤트에 대한 콜백.
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


    void OnApplicationQuit()
    {
        if (this.gameserver != null)
        {
            ((CRemoteServerPeer)this.gameserver).token.disconnect();
        }
    }
}
