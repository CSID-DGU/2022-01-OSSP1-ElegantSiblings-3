using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene_ThemeRoom : MonoBehaviour
{
    private void Awake()
    {
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_ThemeRoom");
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
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_ThemeRoom");
    }

    public void Button_theme1()
    {
        Theme.SetTheme(THEME_LIST.THEME1);
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_ThemeRoom");
    }

    public void Button_theme2()
    {
        Theme.SetTheme(THEME_LIST.THEME2);
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_ThemeRoom");
    }

    public void Button_theme3()
    {
        Theme.SetTheme(THEME_LIST.THEME3);
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_ThemeRoom");
    }

    public void Button_theme4() // �Ľ��� �׸�
    {
        Theme.SetTheme(THEME_LIST.THEME4);
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_ThemeRoom");
    }

}
