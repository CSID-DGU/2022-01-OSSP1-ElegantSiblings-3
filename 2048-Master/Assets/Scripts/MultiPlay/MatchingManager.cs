using GameNetwork;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class MatchingManager : MonoBehaviour
{
	enum USER_STATE
	{
		NOT_CONNECTED,
		CONNECTED,
		WAITING_MATCHING
	}


	NetworkManager networkManager;
	USER_STATE userState;
	BattleRoom battleRoom;

	string themeName;
	Texture matchingBackground;
	List<Texture> waitingImage;
	int waitingCount;

	// Touch Event
	public Vector2 vectorS = new Vector2();
	public Vector2 vectorE = new Vector2();
	public Vector2 vectorM = new Vector2();


	// Use this for initialization
	void Start()
	{
		this.networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.userState = USER_STATE.NOT_CONNECTED;

		this.battleRoom = GameObject.Find("BattleRoom").GetComponent<BattleRoom>();
		this.battleRoom.gameObject.SetActive(false);

		themeName = "_Theme3";
		this.matchingBackground = Resources.Load("theme3/Scene_GameRoom_Background_Matching_APK" + themeName) as Texture;

		this.waitingImage = new List<Texture>
		{
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting0" + themeName) as Texture,
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting1" + themeName) as Texture,
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting2" + themeName) as Texture,
			Resources.Load("theme3/Scene_GameRoom_Message_Waiting3" + themeName) as Texture
		};

		this.waitingCount = 0;
		this.userState = USER_STATE.NOT_CONNECTED;
		Enter();
	}


	public void Enter()
	{
		StopCoroutine("AfterConnected");
			
		this.networkManager.messageReceiver = this;

		if (!this.networkManager.IsConnected())
		{
			this.userState = USER_STATE.CONNECTED;
			this.networkManager.Connect();
		}
		else
		{
			OnConnected();
		}
	}



	IEnumerator AfterConnected()
	{
		yield return new WaitForEndOfFrame();

		while (true)
		{
			if (this.userState == USER_STATE.CONNECTED)
			{
				this.userState = USER_STATE.WAITING_MATCHING;

				// 서버와 연결이 완료되었으면 게임룸 입장 요청
				Packet msg = Packet.Create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
				this.networkManager.Send(msg);
				StopCoroutine("AfterConnected");
			}

			yield return 0;
		}
	}


	void OnGUI()
	{
		switch (this.userState)
		{
			case USER_STATE.NOT_CONNECTED:
				break;

			case USER_STATE.CONNECTED:
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.matchingBackground);
				break;

			case USER_STATE.WAITING_MATCHING:				
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.matchingBackground);
				GUI.DrawTexture(new Rect(Screen.width / 2 - (Screen.width / 4.35f / 2), Screen.height / 2 - (Screen.height / 6f / 2), Screen.width / 4.35f, Screen.height / 6f), this.waitingImage[(waitingCount / 10) % 4]);				
				if (++waitingCount >= 2000) waitingCount = 0;
				Thread.Sleep(50);
				break;
		}
	}


	/// <summary>
	/// 서버에 접속이 완료되면 호출
	/// </summary>
	public void OnConnected()
	{
		this.userState = USER_STATE.CONNECTED;
		StartCoroutine("AfterConnected");
	}


	/// <summary>
	/// 패킷을 수신 했을 때 호출
	/// </summary>
	public void OnRecv(Packet msg)
	{
		// 제일 먼저 프로토콜 아이디를 꺼내온다.
		PROTOCOL protocol_id = (PROTOCOL)msg.PopProtocol_ID();

		switch (protocol_id)
		{
			case PROTOCOL.START_LOADING:
				{
					byte index = msg.PopByte();
					this.battleRoom.gameObject.SetActive(true);
					this.battleRoom.StartLoading(index);
					this.gameObject.SetActive(false);
				}
				break;
		}
	}

	private void CancelMatching()
    {
		networkManager.Disconnect();
	}

	private void Update()
	{
		SlideTouching();
		if (Input.GetKeyUp(KeyCode.Backspace)) CancelMatching();
		if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape)) CancelMatching();
	}

	private void SlideTouching()
    {
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Began)
			{
				vectorS = touch.position;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				vectorE = touch.position;
				vectorM = new Vector2(vectorE.x - vectorS.x, vectorE.y - vectorS.y);

				if (Mathf.Abs(vectorM.x) > Mathf.Abs(vectorM.y))
				{
					if (vectorM.x > (float)Screen.width / 4f)
					{
						CancelMatching();
					}
				}
			}
		}
	}
}
