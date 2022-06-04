using System;
using UnityEngine;
using MySql.Data.MySqlClient;

public class MysqlTest : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("종강기원");
        Insert();
    }

    public void Insert()
    {
        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            string insertQuery = "INSERT INTO users(id,name,psword) VALUES('18tester','tester',1234)";
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
}