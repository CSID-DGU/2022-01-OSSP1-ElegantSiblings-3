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
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_Home");
        GameObject.Find("Button_GamePlay").GetComponent<Image>().sprite = Theme.GetImage("Button_GamePlay");
        GameObject.Find("Button_ThemeRoom").GetComponent<Image>().sprite = Theme.GetImage("Button_ThemeRoom");
        GameObject.Find("Button_LogOut").GetComponent<Image>().sprite = Theme.GetImage("Button_LogOut");
        GameObject.Find("Messagebox_LogOut").GetComponent<Image>().sprite = Theme.GetImage("Messagebox_LogOut");
        GameObject.Find("Button_LogOutYes").GetComponent<Image>().sprite = Theme.GetImage("Button_LogOutYes");
        GameObject.Find("Button_LogOutNo").GetComponent<Image>().sprite = Theme.GetImage("Button_LogOutNo");

        // TODO: theme
        GameObject.Find("BackGround").transform.Find("Messagebox_LogOut").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_LogOutYes").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_LogOutNo").gameObject.SetActive(false);

        PlayerManager.Instance.LoadPlayerData();

        GameObject.Find("Text_NickName").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.nickName;
        GameObject.Find("Text_Level").GetComponent<TextMeshProUGUI>().text = "Lv. " + ((PlayerManager.Instance.exp / 10) + 1).ToString();
        GameObject.Find("Text_HighestScore").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.highestScore.ToString();
        GameObject.Find("Text_HighestBlock").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.highestBlock.ToString();
        GameObject.Find("Text_Games").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.games.ToString();
        GameObject.Find("Text_Win").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.win.ToString();
        GameObject.Find("Text_Lose").GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.lose.ToString();
    }


    public void Button_LogOut_Click()
    {
        GameObject.Find("BackGround").transform.Find("Messagebox_LogOut").gameObject.SetActive(true);
        GameObject.Find("BackGround").transform.Find("Button_LogOutYes").gameObject.SetActive(true);
        GameObject.Find("BackGround").transform.Find("Button_LogOutNo").gameObject.SetActive(true);
    }

    public void Button_LogOutYes_Click()
    {
        SceneManager.LoadScene("Scene_Login");
    }

    public void Button_LogOutNo_Click()
    {
        GameObject.Find("BackGround").transform.Find("Messagebox_LogOut").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_LogOutYes").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_LogOutNo").gameObject.SetActive(false);
    }

    public void Button_GamePlay_Click()
    {
        SceneManager.LoadScene("Scene_GamePlay");
    }

    public void Button_ThemeRoom_Click()
    {
        SceneManager.LoadScene("Scene_ThemeRoom");
    }
}
