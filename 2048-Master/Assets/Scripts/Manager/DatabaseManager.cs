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
    public enum ATTRIBUTE { id, password, in_date, nickname, highestscore, highestblock, games, win, lose, exp, saveclassicmode, savechallengemode, savepracticemode }

    public static bool Insert(List<KeyValuePair<ATTRIBUTE, string>> dataList)
    {
        string attributes = "";
        string values = "";

        foreach (var data in dataList)
        {
            attributes += (attributes.Length == 0 ? data.Key : ", " + data.Key);
            values += (values.Length == 0 ? "'" + data.Value + "'" : ", " + "'" + data.Value + "'");
        }

        string insertQuery = $"INSERT INTO users({attributes}) VALUES({values})";
        //Debug.Log(insertQuery);


        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));

        try
        {
            mySqlConnection.Open();
            //Debug.Log("Connected");

            try
            {
                MySqlCommand command = new MySqlCommand(insertQuery, mySqlConnection);

                if (command.ExecuteNonQuery() == 1)
                {
                    //Debug.Log("Insert Success");
                    return true;
                }
                else
                {
                    //Debug.Log("Insert Failure");
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

    public static DataTable Select(List<ATTRIBUTE> dataList, string id)
    {
        string attributes = dataList.Count == 0 ? "*" : "";
        dataList.ForEach(column => { attributes += (attributes.Length == 0 ? column : ", " + column); });

        string selectQuery = $"SELECT {attributes} FROM users WHERE {ATTRIBUTE.id} = '{id}'";
        //Debug.Log(selectQuery);

        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));
        DataTable dataTable = new DataTable();

        try
        {
            mySqlConnection.Open();
            //Debug.Log("Connected");

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

    public static void Update(List<KeyValuePair<ATTRIBUTE, string>> dataList, string id)
    {
        string values = "";
        dataList.ForEach(data => { values += ((values.Length == 0 ? "" : ", ") + data.Key + $"='{data.Value}'"); });

        string updateQuery = $"UPDATE users SET {values} WHERE {ATTRIBUTE.id} = '{id}'";
        //Debug.Log(updateQuery);


        MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", "plus2048.cb8k6mln4cv6.ap-northeast-2.rds.amazonaws.com", "3306", "plus2048", "admin", "dbjunohshin"));

        try
        {
            mySqlConnection.Open();
            //Debug.Log("Connected");

            try
            {
                MySqlCommand command = new MySqlCommand(updateQuery, mySqlConnection);

                if (command.ExecuteNonQuery() == 1)
                {
                    //Debug.Log("Update Success");
                }
                else
                {
                    //Debug.Log("Update Failure");
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
