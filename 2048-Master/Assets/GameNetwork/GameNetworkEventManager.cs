using GameNetwork;
using System.Collections.Generic;


public enum NETWORK_EVENT : byte
{
	connected,
	disconnected,
	end
}


public class GameNetworkEventManager
{	
	object serverEvent;						// ����ȭ ��ü
	Queue<NETWORK_EVENT> networkEvents;		// ��Ʈ��ũ �������� �߻��� �̺�Ʈ���� �����ϴ� ť
	Queue<Packet> networkMessageEvents;		// �������� ���� ��Ŷ���� �����س��� ť

	public GameNetworkEventManager()
	{
		this.networkEvents = new Queue<NETWORK_EVENT>();
		this.networkMessageEvents = new Queue<Packet>();
		this.serverEvent = new object();
	}



	// ��Ʈ��ũ �̺�Ʈ Enqueue, Dequeue
	public void EnqueueNetworkEvent(NETWORK_EVENT networkEvent)
	{
		lock (this.serverEvent)
		{
			this.networkEvents.Enqueue(networkEvent);
		}
	}

	public bool ExistEvent()
	{
		lock (this.serverEvent)
		{
			return this.networkEvents.Count > 0;
		}
	}

	public NETWORK_EVENT DequeueNetworkEvent()
	{
		lock (this.serverEvent)
		{
			return this.networkEvents.Dequeue();
		}
	}



	// ��Ʈ��ũ �޽��� Enqueue, Dequeue
	public bool ExistMessage()
	{
		lock (this.serverEvent)
		{
			return this.networkMessageEvents.Count > 0;
		}
	}

	public void EnqueueNetworkMessage(Packet buffer)
	{
		lock (this.serverEvent)
		{
			this.networkMessageEvents.Enqueue(buffer);
		}
	}

	public Packet DequeueNetworkMessage()
	{
		lock (this.serverEvent)
		{
			return this.networkMessageEvents.Dequeue();
		}
	}
}
