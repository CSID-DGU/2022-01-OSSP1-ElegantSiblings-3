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

		// 네트워크 통신을 위해 CFreeNetUnityService객체 추가
		this.gameServer = gameObject.AddComponent<UnityGameNetworkService>();

		// 상태 변화(접속, 끊김등)를 통보 받을 델리게이트
		this.gameServer.appcallbackOnStatusChanged += OnStatusChanged;

		// 패킷 수신 델리게이트
		this.gameServer.appcallbackOnMessage += OnMessage;
	}


	public void Connect()
	{
		// 서버 컴퓨터의 주소와 포트 번호를 입력 (최종 발표 이후 주소와 포트는 제거하고 아무 값을 넣음)
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
	/// 네트워크 상태 변경시 호출되는 콜백 매소드
	/// </summary>
	void OnStatusChanged(NETWORK_EVENT status)
	{
		switch (status)
		{
			// 접속 성공
			case NETWORK_EVENT.connected:
				{
					LogManager.log("on connected");
					this.receivedMsg += "on connected\n";
					GameObject.Find("MatchingManager").GetComponent<MatchingManager>().OnConnected();
				}
				break;

			// 연결 끊김
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
