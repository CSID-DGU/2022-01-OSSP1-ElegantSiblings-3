using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Scene_SinglePlay : MonoBehaviour
{
    public void Awake()
    {
        // Theme Road
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("Scene_SinglePlay");
        GameObject.Find("Button_Back").GetComponent<Image>().sprite = Theme.GetImage("Button_Back");
        GameObject.Find("Button_Home").GetComponent<Image>().sprite = Theme.GetImage("Button_Home");
        GameObject.Find("Button_Classic").GetComponent<Image>().sprite = Theme.GetImage("Button_ClassicMode");
        GameObject.Find("Button_Challenge").GetComponent<Image>().sprite = Theme.GetImage("Button_ChallengeMode");
        GameObject.Find("Button_Practice").GetComponent<Image>().sprite = Theme.GetImage("Button_PracticeMode");
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
