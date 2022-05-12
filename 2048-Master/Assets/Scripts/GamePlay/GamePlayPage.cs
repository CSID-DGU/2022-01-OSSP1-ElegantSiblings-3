using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO;

public class GamePlayPage : MonoBehaviour
{
    public void ReturnPrevPageButton()
    {
        SceneManager.LoadScene("HomePage");
    }

    public void ReturnHomePageButton()
    {
        SceneManager.LoadScene("HomePage");
    }

    public void SinglePlayButton()
    {
        SceneManager.LoadScene("SinglePlayPage");
    }

    public void CompetitionPlayButton()
    {
        //SceneManager.LoadScene("Game");
    }
}
