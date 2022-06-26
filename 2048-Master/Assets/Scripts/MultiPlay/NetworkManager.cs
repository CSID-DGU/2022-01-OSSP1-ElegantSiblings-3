using GameNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkManager : MonoBehaviour
{
	UnityGameNetworkService gameServer;
	string receivedMsg;

	public MonoBehaviour messageReceiver;

	void Awake()
	{
		this.receivedMsg = "";

		// ��Ʈ��ũ ����� ���� CFreeNetUnityService��ü �߰�
		this.gameServer = gameObject.AddComponent<UnityGameNetworkService>();

		// ���� ��ȭ(����, �����)�� �뺸 ���� ��������Ʈ
		this.gameServer.appcallbackOnStatusChanged += OnStatusChanged;

		// ��Ŷ ���� ��������Ʈ
		this.gameServer.appcallbackOnMessage += OnMessage;
	}


	public void Connect()
	{
		// ���� ��ǻ���� �ּҿ� ��Ʈ ��ȣ�� �Է� (���� ��ǥ ���� �ּҿ� ��Ʈ�� �����ϰ� �ƹ� ���� ����)
		this.gameServer.Connect("123.456.789.12", 1234);
	}

	public bool IsConnected()
	{
		return this.gameServer.IsConnected();
	}

	public void Disconnect()
    {
		this.gameServer.Disconnect();
		SceneManager.LoadScene("Scene_MultiPlay");
	}

	/// <summary>
	/// ��Ʈ��ũ ���� ����� ȣ��Ǵ� �ݹ� �żҵ�
	/// </summary>
	void OnStatusChanged(NETWORK_EVENT status)
	{
		switch (status)
		{
			// ���� ����
			case NETWORK_EVENT.connected:
				{
					LogManager.log("on connected");
					this.receivedMsg += "on connected\n";
					GameObject.Find("MatchingManager").GetComponent<MatchingManager>().OnConnected();
				}
				break;

			// ���� ����
			case NETWORK_EVENT.disconnected:
				LogManager.log("disconnected");
				this.receivedMsg += "disconnected\n";
				break;
		}
	}

	void OnMessage(Packet msg)
	{
		this.messageReceiver.SendMessage("OnRecv", msg);
	}

	public void Send(Packet msg)
	{
		this.gameServer.send(msg);
	}
}
