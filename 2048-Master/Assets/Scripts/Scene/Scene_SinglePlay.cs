using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;


public enum SINGLE_MODE_LIST
{
    CLASSIC, CHALLENGE, PRACTICE
}

[System.Serializable]
public class SingleMode
{
    public SINGLE_MODE_LIST index = SINGLE_MODE_LIST.CLASSIC;
    public string name = "Classic";

    public static void Set_Mode(SINGLE_MODE_LIST m_name)
    {
        Json.Write(Path.Combine(Application.persistentDataPath, "SingleMode.json"),
            new SingleMode { index = m_name, name = new List<string> { "Classic", "Challenge", "Practice" }[(int)m_name] });
    }

    public static SingleMode Get_Mode()
    {
        SingleMode m = Json.Read<SingleMode>(Path.Combine(Application.persistentDataPath, "SingleMode.json"));
        return m == null ? new SingleMode() : m;
    }
}


public class Scene_SinglePlay : MonoBehaviour
{
    public void Awake()
    {
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.Get_Image("Scene_SinglePlay");
    }

    public void Button_Back_Click()
    {
        SceneManager.LoadScene("Scene_GamePlay");
    }

    public void Button_Home_Click()
    {
        SceneManager.LoadScene("Scene_Home");
    }

    public void Button_ClassicMode_Click()
    {
        SingleMode.Set_Mode(SINGLE_MODE_LIST.CLASSIC);
        SceneManager.LoadScene("Scene_SingleGame");
    }

    public void Button_ChallengeMode_Click()
    {
        SingleMode.Set_Mode(SINGLE_MODE_LIST.CHALLENGE);
        SceneManager.LoadScene("Scene_SingleGame");
    }

    public void Button_PracticeMode_Click()
    {
        SingleMode.Set_Mode(SINGLE_MODE_LIST.PRACTICE);
        SceneManager.LoadScene("Scene_SingleGame");
    }

    public void DataClear()
    {
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "ClassicMode" + "GameData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "ClassicMode" + "StateData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "ChallengeMode" + "GameData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "ChallengeMode" + "StateData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "PracticeMode" + "GameData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "PracticeMode" + "StateData.json"), null);
    }
}
