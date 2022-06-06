using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO;

public class Scene_GamePlay : MonoBehaviour
{
    public void Awake()
    {

        // TODO: theme ����
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
