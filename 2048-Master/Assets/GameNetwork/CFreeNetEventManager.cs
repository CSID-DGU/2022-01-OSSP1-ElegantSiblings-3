using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public enum NETWORK_EVENT : byte
{
	connected,
	disconnected,
	end
}


public class CFreeNetEventManager
{	
	object cs_event;						// ����ȭ ��ü
	Queue<NETWORK_EVENT> network_events;	// ��Ʈ��ũ �������� �߻��� �̺�Ʈ���� �����ϴ� ť
	Queue<CPacket> network_message_events;	// �������� ���� ��Ŷ���� �����س��� ť

	public CFreeNetEventManager()
	{
		this.network_events = new Queue<NETWORK_EVENT>();
		this.network_message_events = new Queue<CPacket>();
		this.cs_event = new object();
	}

	public void enqueue_network_event(NETWORK_EVENT event_type)
	{
		lock (this.cs_event)
		{
			this.network_events.Enqueue(event_type);
		}
	}

	public bool has_event()
	{
		lock (this.cs_event)
		{
			return this.network_events.Count > 0;
		}
	}

	public NETWORK_EVENT dequeue_network_event()
	{
		lock (this.cs_event)
		{
			return this.network_events.Dequeue();
		}
	}


	public bool has_message()
	{
		lock (this.cs_event)
		{
			return this.network_message_events.Count > 0;
		}
	}

	public void enqueue_network_message(CPacket buffer)
	{
		lock (this.cs_event)
		{
			this.network_message_events.Enqueue(buffer);
		}
	}

	public CPacket dequeue_network_message()
	{
		lock (this.cs_event)
		{
			return this.network_message_events.Dequeue();
		}
	}
}
