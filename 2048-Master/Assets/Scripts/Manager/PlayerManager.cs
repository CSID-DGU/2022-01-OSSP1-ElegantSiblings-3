using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System.Data;
using System.IO;


[System.Serializable]
public class Player
{
    public string id = "";
    public string passWord = "";
    public string nickName = "";
    public int highestScore = 0;
    public int highestBlock = 0;
    public int games = 0;
    public int win = 0;
    public int lose = 0;
    public int exp = 0;
    public List<string> singleModeData = new List<string>();
}


public class PlayerManager
{
    private static int StringToInt(string str)
    {
        return str == "" ? 0 : int.Parse(str);
    }

    /// <summary>
    /// Player Data 임시 저장
    /// </summary>
    public static void SaveTempPlayerData(DataTable playerData)
    {
        Player player = new Player
        {
            id = playerData.Rows[0][0].ToString(),
            passWord = playerData.Rows[0][1].ToString(),
            nickName = playerData.Rows[0][3].ToString(),
            highestScore = StringToInt(playerData.Rows[0][4].ToString()),
            highestBlock = StringToInt(playerData.Rows[0][5].ToString()),
            games = StringToInt(playerData.Rows[0][6].ToString()),
            win = StringToInt(playerData.Rows[0][7].ToString()),
            lose = StringToInt(playerData.Rows[0][8].ToString()),
            exp = StringToInt(playerData.Rows[0][9].ToString()),
            singleModeData = new List<string> { playerData.Rows[0][10].ToString(), playerData.Rows[0][11].ToString(), playerData.Rows[0][12].ToString() }
        };

        JsonManager.Write(Path.Combine(Application.persistentDataPath, "SaveTempPlayerData.json"), player);
    }


    public static Player ReloadPlayerData(string id)
    {
        DataTable dataTable = DatabaseManager.Select(new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("id", id)
        });

        Player player = new Player();

        if (dataTable.Rows.Count != 0)
        {
            player = new Player
            {
                id = dataTable.Rows[0][0].ToString(),
                passWord = dataTable.Rows[0][1].ToString(),
                nickName = dataTable.Rows[0][3].ToString(),
                highestScore = StringToInt(dataTable.Rows[0][4].ToString()),
                highestBlock = StringToInt(dataTable.Rows[0][5].ToString()),
                games = StringToInt(dataTable.Rows[0][6].ToString()),
                win = StringToInt(dataTable.Rows[0][7].ToString()),
                lose = StringToInt(dataTable.Rows[0][8].ToString()),
                exp = StringToInt(dataTable.Rows[0][9].ToString()),
                singleModeData = new List<string> { dataTable.Rows[0][10].ToString(), dataTable.Rows[0][11].ToString(), dataTable.Rows[0][12].ToString() }
            };
        }

        JsonManager.Write(Path.Combine(Application.persistentDataPath, "SaveTempPlayerData.json"), player);

        return player;
    }


    public static Player LoadTempPlayerData()
    {
        return JsonManager.Read<Player>(Path.Combine(Application.persistentDataPath, "SaveTempPlayerData.json"));
    }

    public static void ClearTempPlayerData()
    {
        JsonManager.Delete(Path.Combine(Application.persistentDataPath, "SaveTempPlayerData.json"));
    }
}
