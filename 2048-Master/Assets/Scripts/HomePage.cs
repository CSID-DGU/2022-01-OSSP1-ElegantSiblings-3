using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO;

public class HomePage : MonoBehaviour
{
    public void AttendanceRewardButton()
    {
        //SceneManager.LoadScene("GamePlayPage");
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene("GamePlayPage");
    }

    public void ProfileButton()
    {
        //SceneManager.LoadScene("HomePage");
    }

    public void StoreButton()
    {
        //SceneManager.LoadScene("SinglePlayPage");
    }
}
