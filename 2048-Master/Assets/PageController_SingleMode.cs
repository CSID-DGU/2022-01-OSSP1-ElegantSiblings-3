using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageController_SingleMode : MonoBehaviour
{
    public void Button_SingleMode()
    {
        SceneManager.LoadScene("SinglePlayPage");
    }
}
