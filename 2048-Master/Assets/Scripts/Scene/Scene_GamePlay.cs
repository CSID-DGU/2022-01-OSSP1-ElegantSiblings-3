using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene_GamePlay : MonoBehaviour
{
    public void Awake()
    {
        // Theme Road
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_GamePlay");
        GameObject.Find("Button_Back").GetComponent<Image>().sprite = Theme.GetImage("Button_Back");
        GameObject.Find("Button_Home").GetComponent<Image>().sprite = Theme.GetImage("Button_Home");
        GameObject.Find("Button_SinglePlay").GetComponent<Image>().sprite = Theme.GetImage("Button_SinglePlay");
        GameObject.Find("Button_MultiPlay").GetComponent<Image>().sprite = Theme.GetImage("Button_MultiPlay");
    }

    public void Button_Back_Click()
    {
        SceneManager.LoadScene("Scene_Home");
    }

    public void Button_Home_Click()
    {
        SceneManager.LoadScene("Scene_Home");
    }

    public void Button_SinglePlay_Click()
    {
        SceneManager.LoadScene("Scene_SinglePlay");
    }

    public void Button_MultiPlay_Click()
    {
        SceneManager.LoadScene("Scene_MultiPlay");
    }
}
