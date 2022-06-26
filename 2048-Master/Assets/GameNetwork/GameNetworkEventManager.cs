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
	object serverEvent;						// 동기화 객체
	Queue<NETWORK_EVENT> networkEvents;		// 네트워크 엔진에서 발생된 이벤트들을 보관하는 큐
	Queue<Packet> networkMessageEvents;		// 서버에서 받은 패킷들을 보관해놓는 큐

	public GameNetworkEventManager()
	{
		this.networkEvents = new Queue<NETWORK_EVENT>();
		this.networkMessageEvents = new Queue<Packet>();
		this.serverEvent = new object();
	}



	// 네트워크 이벤트 Enqueue, Dequeue
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



	// 네트워크 메시지 Enqueue, Dequeue
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
