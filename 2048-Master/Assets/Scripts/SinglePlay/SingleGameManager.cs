using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.IO;


[System.Serializable]
public class Block
{
    [SerializeField] private int value = -1;
    [SerializeField] private Vector2Int point = new Vector2Int();

    public Block() { }
    public Block(int? v, Vector2Int p) { value = (v == null ? -1 : v.GetValueOrDefault()); point = new Vector2Int(p.x, p.y); }

    public int? GetValue() => (value == -1 ? null : value);
    public Vector2Int GetPoint() => new Vector2Int(point.x, point.y);
}


[System.Serializable]
public class SingleGameState
{
    public int currScore = 0;
    public int bestScore = 0;
    public int highestBlockNumber = 0;
    public List<Block> blockList = new List<Block>();

    public bool Empty()
    {
        foreach (var block in blockList)
            if (block.GetValue() != null) return false;
        return true;
    }

    public string GetJson() => Json.GetJson(this);
}


[System.Serializable]
public class SingleGameStateList
{
    public List<SingleGameState> mainState = new List<SingleGameState>();
    public List<SingleGameState> subState = new List<SingleGameState>();

    public bool Empty() => mainState.Count == 0 ? true : false;

    public string GetJson() => JsonUtility.ToJson(this, true);
}


public enum SINGLE_GAME_MODE
{
    CLASSIC, CHALLENGE, PRACTICE
}

[System.Serializable]
public class SingleGameMode
{
    public SINGLE_GAME_MODE index = SINGLE_GAME_MODE.CLASSIC;
    public string name = "Classic";
}



/// <summary>
/// Single Game을 관리하는 클래스. Mode, Game State를 관리한다
/// </summary>
public class SingleGameManager
{
    //------------------- Mode Management -------------------//
    public static void SetGameMode(SINGLE_GAME_MODE mode)
    {
        Json.Write(Path.Combine(Application.persistentDataPath, "SingleMode.json"),
            new SingleGameMode { index = mode, name = new List<string> { "Classic", "Challenge", "Practice" }[(int)mode] });
    }

    public static SingleGameMode GetGameMode()
    {
        SingleGameMode mode = Json.Read<SingleGameMode>(Path.Combine(Application.persistentDataPath, "SingleMode.json"));
        return mode == null ? new SingleGameMode() : mode;
    }



    //------------------- Game State Management -------------------//
    private static void SaveGameState(SingleGameStateList gameStateList)
    {
        Json.Write(Path.Combine(Application.persistentDataPath, "SingleGameState" + GetGameMode().name + ".json"), gameStateList);
    }
    
    private static SingleGameStateList LoadGameState()
    {
        var gameStateList = Json.Read<SingleGameStateList>(Path.Combine(Application.persistentDataPath, "SingleGameState" + GetGameMode().name + ".json"));
        return gameStateList == null ? new SingleGameStateList() : gameStateList;
    }

    public static void AddGameState(SingleBoard board)
    {
        var gameStateList = LoadGameState();
        var gameState = new SingleGameState();

        gameState.currScore = board.currScore;
        gameState.bestScore = board.bestScore;
        gameState.highestBlockNumber = board.highestBlockNumber;
        foreach (var node in board.nodeList) gameState.blockList.Add(new Block(node.value, new Vector2Int(node.point.x, node.point.y)));

        int undoSize = new List<int> { 1, 1, 10 }[(int)GetGameMode().index];
        gameStateList.mainState.Add(gameState);
        if (gameStateList.mainState.Count > undoSize + 1) gameStateList.mainState.RemoveAt(0);
        gameStateList.subState.Clear();

        SaveGameState(gameStateList);
    }

    public static SingleGameState GetGameState()
    {
        var gameStateList = LoadGameState();
        return gameStateList.mainState.Count == 0 ? new SingleGameState() : gameStateList.mainState[gameStateList.mainState.Count - 1];
    }

    public static void ClearGameState()
    {
        Json.Delete(Path.Combine(Application.persistentDataPath, "SingleGameState" + GetGameMode().name + ".json"));
    }

    public static void Undo()
    {
        var gameStateList = LoadGameState();

        if (gameStateList.mainState.Count >= 2)
        {
            gameStateList.subState.Add(gameStateList.mainState[gameStateList.mainState.Count - 1]);
            gameStateList.mainState.RemoveAt(gameStateList.mainState.Count - 1);
        }

        SaveGameState(gameStateList);
    }

    public static void Redo()
    {
        var gameStateList = LoadGameState();

        if (gameStateList.subState.Count >= 1)
        {
            gameStateList.mainState.Add(gameStateList.subState[gameStateList.subState.Count - 1]);
            gameStateList.subState.RemoveAt(gameStateList.subState.Count - 1);
        }

        SaveGameState(gameStateList);
    }
}














/*
     //---------------- Game Data Management ----------------//
    public static void SaveGameData(Board_Single board)
    {
        // game data를 기기에 저장
        SingleGameState gameData = new SingleGameState();

        gameData.curr_score = board.currScore;
        gameData.high_block = board.highestBlockNumber;
        foreach (var node in board.nodeList) gameData.block_list.Add(new Block(node.value, new Vector2Int(node.point.x, node.point.y)));

        Json.Write(Path.Combine(Application.persistentDataPath, "SingleGameDataManager" + GetGameMode().name + ".json"), gameData);

        // TODO: game data를 DB에 저장
    }

    public static SingleGameState LoadGameData()
    {
        SingleGameState gameData
            = Json.Read<SingleGameState>(Path.Combine(Application.persistentDataPath, "SingleGameDataManager" + GetGameMode().name + ".json"));
        return gameData == null ? new SingleGameState() : gameData;
    }

    public static void ClearGameData()
    {
        Json.Delete(Path.Combine(Application.persistentDataPath, "SingleGameDataManager" + GetGameMode().name + ".json"));
    }
 */


/*[System.Serializable]
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
}*/