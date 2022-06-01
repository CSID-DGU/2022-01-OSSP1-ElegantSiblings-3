using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.SceneManagement;
using System.IO;


public class Scene_MultiPlay : MonoBehaviour
{
    private string theme_name;

    public void Awake()
    {
        // TODO: theme ����
    }

    public void Button_Back_Click()
    {
        SceneManager.LoadScene("Scene_GamePlay");
    }

    public void Button_Home_Click()
    {
        SceneManager.LoadScene("Scene_Home");
    }

    public void Button_Matching_Click()
    {
        SceneManager.LoadScene("Scene_GameRoom");
    }
}
