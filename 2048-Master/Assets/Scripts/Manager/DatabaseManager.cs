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


public class DatabaseManager
{
    public static bool Insert(List<KeyValuePair<string, string>> inputList)
    {
        string insertQuery = "INSERT INTO";

        insertQuery += " users(";
        for (int i = 0; i < inputList.Count; i++)
        {
            if (i != 0) insertQuery += ",";
            insertQuery += inputList[i].Key;
        }
        insertQuery += ")";

        insertQuery += " VALUES(";
        for (int i = 0; i < inputList.Count; i++)
        {
            if (i != 0) insertQuery += ",";
            insertQuery += "'" + inputList[i].Value + "'";
        }
        insertQuery += ")";


        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            try
            {
                MySqlCommand command = new MySqlCommand(insertQuery, mySqlConnection);

                if (command.ExecuteNonQuery() == 1)
                {
                    Debug.Log("Insert Success");
                    return true;
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

        return false;
    }


    public static DataTable Select(List<KeyValuePair<string, string>> attributeList)
    {
        string selectQuery = "SELECT * FROM users WHERE ";
        for (int i = 0; i < attributeList.Count; i++)
        {
            if (i != 0) selectQuery += " AND ";
            selectQuery += attributeList[i].Key + "='" + attributeList[i].Value + "'";
        }

        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));
        DataTable dataTable = new DataTable();

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, mySqlConnection);
                adapter.Fill(dataTable);
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

        return dataTable;
    }


    public static void Update(List<KeyValuePair<string, string>> inputList, string id)
    {
        string updateQuery = "UPDATE users SET ";
        for (int i = 0; i < inputList.Count; i++)
        {
            if (i != 0) updateQuery += ", ";
            updateQuery += inputList[i].Key + "='" + inputList[i].Value + "'";
        }
        updateQuery += " WHERE id" + "='" + id + "'";


        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));

        try
        {
            mySqlConnection.Open();
            Debug.Log("Connected");

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
}
