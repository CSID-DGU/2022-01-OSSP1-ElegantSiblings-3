using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using TMPro;
using UnityEngine.SceneManagement;


public class Scene_Registration : MonoBehaviour
{
    TMP_InputField inputField_ID;
    TMP_InputField inputField_PW;
    TMP_InputField inputField_NK;

    private void Awake()
    {
        inputField_ID = GameObject.Find("InputField_ID").GetComponent<TMP_InputField>();
        inputField_PW = GameObject.Find("InputField_PW").GetComponent<TMP_InputField>();
        inputField_NK = GameObject.Find("InputField_NK").GetComponent<TMP_InputField>();
    }

    public void Button_Confirm_Click()
    {
        if (inputField_ID.text == "" || inputField_PW.text == "" || inputField_NK.text == "")
        {
            Debug.Log("��ĭ�� �ֽ��ϴ�!");
            return;
        }

        var dataTable = DatabaseManager.Select(new List<KeyValuePair<string, string>>
        { 
            new KeyValuePair<string, string>(DatabaseManager.GetDBAttribute(DatabaseManager.ATTRIBUTE.ID), inputField_ID.text)
        });

        if (dataTable.Rows.Count == 0)
        {
            var isInsert = DatabaseManager.Insert(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(DatabaseManager.GetDBAttribute(DatabaseManager.ATTRIBUTE.ID),inputField_ID.text),
                new KeyValuePair<string, string>(DatabaseManager.GetDBAttribute(DatabaseManager.ATTRIBUTE.PASSWORD), inputField_PW.text),
                new KeyValuePair<string, string>(DatabaseManager.GetDBAttribute(DatabaseManager.ATTRIBUTE.NICKNAME), inputField_NK.text)
            });

            if (isInsert)
            {
                Debug.Log("������ �Ϸ�Ǿ����ϴ�.");
                SceneManager.LoadScene("Scene_Login");
            }
            else
            {
                Debug.Log("���� �����߽��ϴ�. �ٽ� �Է����ּ���.");
                inputField_ID.text = "";
                inputField_PW.text = "";
                inputField_NK.text = "";
            }
        }
        else
        {
            Debug.Log("�ش� ���̵� �̹� �����մϴ�.");
            inputField_ID.text = "";
        }
    }
}
