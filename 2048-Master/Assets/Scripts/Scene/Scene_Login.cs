using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine.UI;
using System.Data;
using MySql.Data;
using UnityEngine.SceneManagement;


public class Scene_Login : MonoBehaviour
{
    TMP_InputField inputField_UserID;
    TMP_InputField inputField_UserPW;

    private void Awake()
    {
        inputField_UserID = GameObject.Find("InputField_UserID").GetComponent<TMP_InputField>();
        inputField_UserPW = GameObject.Find("InputField_UserPW").GetComponent<TMP_InputField>();
    }



    // ----------------------- Button Click Event -------------------------//
    public void Button_Login_Click()
    {
        if (inputField_UserID.text == "" || inputField_UserPW.text == "")
        {   
            // TODO: Messagebox
            Debug.Log("��ĭ�� �ֽ��ϴ�.");
        }
        else
        {
            DataTable dataTable = DatabaseManager.Select(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", inputField_UserID.text),
                new KeyValuePair<string, string>("password", inputField_UserPW.text)
            });

            if (dataTable.Rows.Count == 0)
            {
                // TODO: Messagebox
                Debug.Log("������ ���������ʽ��ϴ�");
                inputField_UserID.text = "";
                inputField_UserPW.text = "";
            }
            else
            {
                // TODO: Messagebox
                Debug.Log("�α��� �Ǿ����ϴ�");
                PlayerManager.Instance.Initialize(dataTable.Rows[0][(int)DatabaseManager.ATTRIBUTE.ID].ToString());
                SceneManager.LoadScene("Scene_Home");
            }
        }
    }

    public void Button_Register_Click()
    {
        SceneManager.LoadScene("Scene_Registration");
    }
}










/*

    public static bool Insert(string insertQuery)
    {
        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));
        bool check = true;

       // "INSERT INTO users(id,password,nickname) VALUES('" + id + "','" + pw"','" + nickname + "')"

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            //string insertQuery = "INSERT INTO users(id,password) VALUES('18tester','1234')";
            try
            {
                MySqlCommand command = new MySqlCommand(insertQuery, mySqlConnection);

                if (command.ExecuteNonQuery() == 1)
                {
                    Debug.Log("Insert Success");
                    check = true;
                }
                else
                {
                    Debug.Log("Insert Failure");
                    check = false;
                }

            }
            catch (Exception ex)
            {
                Debug.Log("Connect Failure");
                Debug.Log(ex.ToString());
            }

            mySqlConnection.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return check;
    }

    public static void Update(string updateQuery)
    {
        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));



        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            //string updateQuery = "UPDATE users SET password='2345' WHERE id='18tester'";
            try
            {
                MySqlCommand command = new MySqlCommand(updateQuery, mySqlConnection);

                if (command.ExecuteNonQuery() == 1)
                {
                    Debug.Log("Update Success");
                }
                else
                {
                    Debug.Log("Update Failure");
                }

            }
            catch (Exception ex)
            {
                Debug.Log("Connect Failure");
                Debug.Log(ex.ToString());
            }

            mySqlConnection.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    public static DataTable Select(string selectQuery)
    {
        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));
        DataTable dt = new DataTable();

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            //string selectQuery = "SELECT * FROM users";
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, mySqlConnection);
                adapter.Fill(dt);

            }
            catch (Exception ex)
            {
                Debug.Log("Connect Failure");
                Debug.Log(ex.ToString());
            }

            mySqlConnection.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return dt;
    }
}*/