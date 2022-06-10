using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class SingleBoard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum State
    {
        WAIT, PROCESSING, END
    }

    public State state = State.WAIT;

    public static SingleBoard Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<SingleBoard>();
            return _inst;
        }
    }



    // Create Board and Node
    private static SingleBoard _inst;
    public List<NodeObject> realNodeList = new List<NodeObject>();
    public List<Node> nodeList = new List<Node>();
    public Dictionary<Vector2Int, Node> nodeMap = new Dictionary<Vector2Int, Node>();
    public int col = 4;
    public int row = 4;
    public GameObject emptyNodePrefab;
    public GameObject nodePrefab;
    public RectTransform emptyNodeRect;
    public RectTransform realNodeRect;

    // Game Data
    public bool gameStart = false;
    public int targetNumber;
    public int currentScore;
    public int highestScore;
    public int highestBlock;
    public int exp = 0;

    // Touch Event
    public Vector2 vectorS = new Vector2();
    public Vector2 vectorE = new Vector2();
    public Vector2 vectorM = new Vector2();



    private void Awake()
    {
        StartCoroutine(LoadGameManager());
    }

    private IEnumerator LoadGameManager()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        while (SingleGameManager.Instance == null || !SingleGameManager.Instance.isReady)
        {
            Debug.Log("Not Ready");
            yield return wait;
        }

        LoadGame();
    }

    public void OnApplicationQuit() 
    {
        SingleGameManager.Instance.SaveGame();
    }



    //============================== Button Event ==============================//
    public void Button_Back_Click()
    {
        if (gameStart)
        {
            SingleGameManager.Instance.SaveGame();
            SceneManager.LoadScene("Scene_SinglePlay");
        }
    }
    public void Button_Home_Click()
    {
        if (gameStart)
        {
            SingleGameManager.Instance.SaveGame();
            SceneManager.LoadScene("Scene_Home");
        }
    }
    public void Button_NewGame_Click()
    {
        if (gameStart)
        {
            SingleGameManager.Instance.ClearGame();
            SceneManager.LoadScene("Scene_SingleGame");
        }
    }
    public void Button_Undo_Click()
    {
        if (gameStart && SingleGameManager.Instance.UndoGame())
        {
            SceneManager.LoadScene("Scene_SingleGame");
        }
    }
    public void Button_Redo_Click()
    {
        if (gameStart && SingleGameManager.Instance.RedoGame())
        {
            SceneManager.LoadScene("Scene_SingleGame");
        }
    }
    public void Button_GameEnd_Click()
    {
        if (!gameStart)
        {
            SingleGameManager.Instance.ClearGame();
            SceneManager.LoadScene("Scene_SinglePlay");
        }
    }


    //============================== Game Data Method ==============================//
    private void LoadGame()
    {
        gameStart = false;


        // load saved game data
        var mode = SingleGameModeManager.Instance;
        var gameState = SingleGameManager.Instance.LoadGame();


        targetNumber = new List<int> { 2048, 1073741824, 32 }[(int)mode.GetMode()];
        currentScore = gameState.currentScore;
        highestScore = SingleGameManager.Instance.highestScore;
        highestBlock = gameState.highestBlock;
        exp = SingleGameManager.Instance.exp;


        // load theme
        GameObject.Find("BackGround").GetComponent<Image>().sprite = Theme.GetImage("GameBoard_Single" + "_" + mode.GetModeName());
        GameObject.Find("Button_NewGame").GetComponent<Image>().sprite = Theme.GetImage("Button_NewGame" + "_" + mode.GetModeName());
        GameObject.Find("Button_Undo").GetComponent<Image>().sprite = Theme.GetImage("Button_Undo" + "_" + mode.GetModeName());
        GameObject.Find("Button_Redo").GetComponent<Image>().sprite = Theme.GetImage("Button_Redo" + "_" + mode.GetModeName());
        GameObject.Find("Button_Back").GetComponent<Image>().sprite = Theme.GetImage("Button_Back");
        GameObject.Find("Button_Home").GetComponent<Image>().sprite = Theme.GetImage("Button_Home");


        // initialize score screen 
        GameObject.Find("Textbox_HighestScore").SetActive(mode.GetMode() == SingleGameModeManager.MODE.CHALLENGE ? true : false);      
        GameObject.Find("Textbox_TargetNumber").GetComponent<TextMeshProUGUI>().text = new List<string> { "2048", "Infinity", "32" }[(int)mode.GetMode()];
        GameObject.Find("Message_Result").GetComponent<Image>().gameObject.SetActive(false);
        GameObject.Find("Button_GameEnd").GetComponent<Image>().gameObject.SetActive(false);
       

        // 게임 보드 생성
        CreateGameBoard(gameState);
        UpdateScoreBoard(currentScore, highestScore);
    }

    private void UpdateScoreBoard(int currentScore, int highestScore)
    {
        GameObject.Find("Textbox_CurrentScore").GetComponent<TextMeshProUGUI>().text = currentScore.ToString();
        if (SingleGameModeManager.Instance.GetMode() == SingleGameModeManager.MODE.CHALLENGE) 
            GameObject.Find("Textbox_HighestScore").GetComponent<TextMeshProUGUI>().text = highestScore.ToString();       
    }



    //============================== Create Game Board ==============================//

    private void CreateGameBoard(SingleGameManager.GameState gameState)
    {
        realNodeList.Clear();
        nodeMap.Clear();
        nodeList.Clear();

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
                node.value = gameState.Empty() ? null : gameState.blockList[i * 4 + j].GetValue();
                nodeList.Add(node);
                instantiatePrefab.name = node.point.ToString();
                this.nodeMap.Add(point, node);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(emptyNodeRect);
        foreach (var node in nodeList)
            node.position = node.nodeRectObj.GetComponent<RectTransform>().localPosition;

        if (gameState.Empty())
        {
            FirstBlock();
            SingleGameManager.Instance.UpdateGame();
        }
        else StartCoroutine(LoadSavedBlock(gameState));
    }

    private void FirstBlock()
    {
        CreateRandomBlock();
        gameStart = true;      
    }

    private IEnumerator LoadSavedBlock(SingleGameManager.GameState gameState)
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);

        foreach (var block in gameState.blockList)
        {
            if (block.GetValue() >= 2)
            {
                while (true)
                {
                    if (nodeMap[block.GetPoint()].realNodeObj == null)
                    {
                        CreateBlock(block.GetPoint().x, block.GetPoint().y, block.GetValue());
                        break;
                    }

                    yield return wait;
                }
            }
        }

        gameStart = true;
    }

    private bool IsValid(Vector2Int point)
    {
        return (point.x == -1 || point.x == row || point.y == col || point.y == -1) ? false : true;
    }



    //============================== Create Block ==============================//
    private void CreateBlock(int x, int y, int? block_number = null)
    {
        if (nodeMap[new Vector2Int(x, y)].realNodeObj != null) return;

        GameObject realNodeObj = Instantiate(nodePrefab, realNodeRect.transform, false);
        var node = nodeMap[new Vector2Int(x, y)];
        var pos = node.position;
        realNodeObj.GetComponent<RectTransform>().localPosition = pos;
        realNodeObj.transform.DOPunchScale(new Vector3(.32f, .32f, .32f), 0.15f, 3);
        var nodeObj = realNodeObj.GetComponent<NodeObject>();
        this.realNodeList.Add(nodeObj);

        if (block_number == null) nodeObj.InitializeFirstValue();
        else nodeObj.InitializeSpecificValue(block_number.GetValueOrDefault());

        node.value = nodeObj.value;
        node.realNodeObj = nodeObj;
    }

    private void CreateRandomBlock()
    {
        var emptys = nodeList.FindAll(x => x.realNodeObj == null);
        if (emptys.Count == 0)
        {
            CheckGameState();
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, emptys.Count);
            var pt = emptys[rand].point;
            CreateBlock(pt.x, pt.y);
        }
    }



    //============================== Block Movement ==============================//
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

        int newBlockValue = to.value.GetValueOrDefault();
        currentScore += newBlockValue;
        highestScore = Mathf.Max(highestScore, currentScore);
        highestBlock = Mathf.Max(highestBlock, newBlockValue);

        // 테스트용으로 8마다 경험치 획득
        if (newBlockValue == 8) exp++;

        UpdateScoreBoard(currentScore, highestScore);
        CheckGameState();
    }

    public void Move(Node from, Node to)
    {
        to.value = from.value;
        from.value = null;  
        if (from.realNodeObj != null)
        {
            from.realNodeObj.MoveToNode(from, to);
            if (from.realNodeObj != null)
            {
                to.realNodeObj = from.realNodeObj;
                from.realNodeObj = null;
            }
        }
    }

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

        CheckGameState();
    }



    //============================== 게임 종료 조건 ==============================//

    private void CheckGameState()
    {
        bool isGameEnd = IsGameEnd();
        bool isGameOver = IsGameOver();

        if (isGameEnd || isGameOver)
        {
            gameStart = false;
            GameObject.Find("BackGround").transform.Find("Message_Result").gameObject.SetActive(true);
            GameObject.Find("BackGround").transform.Find("Button_GameEnd").gameObject.SetActive(true);

            if (isGameEnd)  // game clear
            {
                GameObject.Find("Message_Result").GetComponent<Image>().sprite = Theme.GetImage("Scene_SingleGame_Message_GameClear" + "_" + SingleGameModeManager.Instance.GetModeName());
                GameObject.Find("Button_GameEnd").GetComponent<Image>().sprite = Theme.GetImage("Scene_SingleGame_Button_GameEnd" + "_" + SingleGameModeManager.Instance.GetModeName());
            }
            else if (isGameOver)  // game over
            {
                GameObject.Find("Message_Result").GetComponent<Image>().sprite = Theme.GetImage("Scene_SingleGame_Message_GameOver" + "_" + SingleGameModeManager.Instance.GetModeName());
                GameObject.Find("Button_GameEnd").GetComponent<Image>().sprite = Theme.GetImage("Scene_SingleGame_Button_GameEnd" + "_" + SingleGameModeManager.Instance.GetModeName());
            }
        }
    }

    public bool IsGameOver()
    {
        bool gameOver = true;
        nodeList.ForEach(x =>
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

    public bool IsGameEnd()
    {
        return highestBlock >= targetNumber ? true : false;
    }



    //---------------------------------------------------------------------//
    //------------------------------- Update ------------------------------//
    //---------------------------------------------------------------------//

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
            nodeList.ForEach(x => x.combined = false);  
            CreateRandomBlock();

            SingleGameManager.Instance.UpdateGame();
            state = State.WAIT;
        }
    }

    private void Update()
    {
        StartCoroutine(LazyUpdate());
    }

    private IEnumerator LazyUpdate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        if (gameStart)
        {
            UpdateState();
            UpdateByKeyboard();
            UpdateByTouchscreen();
        }

        yield return wait;
    }


    //============================== Touch Event ==============================//

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

    private void UpdateByKeyboard()
    {
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
    }

    private void UpdateByTouchscreen()
    {
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
                }

                TouchGameBoard = false;
            }
        }
    }
}
 

 