using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageController_PracticePlay : MonoBehaviour
{
    public void Button_PracticeMode()
    {
        SceneManager.LoadScene("PracticeGame");
    }

}

