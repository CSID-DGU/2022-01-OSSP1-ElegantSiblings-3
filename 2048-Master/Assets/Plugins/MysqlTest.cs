using System;
using UnityEngine;
using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine.UI;
using System.Data;
using MySql.Data;

public class MysqlTest : MonoBehaviour
{
    private void Awake()
    {
        DataTable dt = Select();

        foreach (DataRow row in dt.Rows)
        {
            Console.WriteLine("{0}\t{1}", row[0], row[1]);
        }
    }

    public void Button_Login_Click()
    {
        string id = GameObject.Find("UserID_InputField").GetComponent<TMP_InputField>().text;
        string pw = GameObject.Find("UserPW_InputField").GetComponent<TMP_InputField>().text;




        Debug.Log("ID: " + id + Environment.NewLine + "PW: " + pw);

        DataTable dt = Select();

        foreach (DataRow row in dt.Rows)
        {
            GameObject.Find("UserID_InputField").GetComponent<TMP_InputField>().text = row[0].ToString();
            GameObject.Find("UserPW_InputField").GetComponent<TMP_InputField>().text = row[1].ToString();

        }
    }
    
    public void Insert()
    {
        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            string insertQuery = "INSERT INTO users(id,password) VALUES('18tester','1234')";
            try
            {
                MySqlCommand command = new MySqlCommand(insertQuery, mySqlConnection);

                if (command.ExecuteNonQuery() == 1)
                {
                    Debug.Log("Insert Success");
                }
                else
                {
                    Debug.Log("Insert Failure");
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

    public void Update()
    {
        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            string updateQuery = "UPDATE users SET password='2345' WHERE id='18tester'";
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

    public DataTable Select()
    {
        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));
        DataTable dt = new DataTable();

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            string selectQuery = "SELECT * FROM users";
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
}