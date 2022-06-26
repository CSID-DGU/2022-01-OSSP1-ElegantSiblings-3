using GameNetwork;
using System;


public class RemoteServerPeer : IPeer
{
	public UserToken token { get; private set; }
	WeakReference networkEventManager;

	public RemoteServerPeer(UserToken token)
	{
		this.token = token;
		this.token.SetPeer(this);
	}

	public void set_eventmanager(GameNetworkEventManager eventManager)
	{
		this.networkEventManager = new WeakReference(eventManager);
	}

	void IPeer.OnMessage(Const<byte[]> buffer)
	{
		// 버퍼를 복사한 뒤 CPacket클래스로 감싼 뒤 넘겨준다.
		// CPacket클래스 내부에서는 참조로만 들고 있는다.
		byte[] app_buffer = new byte[buffer.Value.Length];
		Array.Copy(buffer.Value, app_buffer, buffer.Value.Length);
		Packet msg = new Packet(app_buffer, this);
		(this.networkEventManager.Target as GameNetworkEventManager).EnqueueNetworkMessage(msg);
	}

	void IPeer.OnRemoved()
	{
		(this.networkEventManager.Target as GameNetworkEventManager).EnqueueNetworkEvent(NETWORK_EVENT.disconnected);
	}

	void IPeer.Send(Packet msg)
	{
		this.token.Send(msg);
	}

	void IPeer.Disconnect() { }

	void IPeer.ProcessUserOperation(GameNetwork.Packet msg) { }
}
