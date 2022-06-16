using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Single Game을 관리하는 클래스. Mode, Game State를 관리한다
/// </summary>
public class SingleGameManager : MonoBehaviour
{
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
    public class GameState
    {
        [SerializeField] private bool empty = true;
        public int currentScore = 0;
        public int highestBlock = 0;
        public List<Block> blockList = new List<Block>();

        public GameState() { }
        public GameState(int currentScore, int highestBlock, List<Node> nodeList)
        {
            this.currentScore = currentScore;
            this.highestBlock = highestBlock;
            nodeList.ForEach(node => { if (node.value != null) { this.empty = false; } this.blockList.Add(new Block(node.value, new Vector2Int(node.point.x, node.point.y))); });
        }

        public bool Empty() => empty;
    }


    [System.Serializable]
    public class GameStateList
    {
        public List<GameState> mainState = new List<GameState>();
        public List<GameState> tempState = new List<GameState>();

        public bool Empty() => mainState.Count == 0 ? true : false;
    }


    //------------------------------ 선언부 ----------------------------//
    public static SingleGameManager Instance;

    public bool isReady = false;
    public int highestScore = 0;
    public int highestBlock = 0;
    public int exp = 0;
    public GameStateList gameStateList;// = new GameStateList();


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("SingleGameManager Awake!!");
        LoadFromDB();
    }


    // ------------------------------ Game Data Management ------------------------------- //
    private void SaveToDB()
    {
        var player = PlayerManager.Instance;
        var mode = SingleGameModeManager.Instance;
  
        // Challenge Mode일 때 기록 갱신
        if (mode.GetMode() == SingleGameModeManager.MODE.CHALLENGE)
        {
            DatabaseManager.Update(new List<KeyValuePair<DatabaseManager.ATTRIBUTE, string>>
            {
                new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.highestscore, this.highestScore.ToString()),
                new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.highestblock, this.highestBlock.ToString()),
            }, player.id);
        }

        // Game State Update
        DatabaseManager.Update(new List<KeyValuePair<DatabaseManager.ATTRIBUTE, string>>
        {
            new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(DatabaseManager.ATTRIBUTE.exp, exp.ToString()),
            new KeyValuePair<DatabaseManager.ATTRIBUTE, string>(
                new Dictionary<SingleGameModeManager.MODE, DatabaseManager.ATTRIBUTE>
                {
                    { SingleGameModeManager.MODE.CLASSIC, DatabaseManager.ATTRIBUTE.saveclassicmode },
                    { SingleGameModeManager.MODE.CHALLENGE, DatabaseManager.ATTRIBUTE.savechallengemode },
                    { SingleGameModeManager.MODE.PRACTICE, DatabaseManager.ATTRIBUTE.savepracticemode }
                }[mode.GetMode()], JsonManager.Serialize(gameStateList))
        }, player.id);
    }
    
    private void LoadFromDB()
    {
        var player = PlayerManager.Instance;
        var mode = SingleGameModeManager.Instance;
        var data = new System.Data.DataTable().Rows;

        if (mode.GetMode() == SingleGameModeManager.MODE.CLASSIC)
        {
            data = DatabaseManager.Select(new List<DatabaseManager.ATTRIBUTE>
            {
                DatabaseManager.ATTRIBUTE.highestscore, DatabaseManager.ATTRIBUTE.highestblock, DatabaseManager.ATTRIBUTE.exp, DatabaseManager.ATTRIBUTE.saveclassicmode
            }, player.id).Rows;
        }
        else if (mode.GetMode() == SingleGameModeManager.MODE.CHALLENGE)
        {
            data = DatabaseManager.Select(new List<DatabaseManager.ATTRIBUTE>
            {
                DatabaseManager.ATTRIBUTE.highestscore, DatabaseManager.ATTRIBUTE.highestblock, DatabaseManager.ATTRIBUTE.exp,DatabaseManager.ATTRIBUTE.savechallengemode
            }, player.id).Rows;
        }
        else if (mode.GetMode() == SingleGameModeManager.MODE.PRACTICE)
        {
            data = DatabaseManager.Select(new List<DatabaseManager.ATTRIBUTE>
            {
                DatabaseManager.ATTRIBUTE.highestscore, DatabaseManager.ATTRIBUTE.highestblock, DatabaseManager.ATTRIBUTE.exp,DatabaseManager.ATTRIBUTE.savepracticemode
            }, player.id).Rows;
        }

        this.highestScore = (data[0][0].ToString() == "" ? 0 : int.Parse(data[0][0].ToString()));
        this.highestBlock = (data[0][1].ToString() == "" ? 0 : int.Parse(data[0][1].ToString()));
        this.exp = (data[0][2].ToString() == "" ? 0 : int.Parse(data[0][2].ToString()));
        this.gameStateList = (data[0][3].ToString() == "" ? new GameStateList() : JsonManager.Deserialize<GameStateList>(data[0][3].ToString()));

        isReady = true;
    }



    // ------------------------------ Game Play Management ------------------------------- //
    public GameState LoadGame()
    {
        return gameStateList.Empty() ? new GameState() : gameStateList.mainState[gameStateList.mainState.Count - 1];
    }

    public void UpdateGame()
    {
        var board = SingleBoard.Instance;
        var mode = SingleGameModeManager.Instance;
        var undoSize = new List<int> { 1, 1, 10 }[(int)mode.GetMode()];

        if (gameStateList.mainState.Count > undoSize) gameStateList.mainState.RemoveAt(0);

        this.highestScore = Math.Max(highestScore, board.highestScore);
        this.highestBlock = Math.Max(highestBlock, board.highestBlock);
        this.exp = board.exp;
        this.gameStateList.mainState.Add(new GameState(board.currentScore, board.highestBlock, board.nodeList));
        this.gameStateList.tempState.Clear();
    }

    public void SaveGame()
    {
        SaveToDB();
    }

    public void ClearGame()
    {
        this.gameStateList = new GameStateList();
        SaveToDB();
    }

    public bool UndoGame()
    {
        if (gameStateList.mainState.Count >= 2)
        {
            gameStateList.tempState.Add(gameStateList.mainState[gameStateList.mainState.Count - 1]);
            gameStateList.mainState.RemoveAt(gameStateList.mainState.Count - 1);
            return true;
        }
        return false;
    }

    public bool RedoGame()
    {
        if (gameStateList.tempState.Count >= 1)
        {
            gameStateList.mainState.Add(gameStateList.tempState[gameStateList.tempState.Count - 1]);
            gameStateList.tempState.RemoveAt(gameStateList.tempState.Count - 1);
            return true;
        }
        return false;
    }
}