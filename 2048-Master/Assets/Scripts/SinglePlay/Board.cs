﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;

/// <summary>
/// coder by shlifedev(zero is black)
/// </summary>
public class Board : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum State
    {
        WAIT, PROCESSING, END
    }

    public State state = State.WAIT;

    public static Board Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<Board>();
            return _inst;
        }
    }



    // Create Board and Node
    private static Board _inst;
    public List<NodeObject> realNodeList = new List<NodeObject>();
    public List<Node> nodeData = new List<Node>();
    public Dictionary<Vector2Int, Node> nodeMap = new Dictionary<Vector2Int, Node>();
    public int col = 4;
    public int row = 4;

    public GameObject emptyNodePrefab;
    public GameObject nodePrefab;
    public RectTransform emptyNodeRect;
    public RectTransform realNodeRect;


    // GameData Load, Save, NewGame, Undo, Redo
    public bool isReloading;
    public SinglePlayMode gameMode;
    public GameData gameData;
    public StateData stateData;
    public string path_gameMode;
    public string path_gameData;
    public string path_stateData;



    // Touch Event
    public Vector2 vectorS = new Vector2();
    public Vector2 vectorE = new Vector2();
    public Vector2 vectorM = new Vector2();






    private void Awake()
    {
        LoadGame();
        ThemeRoad();
        CreateGameBoard();
    }

    public void OnApplicationQuit()
    {
        SaveGame();
        SaveToJson();
    }

    private void Start() { }





    //============================== Button Object Event ==============================//
    public void ReturnPrevPageButton()
    {
        SaveGame();
        SaveToJson();
        SceneManager.LoadScene("Scene_SinglePlay");
    }
    public void ReturnHomePageButton()
    {
        SaveGame();
        SaveToJson();
        SceneManager.LoadScene("Scene_Home");
    }
    public void NewGameButton()
    {
        ClearGame();
        SaveToJson();
        isReloading = true;
        SceneManager.LoadScene("Game");
    }
    public void UndoButton()
    {
        UndoGame();
        SaveToJson();
        isReloading = true;
        SceneManager.LoadScene("Game");
    }
    public void RedoButton()
    {
        RedoGame();
        SaveToJson();
        isReloading = true;
        SceneManager.LoadScene("Game");
    }



    //============================== Game Data Method ==============================//
    private void LoadGame()
    {
        isReloading = false;

        gameMode = new SinglePlayMode();
        path_gameMode = Path.Combine(Application.persistentDataPath, "SinglePlayMode.json");
        if (File.Exists(path_gameMode))
        {
            string loadJson = File.ReadAllText(path_gameMode);
            gameMode = JsonUtility.FromJson<SinglePlayMode>(loadJson);
            if (gameMode == null) gameMode = new SinglePlayMode();
        }

        gameData = new GameData() { targetBlockNumber = new Dictionary<string, int> { { "ClassicMode", 2048 }, { "ChallengeMode", 1073741824 }, { "PracticeMode", 2048 } }[gameMode.modeName] };
        path_gameData = Path.Combine(Application.persistentDataPath, gameMode.modeName + "GameData.json");
        if (File.Exists(path_gameData))
        {
            string loadJson = File.ReadAllText(path_gameData);
            gameData = JsonUtility.FromJson<GameData>(loadJson);
            if (gameData == null) gameData = new GameData() { targetBlockNumber = new Dictionary<string, int> { { "ClassicMode", 2048 }, { "ChallengeMode", 1073741824 }, { "PracticeMode", 2048 } }[gameMode.modeName] };
        }

        stateData = new StateData() { stateDataCount = new Dictionary<string, int> { { "ClassicMode", 1 }, { "ChallengeMode", 1 }, { "PracticeMode", 10 } }[gameMode.modeName] };
        path_stateData = Path.Combine(Application.persistentDataPath, gameMode.modeName + "StateData.json");
        if (File.Exists(path_stateData))
        {
            string loadJson = File.ReadAllText(path_stateData);
            stateData = JsonUtility.FromJson<StateData>(loadJson);
            if (stateData == null) stateData = new StateData() { stateDataCount = new Dictionary<string, int> { { "ClassicMode", 1 }, { "ChallengeMode", 1 }, { "PracticeMode", 10 } }[gameMode.modeName] };
        }
    }
    private void ThemeRoad()
    {
        GameObject.Find("ReturnPrevPage").GetComponent<Image>().sprite = Resources.Load<Sprite>("theme3/" + "BackArrow_Theme3");
        GameObject.Find("ReturnHomePage").GetComponent<Image>().sprite = Resources.Load<Sprite>("theme3/" + "ReturnHome_Theme3");

        GameObject.Find("BackGround").GetComponent<Image>().sprite = Resources.Load<Sprite>("theme3/" + gameMode.modeName + "GameBoard_theme3");
        GameObject.Find("NewGame").GetComponent<Image>().sprite = Resources.Load<Sprite>("theme3/" + gameMode.modeName + "NewGame_theme3");
        GameObject.Find("Undo").GetComponent<Image>().sprite = Resources.Load<Sprite>("theme3/" + gameMode.modeName + "Undo_theme3");
        GameObject.Find("Redo").GetComponent<Image>().sprite = Resources.Load<Sprite>("theme3/" + gameMode.modeName + "Redo_theme3");
    }
    private void SaveGame()
    {
        gameData.nodeData.Clear();
        foreach (var node in nodeData) gameData.nodeData.Add(new NodeClone(node));

    }
    private void ClearGame()
    {
        gameData.clear();
        stateData.clear();
    }
    private void UndoGame()
    {
        gameData = stateData.Undo();
    }
    private void RedoGame()
    {
        gameData = stateData.Redo();
    }
    private void SaveToJson()
    {
        File.WriteAllText(path_gameData, gameData.GetJson());
        File.WriteAllText(path_stateData, stateData.GetJson());
    }
    private void ClearJson()
    {
        File.WriteAllText(path_gameData, null);
        File.WriteAllText(path_stateData, null);
    }





    //============================== Make Game Board ==============================//
    private void NewBoard()
    {
        var emptyChildCount = emptyNodeRect.transform.childCount;
        for (int i = 0; i < emptyChildCount; i++) { var child = emptyNodeRect.GetChild(i); }

        /* and, empty node create for get grid point*/
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var instantiatePrefab = GameObject.Instantiate(emptyNodePrefab, emptyNodeRect.transform, false);
                var point = new Vector2Int(j, i);
                //r-d-l-u
                Vector2Int left = point - new Vector2Int(1, 0);
                Vector2Int down = point - new Vector2Int(0, 1);
                Vector2Int right = point + new Vector2Int(1, 0);
                Vector2Int up = point + new Vector2Int(0, 1);
                Vector2Int?[] v = new Vector2Int?[4];
                if (IsValid(right)) v[0] = right;
                if (IsValid(down)) v[1] = down;
                if (IsValid(left)) v[2] = left;
                if (IsValid(up)) v[3] = up;

                Node node = new Node(v);
                node.point = point;
                node.nodeRectObj = instantiatePrefab;
                nodeData.Add(node);
                instantiatePrefab.name = node.point.ToString();
                this.nodeMap.Add(point, node);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(emptyNodeRect);
        foreach (var data in nodeData)
            data.position = data.nodeRectObj.GetComponent<RectTransform>().localPosition;

        CreateRandom();
    }

    private void CreateGameBoard()
    {
        bool exsitSaveFile = gameData.nodeData.Count == 0 ? false : true;

        /* first initialize Score Board */
        //GameObject.Find("ModeName").GetComponent<TextMeshProUGUI>().text = new Dictionary<string, string> { { "ClassicMode", "Classic Mode" }, { "ChallengeMode", "Challenge Mode" }, { "PracticeMode", "Practice Mode" } }[gameMode.modeName];
        GameObject.Find("TargetNumber").GetComponent<TextMeshProUGUI>().text = new Dictionary<int, string> { { 2048, "2048" }, { 1073741824, "Infinity" } }[gameData.targetBlockNumber];
        GameObject.Find("CurrScore").GetComponent<TextMeshProUGUI>().text = gameData.currScore.ToString();
        GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>().text = gameData.highScore.ToString();

        /* first initialize empty Node rect */
        realNodeList.Clear();
        nodeMap.Clear();
        nodeData.Clear();

        if (exsitSaveFile == false) NewBoard();      
        else
        {
            var emptyChildCount = emptyNodeRect.transform.childCount;
            for (int i = 0; i < emptyChildCount; i++) { var child = emptyNodeRect.GetChild(i); }

            /* and, empty node create for get grid point*/
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    var instantiatePrefab = GameObject.Instantiate(emptyNodePrefab, emptyNodeRect.transform, false);
                    var point = new Vector2Int(j, i);
                    //r-d-l-u
                    Vector2Int left = point - new Vector2Int(1, 0);
                    Vector2Int down = point - new Vector2Int(0, 1);
                    Vector2Int right = point + new Vector2Int(1, 0);
                    Vector2Int up = point + new Vector2Int(0, 1);
                    Vector2Int?[] v = new Vector2Int?[4];
                    if (IsValid(right)) v[0] = right;
                    if (IsValid(down)) v[1] = down;
                    if (IsValid(left)) v[2] = left;
                    if (IsValid(up)) v[3] = up;

                    Node node = new Node(v);
                    node.point = point;
                    node.nodeRectObj = instantiatePrefab;

                    NodeClone nodeClone = gameData.nodeData[i * 4 + j];
                    node.value = nodeClone.value == -1 ? null : nodeClone.value;
                    //node.combined = nodeClone.combined;

                    nodeData.Add(node);
                    instantiatePrefab.name = node.point.ToString();
                    this.nodeMap.Add(point, node);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(emptyNodeRect);  
            foreach (var data in nodeData)
                data.position = data.nodeRectObj.GetComponent<RectTransform>().localPosition;

            foreach (var data in gameData.nodeData)
                if (data.value >= 2) CreateBlock(data.point.x, data.point.y, data.value);
        }

        SaveGame();

        if (stateData.Empty()) stateData.AddState(gameData.Copy());
    }

    private bool IsValid(Vector2Int point)
    {
        if (point.x == -1 || point.x == row || point.y == col || point.y == -1)
            return false;

        return true;
    }
    private void CreateBlock(int x, int y, int? blockNum = null)
    {
        if (nodeMap[new Vector2Int(x, y)].realNodeObj != null) return;

        GameObject realNodeObj = Instantiate(nodePrefab, realNodeRect.transform, false);
        var node = nodeMap[new Vector2Int(x, y)];
        var pos = node.position;
        realNodeObj.GetComponent<RectTransform>().localPosition = pos;
        realNodeObj.transform.DOPunchScale(new Vector3(.32f, .32f, .32f), 0.15f, 3);
        var nodeObj = realNodeObj.GetComponent<NodeObject>();
        this.realNodeList.Add(nodeObj);

        if (blockNum == null) nodeObj.InitializeFirstValue();
        else nodeObj.InitializeSavedValue(blockNum.GetValueOrDefault());

        node.value = nodeObj.value;
        node.realNodeObj = nodeObj;
    }



    //============================== Game Rule ==============================//
    public void Combine(Node from, Node to)
    {
        to.value = to.value * 2;

        from.value = null;
        if (from.realNodeObj != null)
        {
            from.realNodeObj.CombineToNode(from, to);
            from.realNodeObj = null;
            to.combined = true;
        }

        gameData.currScore += to.value.GetValueOrDefault();
        gameData.highScore = Mathf.Max(gameData.highScore, gameData.currScore);
        gameData.highestBlockNumber = Mathf.Max(gameData.highestBlockNumber, to.value.GetValueOrDefault());
        GameObject.Find("CurrScore").GetComponent<TextMeshProUGUI>().text = gameData.currScore.ToString();
        GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>().text = gameData.highScore.ToString();
    }

    public void Move(Node from, Node to)
    {
//        Debug.Log($"TRY MOVE {from.point} , {to.point}");
        to.value = from.value;
        from.value = null;  
        if (from.realNodeObj != null)
        {
            from.realNodeObj.MoveToNode(from, to);
            if (from.realNodeObj != null)
            {
                to.realNodeObj = from.realNodeObj;
                from.realNodeObj = null;
                //Debug.Log(to.realNodeObj != null);
            }
        }
    }
 
    /// <summary>
    /// Move Blocks by User Input.
    /// </summary>
    /// <param name="dir"></param>
    public void MoveTo(Node.Direction dir)
    {
        if (dir == Node.Direction.RIGHT)
        {
            for (int j = 0; j < col; j++)
            {
                for (int i = (row - 2); i >= 0; i--)
                { 
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null) 
                        continue; 
                    var right = node.FindTarget(node, Node.Direction.RIGHT);
                    if (right != null)
                    {
                        if (node.value.HasValue && right.value.HasValue)
                        {
                            if (node.value == right.value)
                            {
                                Combine(node, right);
                            }
                        }
                        else if (right != null && right.value.HasValue == false)
                        {
                            Move(node, right);
                        }
                        else if (right == null) return;
                    } 
                }
            }
 
        }
        if (dir == Node.Direction.LEFT)
        { 
            for (int j = 0; j< col; j ++)
            {
                for (int i = 1; i < row; i++)
                { 
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null) 
                        continue; 

                    var left = node.FindTarget(node, Node.Direction.LEFT);
                    if (left != null)
                    {
                        if (node.value.HasValue && left.value.HasValue)
                        {
                            if (node.value == left.value)
                            {
                                Combine(node, left);
                            }
                        }
                        else if (left != null && left.value.HasValue == false)
                        {
                            Move(node, left);
                        }  
                    } 
                }
            }
 
        }
        if (dir == Node.Direction.UP)
        {
            for (int j = col-2; j >= 0; j --)
            {
                for (int i = 0; i<row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null) 
                        continue;  
                    var up = node.FindTarget(node, Node.Direction.UP);
                    if (up != null)
                    {
                        if (node.value.HasValue && up.value.HasValue)
                        {
                            if (node.value == up.value)
                            {
                                Combine(node, up);
                            }
                        }
                        else if (up != null && up.value.HasValue == false)
                        {
                            Move(node, up);
                        }  
                    } 
                }
            }
        }
        if (dir == Node.Direction.DOWN)
        {
            for (int j = 1; j<col; j++)
            {
                for (int i = 0; i< row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null) 
                        continue;  
                    var down = node.FindTarget(node, Node.Direction.DOWN);
                    if (down != null)
                    {
                        if (node.value.HasValue && down.value.HasValue)
                        {
                            if (node.value == down.value)
                            {
                                Combine(node, down);
                            }
                        }
                        else if (down != null && down.value.HasValue == false)
                        {
                            Move(node, down);
                        }  
                    } 
                }
            }
        }
        
        foreach (var data in realNodeList)
        {
            if (data.target != null)
            {
                state = State.PROCESSING; 
                data.StartMoveAnimation(); 
            }
        }
        
        Show();

        if (IsGameOver())
        {
           OnGameOver();
        }
    }

    /// <summary>
    /// if can't combine anymore then game over!!!!
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        bool gameOver = true;
        nodeData.ForEach(x =>
        { 
            for (int i = 0; i < x.linkedNode.Length; i++)
            {
                if (x.realNodeObj == null) gameOver = false;
                if (x.linkedNode[i] == null)
                    continue;
                
                var nearNode = nodeMap[x.linkedNode[i].Value];
                if(x.value != null && nearNode.value != null)
                if (x.value == nearNode.value)
                {
                    gameOver = false;
                }
            } 
        });

        return gameOver;
    }
    private void CreateRandom()
    {
        var emptys = nodeData.FindAll(x => x.realNodeObj == null); 
        if (emptys.Count == 0)
        {
            if (IsGameOver())
            {
                OnGameOver();
            }
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, emptys.Count);
            var pt = emptys[rand].point;
            CreateBlock(pt.x, pt.y);
        }
    }

    public void OnGameOver()
    {
        Debug.Log("Game Over!!!!");
    }

    public void OnGameEnd()
    {
        Debug.Log("Congratulations!!!!");
    }

    public void UpdateState()
    {
        bool targetAllNull = true;
        foreach (var data in realNodeList)
        {
            if (data.target != null)
            { 
                targetAllNull = false;
                break;
            }
        }

        if (targetAllNull)
        {
            if (state == State.PROCESSING)
            { 
                var removed = new List<NodeObject>();
                List<NodeObject> removeTarget = new List<NodeObject>();
                foreach (var data in realNodeList) 
                    if (data.needDestroy)  
                        removeTarget.Add(data);
                
                removeTarget.ForEach(x =>
                {
                    realNodeList.Remove(x);
                    GameObject.Destroy(x.gameObject);
                });
                state = State.END;
            }
        }

        if (state == State.END)
        {
            nodeData.ForEach(x => x.combined = false);
            state = State.WAIT;
            CreateRandom();

            //--- State Save For UndoRedo ---//
            SaveGame();        
            stateData.AddState(gameData);
            if (gameData.highestBlockNumber >= gameData.targetBlockNumber) { OnGameEnd(); }
        }
    }

    private void Show()
    {
        string v = null;
        for (int i = col-1; i >= 0; i--)
        {
            for (int j = 0; j < row; j++)
            {
                var p = nodeMap[new Vector2Int(j, i)].value;
                string t = p.ToString();
                if (p.HasValue == false)
                {
                    t = "N";
                } 
                if (p == 0) t = "0";
                    
                v += t + " ";
            } 
            v += "\n";
        } 
    }


    //---------------------------------------------------------------------//
    //---------------------------- Touch Event ----------------------------//
    //---------------------------------------------------------------------//

    private void Update()
    {
        UpdateState();

     /*   UpdateByKeyboard();
        if (Input.GetKeyUp(KeyCode.Backspace)) ReturnPrevPageButton();*/

        UpdateByTouchscreen();
        if (Application.platform == RuntimePlatform.Android)
            if (Input.GetKey(KeyCode.Escape)) ReturnPrevPageButton();
    }


    //--------------- 터치 영역 제한 ----------------//
    public bool TouchGameBoard = false;

    public void OnPointerDown(PointerEventData data)  
    {
        TouchGameBoard = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        TouchGameBoard = true;
    }
    //-----------------------------------------------//


    private void UpdateByKeyboard()
    {
        if (isReloading) return;

        if (state == State.WAIT)
        {
            if (Input.anyKeyDown == true)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTo(Node.Direction.RIGHT);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTo(Node.Direction.LEFT);
                if (Input.GetKeyDown(KeyCode.UpArrow)) MoveTo(Node.Direction.UP);
                if (Input.GetKeyDown(KeyCode.DownArrow)) MoveTo(Node.Direction.DOWN);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space)) Show();
    }
 
    private void UpdateByTouchscreen()
    {
        if (isReloading) return;

        if (state == State.WAIT)
        {
            if (Input.touchCount > 0 && TouchGameBoard == true)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    vectorS = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    vectorE = touch.position;
                    vectorM = new Vector2(vectorE.x - vectorS.x, vectorE.y - vectorS.y);

                    if (Mathf.Abs(vectorM.x) > Mathf.Abs(vectorM.y))
                    {
                        if (vectorM.x > 0) MoveTo(Node.Direction.RIGHT);
                        else MoveTo(Node.Direction.LEFT);
                    }
                    else if (Mathf.Abs(vectorM.x) < Mathf.Abs(vectorM.y))
                    {
                        if (vectorM.y > 0) MoveTo(Node.Direction.UP);
                        else MoveTo(Node.Direction.DOWN);
                    }

                    Show();
                }

                TouchGameBoard = false;
            }
        }
    }
}
 

 