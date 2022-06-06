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
            Debug.Log("빈칸이 있습니다!");
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
                Debug.Log("가입 실패했습니다. 다시 입력해주세요.");
                GameObject.Find("InputField_UserID").GetComponent<TMP_InputField>().text = "";
                GameObject.Find("InputField_UserPW").GetComponent<TMP_InputField>().text = "";
                GameObject.Find("InputField_Nickname").GetComponent<TMP_InputField>().text = "";
            }
            else
            {
                Debug.Log("가입이 완료되었습니다.");
                SceneManager.LoadScene("Scene_Login");
            }
        }
        else
        {
            Debug.Log("해당 아이디가 이미 존재합니다.");
            GameObject.Find("InputField_UserID").GetComponent<TMP_InputField>().text = "";
        }
    }

}
