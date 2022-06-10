using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public string id = "";
    public string nickName = "";
    public int highestScore = 0;
    public int highestBlock = 0;
    public int games = 0;
    public int win = 0;
    public int lose = 0;
    public int exp = 0;

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
        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        var data = DatabaseManager.Select(new List<DatabaseManager.ATTRIBUTE>
        {
            DatabaseManager.ATTRIBUTE.nickname,
            DatabaseManager.ATTRIBUTE.highestscore,
            DatabaseManager.ATTRIBUTE.highestblock,
            DatabaseManager.ATTRIBUTE.games,
            DatabaseManager.ATTRIBUTE.win,
            DatabaseManager.ATTRIBUTE.lose,
            DatabaseManager.ATTRIBUTE.exp
        }, this.id).Rows[0];

        nickName = data[0].ToString();
        highestScore = StrToInt(data[1].ToString());
        highestBlock = StrToInt(data[2].ToString());
        games = StrToInt(data[3].ToString());
        win = StrToInt(data[4].ToString());
        lose = StrToInt(data[5].ToString());
        exp = StrToInt(data[6].ToString());
    }

    private static int StrToInt(string str) => str == "" ? 0 : int.Parse(str);
}
