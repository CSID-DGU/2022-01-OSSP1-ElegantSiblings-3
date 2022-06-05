using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.IO;


[System.Serializable]
public class SingleGameDataManager
{
    public int curr_score = 0;
    public int best_score = 0;
    public int high_block = 0;
    public List<Block> block_list = new List<Block>();

    
    // TODO: Get_Json 

    public static void Write(SingleGameDataManager game_data)
    {
        Json.Write(Path.Combine(Application.persistentDataPath, "SingleGameDataManager.json"), game_data);
    }

    public static SingleGameDataManager Read()
    {
        SingleGameDataManager game_data = Json.Read<SingleGameDataManager>(Path.Combine(Application.persistentDataPath, "SingleGameDataManager.json"));
        return game_data == null ? new SingleGameDataManager() : game_data;
    }
}


[System.Serializable]
public class Block
{
    public int? value = null;
    public Vector2Int point = new Vector2Int();
}


[System.Serializable]
public class GameData
{
    public bool fixedState = false;
    public int targetBlockNumber = 0;
    public int highestBlockNumber = 0;
    public int currScore = 0;
    public int highScore = 0;
    public List<NodeClone> nodeData = new List<NodeClone>();

    public string GetJson() => JsonUtility.ToJson(this, true);  

    public void clear()
    {
        fixedState = false;   
        highestBlockNumber = 0;
        currScore = 0;
        nodeData = new List<NodeClone>();
    }
    public void clearAll()
    {
        targetBlockNumber = 0;
        highScore = 0;
        clear();
    }

    public GameData Copy()
    {
        GameData temp = new GameData();

        temp.fixedState = fixedState;
        temp.targetBlockNumber = targetBlockNumber;
        temp.highestBlockNumber = highestBlockNumber;
        temp.currScore = currScore;
        temp.highScore = highScore;
        foreach (NodeClone e in nodeData) temp.nodeData.Add(e.Copy());

        return temp;
    }
}