using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO;


[System.Serializable]
public class SinglePlayMode
{
    public string modeName = "";
    public string GetJson() => JsonUtility.ToJson(this, true);
}


public class Scene_SinglePlay : MonoBehaviour
{
    private string theme_name;

    public void Awake()
    {
        // TODO: theme ¼³Á¤
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
        string path = Path.Combine(Application.persistentDataPath, "SinglePlayMode.json");
        File.WriteAllText(path, new SinglePlayMode { modeName = "ClassicMode" }.GetJson());
        SceneManager.LoadScene("Game");
    }

    public void Button_ChallengeMode_Click()
    {
        string path = Path.Combine(Application.persistentDataPath, "SinglePlayMode.json");
        File.WriteAllText(path, new SinglePlayMode { modeName = "ChallengeMode" }.GetJson());
        SceneManager.LoadScene("Game");
    }

    public void Button_PracticeMode_Click()
    {
        string path = Path.Combine(Application.persistentDataPath, "SinglePlayMode.json");
        File.WriteAllText(path, new SinglePlayMode { modeName = "PracticeMode" }.GetJson());
        SceneManager.LoadScene("Game");
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
