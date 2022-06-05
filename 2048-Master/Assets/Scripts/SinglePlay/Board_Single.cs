using System;
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



public class Board_Single : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum State
    {
        WAIT, PROCESSING, END
    }

    public State state = State.WAIT;

    public static Board_Single Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<Board_Single>();
            return _inst;
        }
    }



    // Create Board and Node
    private static Board_Single _inst;
    public List<NodeObject> real_node_list = new List<NodeObject>();
    public List<Node> node_list = new List<Node>();
    public Dictionary<Vector2Int, Node> node_map = new Dictionary<Vector2Int, Node>();
    public int col = 4;
    public int row = 4;

    public GameObject emptyNodePrefab;
    public GameObject nodePrefab;
    public RectTransform emptyNodeRect;
    public RectTransform realNodeRect;


    // GameData Load, Save, NewGame, Undo, Redo
    //public SingleMode game_mode;
    //public SingleGameDataManager game_data;


    // 
    public int target_num;
    public int curr_score;
    public int best_score;
    public int high_block;


    //
    public bool isReloading;
 //   public GameData game_data;
    public StateData state_data;

    public string path_gameMode;
    public string path_gameData;
    public string path_stateData;



    // Touch Event
    public Vector2 vectorS = new Vector2();
    public Vector2 vectorE = new Vector2();
    public Vector2 vectorM = new Vector2();






    private void Awake()
    {
        Load_GameData();
        ThemeRoad();
        //CreateGameBoard();
    }

    public void OnApplicationQuit()
    {
        //SaveGame();
        //SaveToJson();
    }

    private void Start() { }





    //============================== Button Object Event ==============================//
    public void ReturnPrevPageButton()
    {
        //SaveGame();
        //SaveToJson();
        SceneManager.LoadScene("Scene_SinglePlay");
    }
    public void ReturnHomePageButton()
    {
        //SaveGame();
        //SaveToJson();
        SceneManager.LoadScene("Scene_Home");
    }
    public void NewGameButton()
    {
        //ClearGame();
        //SaveToJson();
        //isReloading = true;
        SceneManager.LoadScene("Scene_SingleGame");
    }
    public void UndoButton()
    {
        //UndoGame();
        //SaveToJson();
        //isReloading = true;
        //SceneManager.LoadScene("Scene_SingleGame");
    }
    public void RedoButton()
    {
        //RedoGame();
        //SaveToJson();
        //isReloading = true;
        //SceneManager.LoadScene("Scene_SingleGame");
    }



    //============================== Game Data Method ==============================//
    private void Load_GameData()
    {
        // TODO: target number, state count

        isReloading = false;

        // 저장된 데이터 불러오기
        var game_mode = SingleMode.Get_Mode();
        var game_data = SingleGameDataManager.Read();

        curr_score = game_data.curr_score;
        best_score = game_data.best_score;
        high_block = game_data.high_block;
        Create_GameBoard(game_data);


        // 테마 로드
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.Get_Image("GameBoard_Single" + "_" + game_mode.name);
        GameObject.Find("Button_NewGame").GetComponent<Image>().sprite = Theme.Get_Image("Button_NewGame" + "_" + game_mode.name);
        GameObject.Find("Button_Undo").GetComponent<Image>().sprite = Theme.Get_Image("Button_Undo" + "_" + game_mode.name);
        GameObject.Find("Button_Redo").GetComponent<Image>().sprite = Theme.Get_Image("Button_Redo" + "_" + game_mode.name);
        GameObject.Find("Button_Back").GetComponent<Image>().sprite = Theme.Get_Image("Button_Back");
        GameObject.Find("Button_Home").GetComponent<Image>().sprite = Theme.Get_Image("Button_Home");


        // 오브젝트 초기화
        GameObject.Find("Textbox_CurrScore").GetComponent<TextMeshProUGUI>().text = game_data.curr_score.ToString();
        GameObject.Find("Textbox_BestScore").GetComponent<TextMeshProUGUI>().text = game_data.best_score.ToString();
        GameObject.Find("Textbox_TargetNumber").GetComponent<TextMeshProUGUI>().text = new List<string> { "2048", "Infinity", "2048" }[(int)game_mode.index];
    }

    private void ThemeRoad()
    {

    }
    private void Save_Game()
    {
        SingleGameDataManager g_data = new SingleGameDataManager();

        g_data.curr_score = this.curr_score;
        g_data.high_block = this.high_block;
        foreach (var node in this.node_list)
            g_data.block_list.Add(new Block { value = node.value, point = new Vector2Int(node.point.x, node.point.y) });

        SingleGameDataManager.Write(g_data);


        // TODO: DB 저장 필요


     //   game_data.nodeData.Clear();
     //   foreach (var node in nodeData) game_data.nodeData.Add(new NodeClone(node));

    }
    private void Reset_Game()
    {
        SingleGameDataManager.Write(new SingleGameDataManager());

        //   game_data.clear();
        //   state_data.clear();
    }
    private void UndoGame()
    {
     //   game_data = state_data.Undo();
    }
    private void RedoGame()
    {
     //   game_data = state_data.Redo();
    }
    private void SaveToJson()
    {
     //   File.WriteAllText(path_gameData, game_data.GetJson());
    //    File.WriteAllText(path_stateData, state_data.GetJson());
    }
    private void ClearJson()
    {
      //  File.WriteAllText(path_gameData, null);
       // File.WriteAllText(path_stateData, null);
    }





    //============================== Make Game Board ==============================//
    /*private void NewBoard()
    {
        var emptyChildCount = emptyNodeRect.transform.childCount;
        for (int i = 0; i < emptyChildCount; i++) { var child = emptyNodeRect.GetChild(i); }

        // and, empty node create for get grid point
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
                node_list.Add(node);
                instantiatePrefab.name = node.point.ToString();
                this.node_map.Add(point, node);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(emptyNodeRect);
        foreach (var data in node_list)
            data.position = data.nodeRectObj.GetComponent<RectTransform>().localPosition;

        CreateRandom();
    }

    private void CreateGameBoard()
    {
        bool exsitSaveFile = game_data.nodeData.Count == 0 ? false : true;

        // first initialize Score Board 
        //GameObject.Find("ModeName").GetComponent<TextMeshProUGUI>().text = new Dictionary<string, string> { { "ClassicMode", "Classic Mode" }, { "ChallengeMode", "Challenge Mode" }, { "PracticeMode", "Practice Mode" } }[gameMode.modeName];
       // GameObject.Find("TargetNumber").GetComponent<TextMeshProUGUI>().text = new Dictionary<int, string> { { 2048, "2048" }, { 1073741824, "Infinity" } }[game_data.targetBlockNumber];
        GameObject.Find("CurrScore").GetComponent<TextMeshProUGUI>().text = game_data.currScore.ToString();
        GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>().text = game_data.highScore.ToString();

        // first initialize empty Node rect
        real_node_list.Clear();
        node_map.Clear();
        node_list.Clear();

        if (exsitSaveFile == false) NewBoard();      
        else
        {
            var emptyChildCount = emptyNodeRect.transform.childCount;
            for (int i = 0; i < emptyChildCount; i++) { var child = emptyNodeRect.GetChild(i); }

            // and, empty node create for get grid point
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

                    NodeClone nodeClone = game_data.nodeData[i * 4 + j];
                    node.value = nodeClone.value == -1 ? null : nodeClone.value;
                    //node.combined = nodeClone.combined;

                    node_list.Add(node);
                    instantiatePrefab.name = node.point.ToString();
                    this.node_map.Add(point, node);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(emptyNodeRect);  
            foreach (var data in node_list)
                data.position = data.nodeRectObj.GetComponent<RectTransform>().localPosition;

            foreach (var data in game_data.nodeData)
                if (data.value >= 2) CreateBlock(data.point.x, data.point.y, data.value);
        }

        Save_Game();

        if (state_data.Empty()) state_data.AddState(game_data.Copy());
    }*/


    // ************************************************** //
    private void Create_GameBoard(SingleGameDataManager game_data)
    {
      /*  bool exsitSaveFile = game_data.nodeData.Count == 0 ? false : true;

        // first initialize empty Node rect 
        real_node_list.Clear();
        node_map.Clear();
        node_list.Clear();

        if (exsitSaveFile == false) NewBoard();
        else
        {
            var emptyChildCount = emptyNodeRect.transform.childCount;
            for (int i = 0; i < emptyChildCount; i++) { var child = emptyNodeRect.GetChild(i); }

            // and, empty node create for get grid point
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

                    NodeClone nodeClone = game_data.nodeData[i * 4 + j];
                    node.value = nodeClone.value == -1 ? null : nodeClone.value;
                    //node.combined = nodeClone.combined;

                    node_list.Add(node);
                    instantiatePrefab.name = node.point.ToString();
                    this.node_map.Add(point, node);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(emptyNodeRect);
            foreach (var data in node_list)
                data.position = data.nodeRectObj.GetComponent<RectTransform>().localPosition;

            foreach (var data in game_data.nodeData)
                if (data.value >= 2) CreateBlock(data.point.x, data.point.y, data.value);
        }

        Save_Game();

        if (state_data.Empty()) state_data.AddState(game_data.Copy());*/
    }


    private bool IsValid(Vector2Int point)
    {
        if (point.x == -1 || point.x == row || point.y == col || point.y == -1)
            return false;

        return true;
    }
    private void CreateBlock(int x, int y, int? blockNum = null)
    {
        if (node_map[new Vector2Int(x, y)].realNodeObj != null) return;

        GameObject realNodeObj = Instantiate(nodePrefab, realNodeRect.transform, false);
        var node = node_map[new Vector2Int(x, y)];
        var pos = node.position;
        realNodeObj.GetComponent<RectTransform>().localPosition = pos;
        realNodeObj.transform.DOPunchScale(new Vector3(.32f, .32f, .32f), 0.15f, 3);
        var nodeObj = realNodeObj.GetComponent<NodeObject>();
        this.real_node_list.Add(nodeObj);

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

      /*  game_data.currScore += to.value.GetValueOrDefault();
        game_data.highScore = Mathf.Max(game_data.highScore, game_data.currScore);
        game_data.highestBlockNumber = Mathf.Max(game_data.highestBlockNumber, to.value.GetValueOrDefault());
        GameObject.Find("CurrScore").GetComponent<TextMeshProUGUI>().text = game_data.currScore.ToString();
        GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>().text = game_data.highScore.ToString();*/
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
                    var node = node_map[new Vector2Int(i, j)];
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
                    var node = node_map[new Vector2Int(i, j)];
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
                    var node = node_map[new Vector2Int(i, j)];
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
                    var node = node_map[new Vector2Int(i, j)];
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
        
        foreach (var data in real_node_list)
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
        node_list.ForEach(x =>
        { 
            for (int i = 0; i < x.linkedNode.Length; i++)
            {
                if (x.realNodeObj == null) gameOver = false;
                if (x.linkedNode[i] == null)
                    continue;
                
                var nearNode = node_map[x.linkedNode[i].Value];
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
        var emptys = node_list.FindAll(x => x.realNodeObj == null); 
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
        foreach (var data in real_node_list)
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
                foreach (var data in real_node_list) 
                    if (data.needDestroy)  
                        removeTarget.Add(data);
                
                removeTarget.ForEach(x =>
                {
                    real_node_list.Remove(x);
                    GameObject.Destroy(x.gameObject);
                });
                state = State.END;
            }
        }

        if (state == State.END)
        {
            node_list.ForEach(x => x.combined = false);
            state = State.WAIT;
            CreateRandom();

            //--- State Save For UndoRedo ---//
            Save_Game();        
           // state_data.AddState(game_data);
           // if (game_data.highestBlockNumber >= game_data.targetBlockNumber) { OnGameEnd(); }
        }
    }

    private void Show()
    {
        string v = null;
        for (int i = col-1; i >= 0; i--)
        {
            for (int j = 0; j < row; j++)
            {
                var p = node_map[new Vector2Int(j, i)].value;
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
 

 