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


public class SinglePlayPage : MonoBehaviour
{
    public void Awake()
    {
     //   DataClear();
    }

    public void ClassicModeButton()
    {
        string path = Path.Combine(Application.persistentDataPath, "SinglePlayMode.json");
        File.WriteAllText(path, new SinglePlayMode { modeName = "ClassicMode" }.GetJson());
        SceneManager.LoadScene("Game");
    }

    public void InfinityModeButton()
    {
        string path = Path.Combine(Application.persistentDataPath, "SinglePlayMode.json");
        File.WriteAllText(path, new SinglePlayMode { modeName = "InfinityMode" }.GetJson());
        SceneManager.LoadScene("Game");
    }

    public void PracticeModeButton()
    {
        string path = Path.Combine(Application.persistentDataPath, "SinglePlayMode.json");
        File.WriteAllText(path, new SinglePlayMode { modeName = "PracticeMode" }.GetJson());
        SceneManager.LoadScene("Game");
    }

    public void DataClear()
    {
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "ClassicMode" + "GameData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "ClassicMode" + "StateData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "InfinityMode" + "GameData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "InfinityMode" + "StateData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "PracticeMode" + "GameData.json"), null);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "PracticeMode" + "StateData.json"), null);
    }
}
