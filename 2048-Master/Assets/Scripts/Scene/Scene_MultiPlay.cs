using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Scene_MultiPlay : MonoBehaviour
{
    public void Awake()
    {
        // Theme Road
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_MultiPlay");
        GameObject.Find("Button_Back").GetComponent<Image>().sprite = Theme.GetImage("Button_Back");
        GameObject.Find("Button_Home").GetComponent<Image>().sprite = Theme.GetImage("Button_Home");
        GameObject.Find("Button_Matching").GetComponent<Image>().sprite = Theme.GetImage("Button_Matching");
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
