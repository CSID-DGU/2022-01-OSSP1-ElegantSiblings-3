using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Scene_Home : MonoBehaviour
{
    public void Awake()
    {
        // TODO: theme ����

        Player player = PlayerManager.LoadTempPlayerData();

        GameObject.Find("Text_NickName").GetComponent<TextMeshProUGUI>().text = player.nickName;
        GameObject.Find("Text_Level").GetComponent<TextMeshProUGUI>().text = ((player.exp / 5) + 1).ToString();
        GameObject.Find("Text_HighestScore").GetComponent<TextMeshProUGUI>().text = player.highestScore.ToString();
        GameObject.Find("Text_HighestBlock").GetComponent<TextMeshProUGUI>().text = player.highestBlock.ToString();
        GameObject.Find("Text_Games").GetComponent<TextMeshProUGUI>().text = player.games.ToString();
        GameObject.Find("Text_Win").GetComponent<TextMeshProUGUI>().text = player.win.ToString();
        GameObject.Find("Text_Lose").GetComponent<TextMeshProUGUI>().text = player.lose.ToString();
    }

    public void Button_LogOut()
    {
        PlayerManager.ClearTempPlayerData();
        SceneManager.LoadScene("Scene_Login");
    }
   
    public void Button_GamePlay_Click()
    {
        SceneManager.LoadScene("Scene_GamePlay");
    }

    public void Button_ThemeRoom()
    {

    }
}
