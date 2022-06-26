using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Scene_Registration : MonoBehaviour
{
    TMP_InputField inputField_ID;
    TMP_InputField inputField_PW;
    TMP_InputField inputField_NK;

    private void Awake()
    {
        GameObject.Find("BackGround").transform.Find("Messagebox_AlreadyExistsID").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_AlreadyExistsID").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Messagebox_ExistBlank").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_ExistBlank").gameObject.SetActive(false);
        inputField_ID = GameObject.Find("InputField_ID").GetComponent<TMP_InputField>();
        inputField_PW = GameObject.Find("InputField_PW").GetComponent<TMP_InputField>();
        inputField_NK = GameObject.Find("InputField_NK").GetComponent<TMP_InputField>();
    }

    public void Button_Confirm_Click()
    {
        if (inputField_ID.text == "" || inputField_PW.text == "" || inputField_NK.text == "")
        {
            //Debug.Log("��ĭ�� �ֽ��ϴ�!");
            GameObject.Find("BackGround").transform.Find("Messagebox_ExistBlank").gameObject.SetActive(true);
            GameObject.Find("BackGround").transform.Find("Button_ExistBlank").gameObject.SetActive(true);
            return;
        }

        var dataTable = DatabaseManager.Select(new List<DatabaseManager.ATTRIBUTE> { DatabaseManager.ATTRIBUTE.id }, inputField_ID.text);

        if (dataTable.Rows.Count == 0)
        {      
            var isInsert = DatabaseManager.Insert(new List<KeyValuePair<DatabaseManager.ATTRIBUTE, string>>
            {
                new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.id, inputField_ID.text),
                new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.password, inputField_PW.text),
                new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.nickname, inputField_NK.text)
            });

            if (isInsert)
            {
                //Debug.Log("������ �Ϸ�Ǿ����ϴ�.");
                SceneManager.LoadScene("Scene_Login");
            }
            else
            {
                //Debug.Log("���� �����߽��ϴ�. �ٽ� �Է����ּ���.");
                inputField_ID.text = "";
                inputField_PW.text = "";
                inputField_NK.text = "";
            }
        }
        else
        {
            GameObject.Find("BackGround").transform.Find("Messagebox_AlreadyExistsID").gameObject.SetActive(true);
            GameObject.Find("BackGround").transform.Find("Button_AlreadyExistsID").gameObject.SetActive(true);
            inputField_ID = GameObject.Find("InputField_ID").GetComponent<TMP_InputField>();
            inputField_ID.text = "";
        }
    }

    public void Button_Cancel_Click()
    {
        SceneManager.LoadScene("Scene_Login");
    }


    public void Button_AlreadyExistsID_Click()
    {
        GameObject.Find("BackGround").transform.Find("Messagebox_AlreadyExistsID").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_AlreadyExistsID").gameObject.SetActive(false);
    }

    public void Button_ExistBlank_Click()
    {
        GameObject.Find("BackGround").transform.Find("Messagebox_ExistBlank").gameObject.SetActive(false);
        GameObject.Find("BackGround").transform.Find("Button_ExistBlank").gameObject.SetActive(false);
    }
}
