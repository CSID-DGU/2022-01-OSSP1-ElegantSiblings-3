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
        // TODO: theme ¼³Á¤

        Player player = PlayerManager.LoadTempPlayerData();
        GameObject.Find("Text_NickName").GetComponent<TextMeshProUGUI>().text = player.nickName;
        GameObject.Find("Text_Level").GetComponent<TextMeshProUGUI>().text = ((int.Parse(player.exp) / 5) + 1).ToString();
        GameObject.Find("Text_HighestScore").GetComponent<TextMeshProUGUI>().text = player.highestScore;
        GameObject.Find("Text_HighestBlock").GetComponent<TextMeshProUGUI>().text = player.highestBlock;
        GameObject.Find("Text_Games").GetComponent<TextMeshProUGUI>().text = player.games;
        GameObject.Find("Text_Win").GetComponent<TextMeshProUGUI>().text = player.win;
        GameObject.Find("Text_Lose").GetComponent<TextMeshProUGUI>().text = player.lose;
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
