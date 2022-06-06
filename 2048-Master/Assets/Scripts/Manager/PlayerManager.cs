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
    public string highestScore = "";
    public string highestBlock = "";
    public string games = "";
    public string win = "";
    public string lose = "";
    public string exp = "";
}


public class PlayerManager
{
    //id, password, in_date, nickname, highestscore, pvpplaynum, pvpvictorynum, pvpdefeatnum, xp

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
            highestScore = playerData.Rows[0][4].ToString(),
            highestBlock = playerData.Rows[0][5].ToString(),
            games = playerData.Rows[0][6].ToString(),
            win = playerData.Rows[0][7].ToString(),
            lose = playerData.Rows[0][8].ToString(),
            exp = playerData.Rows[0][9].ToString(),
        };

        JsonManager.Write(Path.Combine(Application.persistentDataPath, "SaveTempPlayerData.json"), player);
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
