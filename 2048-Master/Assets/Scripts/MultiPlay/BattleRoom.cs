using GameNetwork;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BattleRoom : MonoBehaviour
{
	enum GAME_STATE
	{
		READY = 0,
		STARTED
	}

	NetworkManager networkManager;		// 데이터 송,수신을 위한 네트워크 매니저
	GAME_STATE gameState;				// 게임 상태를 나타냄

	byte playerIndex;					// 플레이어 번호(인덱스)
	bool isGameFinished;				// 게임이 종료되었는지 확인하는 변수

	Board_Player board_player;			// 플레이어 게임 보드
	Board_Rival board_rival;            // 상대방 게임 보드

	private void Awake()
    {
		// Theme Road
		GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("GameBoard_PVP2048");
		GameObject.Find("Button_GiveUp").GetComponent<Image>().sprite = Theme.GetImage("Button_GiveUp");

		// Initialize
		GameObject.Find("BackGround").transform.Find("Messagebox_Result").gameObject.SetActive(false);
		GameObject.Find("BackGround").transform.Find("Messagebox_Start").gameObject.SetActive(false);
		GameObject.Find("Text_RivalNickName").GetComponent<TextMeshProUGUI>().text = "";
		GameObject.Find("Text_PlayerNickName").GetComponent<TextMeshProUGUI>().text = "";

		this.networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.gameState = GAME_STATE.READY;

		this.board_player = GameObject.Find("NodeBoard_1P").GetComponent<Board_Player>();
		this.board_rival = GameObject.Find("NodeBoard_2P").GetComponent<Board_Rival>();
	}


	public void StartLoading(byte player_me_index)
    {
		this.isGameFinished = false;

		this.networkManager.messageReceiver = this;
		this.playerIndex = player_me_index;

		Packet msg = Packet.Create((short)PROTOCOL.LOADING_COMPLETED);
		this.networkManager.Send(msg);
	}


	public void Disconnect()
    {
		networkManager.Disconnect();
    }

	/// <summary>
	/// Server로 패킷 전송
	/// </summary>
	public void OnSend(Packet msg)
	{
		// 게임이 시작된 상태에서만 게임 이벤트 발생
		if (this.gameState == GAME_STATE.STARTED)
		{
			this.networkManager.Send(msg);
		}
	}


	/// <summary>
	/// Server로부터 패킷 수신
	/// </summary>
	void OnRecv(Packet msg)
	{
		PROTOCOL protocol_id = (PROTOCOL)msg.PopProtocol_ID();

		switch (protocol_id)
		{
			case PROTOCOL.GAME_START:
				ProcessPT_GameStart(msg);
				break;

			case PROTOCOL.EXCHANGE_NICKNAME:
				ProcessPT_ExchangeNickName(msg);
				break;

			case PROTOCOL.MOVED_NODE:
				ProcessPT_MovedNode(msg);
				break;

			case PROTOCOL.CREATED_NEW_NODE:
				ProcessPT_CreatedNewNode(msg);
				break;

			case PROTOCOL.GAME_OVER:
				ProcessPT_GameOver(msg);
				break;
		}
	}


	private void ProcessPT_GameStart(Packet msg)  // 게임 시작 
	{
		StartCoroutine(GameStartEvent());

		this.gameState = GAME_STATE.STARTED;
		board_player.OnGameStart();

		Packet nickName = Packet.Create((short)PROTOCOL.EXCHANGE_NICKNAME);
		nickName.Push(PlayerManager.Instance.nickName);
		OnSend(nickName);
	}

	private IEnumerator GameStartEvent()
	{
		WaitForSeconds wait = new WaitForSeconds(1f);

		GameObject.Find("BackGround").transform.Find("Messagebox_Start").gameObject.SetActive(true);
		for (int i = 3; i > 0; i--)
		{
			GameObject.Find("Messagebox_Start").GetComponent<Image>().sprite = Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Start" + i.ToString() + "_Theme3");
			yield return wait;
		}
		GameObject.Find("BackGround").transform.Find("Messagebox_Start").gameObject.SetActive(false);

		board_player.gameStart = true;
	}


	private void ProcessPT_ExchangeNickName(Packet msg)
    {
        string rivalNickName = msg.PopString();
		GameObject.Find("Text_RivalNickName").GetComponent<TextMeshProUGUI>().text = rivalNickName;
		GameObject.Find("Text_PlayerNickName").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.nickName;
	}


    private void ProcessPT_MovedNode(Packet msg)  
	{
		board_rival.recvGameEvent.Receive_MovedDirection(msg);
	}


	private void ProcessPT_CreatedNewNode(Packet msg) 
	{
		board_rival.recvGameEvent.Receive_CreatedNodeLocation(msg);
	}


	private void ProcessPT_GameOver(Packet msg)  // 게임 결과 및 종료 (1:Win, 2:Lose, 3:Draw)
	{
		int result = msg.PopInt32();

		if (result != 0)
		{
			var player = PlayerManager.Instance;
			var query = new List<KeyValuePair<DatabaseManager.ATTRIBUTE, string>> { new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.games, (player.games + 1).ToString()) };

			if (result == 1)
			{
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.win, (player.win + 1).ToString()));
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.exp, (player.exp + 2).ToString()));
			}
			else if (result == 2)
			{
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.lose, (player.lose + 1).ToString()));
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.exp, (player.exp + 1).ToString()));
			}
			else if(result == 3)
            {
				query.Add(new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.exp, (player.exp + 1).ToString()));
			}

			DatabaseManager.Update(query, player.id);
			board_player.gameStart = false;
			StartCoroutine(GameOverEvent(result));
		}
	}

	private IEnumerator GameOverEvent(int game_result)
	{
		WaitForSeconds wait = new WaitForSeconds(1f);

		GameObject.Find("BackGround").transform.Find("Messagebox_Result").gameObject.SetActive(true);
		Sprite sprite = null;

		for (int i = 3; i > 0; i--)
		{
			if (game_result == 1)
			{
				sprite = Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Victory" + i.ToString() + "_Theme3");
			}
			else if (game_result == 2)
			{
				sprite = Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Defeated" + i.ToString() + "_Theme3");
			}
			else if (game_result == 3)
			{
				sprite = Resources.Load<Sprite>("theme3/Scene_GameRoom_Message_Draw" + i.ToString() + "_Theme3");
			}

			GameObject.Find("Messagebox_Result").GetComponent<Image>().sprite = sprite;
			yield return wait;
		}

		networkManager.Disconnect();
	}
}
