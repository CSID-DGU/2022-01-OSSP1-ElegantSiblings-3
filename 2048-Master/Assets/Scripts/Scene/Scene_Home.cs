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

        GameObject.Find("Text_NickName").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.nickName;
        GameObject.Find("Text_Level").GetComponent<TextMeshProUGUI>().text = ((PlayerManager.Instance.exp / 5) + 1).ToString();
        GameObject.Find("Text_HighestScore").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.highestScore.ToString();
        GameObject.Find("Text_HighestBlock").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.highestBlock.ToString();
        GameObject.Find("Text_Games").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.games.ToString();
        GameObject.Find("Text_Win").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.win.ToString();
        GameObject.Find("Text_Lose").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.lose.ToString();
    }

    public void Button_LogOut()
    {
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
