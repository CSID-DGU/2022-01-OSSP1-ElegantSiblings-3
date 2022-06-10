using System.Collections.Generic;
using UnityEngine;

public class SingleGameModeManager : MonoBehaviour
{
    public enum MODE
    {
        CLASSIC, CHALLENGE, PRACTICE
    }


    public static SingleGameModeManager Instance;

    private MODE mode;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        mode = MODE.CLASSIC;
    }

    public void SetMode(MODE mode)
    {
        this.mode = mode;
    }

    public MODE GetMode()
    {
        return mode;
    }

    public string GetModeName()
    {
        return new List<string> { "Classic", "Challenge", "Practice" }[(int)mode];
    }
}