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
    public int currentScore = 0;
    public int highestScore = 0;
    public int highestBlock = 0;
    public List<Block> blockList = new List<Block>();

    public bool Empty()
    {
        foreach (var block in blockList)
            if (block.GetValue() != null) return false;
        return true;
    }

    public string GetJson() => JsonManager.Serialize(this);
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
        JsonManager.Write(Path.Combine(Application.persistentDataPath, "SingleMode.json"),
            new SingleGameMode { index = mode, name = new List<string> { "Classic", "Challenge", "Practice" }[(int)mode] });
    }

    public static SingleGameMode GetGameMode()
    {
        SingleGameMode mode = JsonManager.Read<SingleGameMode>(Path.Combine(Application.persistentDataPath, "SingleMode.json"));
        return mode == null ? new SingleGameMode() : mode;
    }



    //------------------- Game State Management -------------------//
    private static void SaveGameState(SingleGameStateList gameStateList, SingleBoard gameBoard)
    {
        var player = PlayerManager.Instance;
        var mode = SingleGameManager.GetGameMode().index;

        // Challenge Mode일 때 기록 갱신
        if (mode == SINGLE_GAME_MODE.CHALLENGE)
        {
            DatabaseManager.Update(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(DatabaseManager.GetDBAttribute(DatabaseManager.ATTRIBUTE.HIGHESTSCORE), Math.Max(player.highestScore, gameBoard.highestScore).ToString()),
                new KeyValuePair<string, string>(DatabaseManager.GetDBAttribute(DatabaseManager.ATTRIBUTE.HIGHESTBLOCK), Math.Max(player.highestBlock, gameBoard.highestBlock).ToString()),
            }, player.id);
        }
        
        // Game State Update
        DatabaseManager.Update(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(
                DatabaseManager.GetDBAttribute(new Dictionary<SINGLE_GAME_MODE, DatabaseManager.ATTRIBUTE>
                {
                    { SINGLE_GAME_MODE.CLASSIC, DatabaseManager.ATTRIBUTE.SAVECLASSICMODE },
                    { SINGLE_GAME_MODE.CHALLENGE, DatabaseManager.ATTRIBUTE.SAVECHALLENGEMODE },
                    { SINGLE_GAME_MODE.PRACTICE, DatabaseManager.ATTRIBUTE.SAVEPRACTICEMODE }
                }[mode]), JsonManager.Serialize(gameStateList))
        }, player.id);

        PlayerManager.Instance.Update();
    }

    private static SingleGameStateList LoadGameState()
    {
        var gameStateList = JsonManager.Deserialize<SingleGameStateList>(PlayerManager.Instance.singleModeData[SingleGameManager.GetGameMode().index]);      
        return gameStateList == null ? new SingleGameStateList() : gameStateList;
    }

    public static void ClearGameState(SingleBoard board)
    {
        SaveGameState(new SingleGameStateList(), board);
    }

    public static void AddGameState(SingleBoard board)
    {
        var gameStateList = LoadGameState();
        var gameState = new SingleGameState();

        gameState.currentScore = board.currentScore;
        gameState.highestScore = board.highestScore;
        gameState.highestBlock = board.highestBlock;
        foreach (var node in board.nodeList) gameState.blockList.Add(new Block(node.value, new Vector2Int(node.point.x, node.point.y)));

        int undoSize = new List<int> { 1, 1, 10 }[(int)GetGameMode().index];
        gameStateList.mainState.Add(gameState);
        if (gameStateList.mainState.Count > undoSize + 1) gameStateList.mainState.RemoveAt(0);
        gameStateList.subState.Clear();

        SaveGameState(gameStateList, board);
    }

    public static SingleGameState GetGameState()
    {
        var gameStateList = LoadGameState();
        return gameStateList.Empty() ? new SingleGameState() : gameStateList.mainState[gameStateList.mainState.Count - 1];
    }

    public static void Undo(SingleBoard board)
    {
        var gameStateList = LoadGameState();

        if (gameStateList.mainState.Count >= 2)
        {
            gameStateList.subState.Add(gameStateList.mainState[gameStateList.mainState.Count - 1]);
            gameStateList.mainState.RemoveAt(gameStateList.mainState.Count - 1);
        }

        SaveGameState(gameStateList, board);
    }

    public static void Redo(SingleBoard board)
    {
        var gameStateList = LoadGameState();

        if (gameStateList.subState.Count >= 1)
        {
            gameStateList.mainState.Add(gameStateList.subState[gameStateList.subState.Count - 1]);
            gameStateList.subState.RemoveAt(gameStateList.subState.Count - 1);
        }

        SaveGameState(gameStateList, board);
    }
}