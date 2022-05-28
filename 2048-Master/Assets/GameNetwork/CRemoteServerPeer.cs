using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public class CRemoteServerPeer : IPeer
{
	public CUserToken token { get; private set; }
	WeakReference freenet_eventmanager;

	public CRemoteServerPeer(CUserToken token)
	{
		this.token = token;
		this.token.set_peer(this);
	}

	public void set_eventmanager(CFreeNetEventManager event_manager)
	{
		this.freenet_eventmanager = new WeakReference(event_manager);
	}

	void IPeer.on_message(Const<byte[]> buffer)
	{
		// ���۸� ������ �� CPacketŬ������ ���� �� �Ѱ��ش�.
		// CPacketŬ���� ���ο����� �����θ� ��� �ִ´�.
		byte[] app_buffer = new byte[buffer.Value.Length];
		Array.Copy(buffer.Value, app_buffer, buffer.Value.Length);
		CPacket msg = new CPacket(app_buffer, this);
		(this.freenet_eventmanager.Target as CFreeNetEventManager).enqueue_network_message(msg);
	}

	void IPeer.on_removed()
	{
		(this.freenet_eventmanager.Target as CFreeNetEventManager).enqueue_network_event(NETWORK_EVENT.disconnected);
	}

	void IPeer.send(CPacket msg)
	{
		this.token.send(msg);
	}

	void IPeer.disconnect()
	{
	}

	void IPeer.process_user_operation(CPacket msg)
	{
	}
}
