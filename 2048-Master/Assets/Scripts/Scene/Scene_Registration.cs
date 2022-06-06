using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using TMPro;
using UnityEngine.SceneManagement;


public class Scene_Registration : MonoBehaviour
{
    public static void Button_Confirm_Click()
    {
        string id = GameObject.Find("InputField_UserID").GetComponent<TMP_InputField>().text;
        string pw = GameObject.Find("InputField_UserPW").GetComponent<TMP_InputField>().text;
        string nk = GameObject.Find("InputField_Nickname").GetComponent<TMP_InputField>().text;

        if (id == "" || pw == "" || nk == "")
        {
            Debug.Log("��ĭ�� �ֽ��ϴ�!");
            return;
        }

        var dataTable = DatabaseManager.Select(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("id", id) });

        if (dataTable.Rows.Count == 0)
        {
            var check = DatabaseManager.Insert(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id",id),
                new KeyValuePair<string, string>("password", pw),
                new KeyValuePair<string, string>("nickname", nk)
            });

            if (!check)
            {
                Debug.Log("���� �����߽��ϴ�. �ٽ� �Է����ּ���.");
                GameObject.Find("InputField_UserID").GetComponent<TMP_InputField>().text = "";
                GameObject.Find("InputField_UserPW").GetComponent<TMP_InputField>().text = "";
                GameObject.Find("InputField_Nickname").GetComponent<TMP_InputField>().text = "";
            }
            else
            {
                Debug.Log("������ �Ϸ�Ǿ����ϴ�.");
                SceneManager.LoadScene("Scene_Login");
            }
        }
        else
        {
            Debug.Log("�ش� ���̵� �̹� �����մϴ�.");
            GameObject.Find("InputField_UserID").GetComponent<TMP_InputField>().text = "";
        }
    }

}
