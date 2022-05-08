using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using TMPro;

public class ResetBoard : MonoBehaviour
{
    public void Button_Reset()
    {
        GameObject.Find("Text_Reset").GetComponent<TextMeshProUGUI>().text = "True";
        SceneManager.LoadScene("Board");
    }
}
