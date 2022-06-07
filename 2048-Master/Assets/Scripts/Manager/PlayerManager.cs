using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System.Data;
using System.IO;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public string id = "";
    public string passWord = "";
    public string nickName = "";
    public int highestScore = 0;
    public int highestBlock = 0;
    public int games = 0;
    public int win = 0;
    public int lose = 0;
    public int exp = 0;
    public Dictionary<SINGLE_GAME_MODE, string> singleModeData = new Dictionary<SINGLE_GAME_MODE, string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(string id)
    {
        this.id = id;
        Construct();
    }

    public void Update()
    {
        Construct();
    }

    private void Construct()
    {
        var playerData = DatabaseManager.Select(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(DatabaseManager.GetDBAttribute(DatabaseManager.ATTRIBUTE.ID), this.id),
        }).Rows[0];

        this.id = playerData[(int)DatabaseManager.ATTRIBUTE.ID].ToString();
        this.passWord = playerData[(int)DatabaseManager.ATTRIBUTE.PASSWORD].ToString();
        this.nickName = playerData[(int)DatabaseManager.ATTRIBUTE.NICKNAME].ToString();
        this.highestScore = StrToInt(playerData[(int)DatabaseManager.ATTRIBUTE.HIGHESTSCORE].ToString());
        this.highestBlock = StrToInt(playerData[(int)DatabaseManager.ATTRIBUTE.HIGHESTBLOCK].ToString());
        this.games = StrToInt(playerData[(int)DatabaseManager.ATTRIBUTE.GAMES].ToString());
        this.win = StrToInt(playerData[(int)DatabaseManager.ATTRIBUTE.WIN].ToString());
        this.lose = StrToInt(playerData[(int)DatabaseManager.ATTRIBUTE.LOSE].ToString());
        this.exp = StrToInt(playerData[(int)DatabaseManager.ATTRIBUTE.EXP].ToString());
        this.singleModeData = new Dictionary<SINGLE_GAME_MODE, string>
        {
            { SINGLE_GAME_MODE.CLASSIC, playerData[(int)DatabaseManager.ATTRIBUTE.SAVECLASSICMODE].ToString() },
            { SINGLE_GAME_MODE.CHALLENGE, playerData[(int)DatabaseManager.ATTRIBUTE.SAVECHALLENGEMODE].ToString() },
            { SINGLE_GAME_MODE.PRACTICE, playerData[(int)DatabaseManager.ATTRIBUTE.SAVEPRACTICEMODE].ToString() },
        };
    }

    private static int StrToInt(string str) => str == "" ? 0 : int.Parse(str);
}
