using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPage : MonoBehaviour
{
    public static LoginPage instance;

    public GameObject startMenu;
    public TMP_InputField userIDField;
    public TMP_InputField userPWField;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exsits, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        userIDField.interactable = false;
        userPWField.interactable = false;

        Client.instance.ConnectToServer();
    }
}
