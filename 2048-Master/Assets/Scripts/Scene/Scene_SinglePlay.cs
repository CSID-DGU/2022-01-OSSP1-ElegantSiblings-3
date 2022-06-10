using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;


public class Scene_SinglePlay : MonoBehaviour
{
    public void Awake()
    {
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_SinglePlay");
    }

    public void Button_Back_Click()
    {
        Destroy(GameObject.Find("SingleGameModeManager"));
        SceneManager.LoadScene("Scene_GamePlay");
    }

    public void Button_Home_Click()
    {
        Destroy(GameObject.Find("SingleGameModeManager"));
        SceneManager.LoadScene("Scene_Home");
    }

    public void Button_ClassicMode_Click()
    {
        SingleGameModeManager.Instance.SetMode(SingleGameModeManager.MODE.CLASSIC);    
        SceneManager.LoadScene("Scene_SingleGame");
    }

    public void Button_ChallengeMode_Click()
    {
        SingleGameModeManager.Instance.SetMode(SingleGameModeManager.MODE.CHALLENGE);
        SceneManager.LoadScene("Scene_SingleGame");
    }

    public void Button_PracticeMode_Click()
    {
        SingleGameModeManager.Instance.SetMode(SingleGameModeManager.MODE.PRACTICE);
        SceneManager.LoadScene("Scene_SingleGame");
    }
}
