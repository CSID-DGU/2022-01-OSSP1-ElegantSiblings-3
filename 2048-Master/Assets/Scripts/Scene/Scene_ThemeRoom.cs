using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class Scene_ThemeRoom : MonoBehaviour
{
    private void Awake()
    {   

    }

    public void Button_Back_Click()
    {
        SceneManager.LoadScene("Scene_Home");
    }

    public void Button_Home_Click()
    {
        SceneManager.LoadScene("Scene_Home");
    }



    public void Button_theme0()
    {
        Theme.SetTheme(THEME_LIST.THEME0);
    }

    public void Button_theme1()
    {
        Theme.SetTheme(THEME_LIST.THEME1);
    }

}
