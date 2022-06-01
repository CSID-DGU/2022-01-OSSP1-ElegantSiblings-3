using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO;

public class Scene_Home : MonoBehaviour
{
    private string theme_name;

    public void Awake()
    {
        // TODO: theme ¼³Á¤
    }

    public void Button_GamePlay_Click()
    {
        SceneManager.LoadScene("Scene_GamePlay");
    }

    public void Button_ThemeRoom()
    {

    }
}
