using DG.Tweening;
using GameNetwork;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board_Player : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum State
    {
        WAIT, PROCESSING, END
    }

    public State state = State.WAIT;

    public static Board_Player Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<Board_Player>();
            return _inst;
        }
    }


    private static Board_Player _inst;
    public int col = 4;
    public int row = 4;


    // Create Board and Node
    public List<NodeObject2> realNodeList = new List<NodeObject2>();
    public List<Node2> nodeData = new List<Node2>();
    public Dictionary<Vector2Int, Node2> nodeMap = new Dictionary<Vector2Int, Node2>();

    public GameObject emptyNodePrefab;
    public GameObject nodePrefab;
    public RectTransform emptyNodeRect;
    public RectTransform realNodeRect;


    // 데이터 통신
    public SendGameEvent snedGameEvent;
    public bool gameStart = true;
    public int currentScore;
    public int highestBlock;


    // Touch Event
    public Vector2 vectorS = new Vector2();
    public Vector2 vectorE = new Vector2();
    public Vector2 vectorM = new Vector2();


    private void Awake()
    {
        ThemeRoad();
        CreateGameBoard();
    }


    //============================== Button Object Event ==============================//
    public void Button_GiveUp_Click()
    {
        if (gameStart)
        {
            GameObject.Find("BackGround").transform.Find("Messagebox_GiveUp").gameObject.SetActive(true);
            GameObject.Find("BackGround").transform.Find("Button_GiveUp_Yes").gameObject.SetActive(true);
            GameObject.Find("BackGround").transform.Find("Button_GiveUp_No").gameObject.SetActive(true);
        }
    }

    public void Button_GiveUp_Yes_Click()
    {
        GameObject.Find("Messagebox_GiveUp").GetComponent<Image>().gameObject.SetActive(false);
        GameObject.Find("Button_GiveUp_Yes").GetComponent<Button>().gameObject.SetActive(false);
        GameObject.Find("Button_GiveUp_No").GetComponent<Button>().gameObject.SetActive(false);
        if (gameStart)
        {
            snedGameEvent.GiveUp();
        }
    }

    public void Button_GiveUp_No_Click()
    {
        GameObject.Find("Messagebox_GiveUp").GetComponent<Image>().gameObject.SetActive(false);
        GameObject.Find("Button_GiveUp_Yes").GetComponent<Button>().gameObject.SetActive(false);
        GameObject.Find("Button_GiveUp_No").GetComponent<Button>().gameObject.SetActive(false);
    }

    //============================== Game Data Method ==============================//

    private void ThemeRoad()
    {

    }


    //============================== Send Game Data ==============================//

    public class SendGameEvent
    {
        private BattleRoom battle_room;

        public SendGameEvent(BattleRoom _battle_room) { battle_room = _battle_room; }

        public void ModifiedScore(int curr, int highest)
        {
            Packet msg = Packet.Create((short)PROTOCOL.MODIFIED_SCORE);
            msg.Push(curr);
            msg.Push(highest);
            battle_room.OnSend(msg);
        }

        public void MovedNode(Node2.Direction dir)
        {
            Packet msg = Packet.Create((short)PROTOCOL.MOVED_NODE);
            msg.Push((int)dir);
            battle_room.OnSend(msg);
        }

        public void CreateRandomNode(Vector2Int loc)
        {
            Packet msg = Packet.Create((short)PROTOCOL.CREATED_NEW_NODE);
            msg.Push((int)loc.x);
            msg.Push((int)loc.y);
            battle_room.OnSend(msg);
        }

        public void GiveUp()
        {
            Packet msg = Packet.Create((short)PROTOCOL.GIVE_UP_GAME);
            battle_room.OnSend(msg);
        }
    }


    //============================== Update Screen ==============================//
    private void UpdateScoreScreen()
    {
        GameObject.Find("CurrScore_Player").GetComponent<TextMeshProUGUI>().text = currentScore.ToString();
        GameObject.Find("HighestNodeValue_Player").GetComponent<TextMeshProUGUI>().text = highestBlock.ToString();

        Color color = new Color(1f, 0.42f, 0.42f);

        switch (highestBlock)
        {
            case 2:
                color = Color.black;
                break;
            case 4:
                color = Color.black;
                break;
            case 8:
                color = Color.black;
                break;
            case 16:
                color = Color.black;
                break;
            case 32:
                color = Color.black;
                break;
            case 64:
                color = Color.black;
                break;
            case 128:
                color = Color.black;
                break;
            case 256:
                color = new Color(237f / 255f, 125f / 255f, 49f / 255f);
                break;
            case 512:
                color = new Color(236f / 255f, 77f / 255f, 50f / 255f);
                break;
            case 1024:
                color = Color.red;
                break;
            case 2048:
                color = Color.red;
                break;
            default:
                color = Color.black;
                break;
        }

        GameObject.Find("HighestNodeValue_Player").GetComponent<TextMeshProUGUI>().color = color;
    }



    //============================== Make Game Board ==============================//

    private void CreateGameBoard()
    {
        gameStart = false;

        GameObject.Find("Messagebox_GiveUp").GetComponent<Image>().gameObject.SetActive(false);
        GameObject.Find("Button_GiveUp_Yes").GetComponent<Button>().gameObject.SetActive(false);
        GameObject.Find("Button_GiveUp_No").GetComponent<Button>().gameObject.SetActive(false);

        /* 게임 데이터 송/수신 */
        snedGameEvent = new SendGameEvent(GameObject.Find("BattleRoom").GetComponent<BattleRoom>());
        currentScore = 0;
        highestBlock = 2;
        UpdateScoreScreen();    


        /* initialize empty Node rect */
        realNodeList.Clear();
        nodeMap.Clear();
        nodeData.Clear();

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

                Node2 node = new Node2(v);
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
    }

    public void OnGameStart()
    {
        CreateRandom();
    }


    //============================== Method ==============================//

    private bool IsValid(Vector2Int point)
    {
        if (point.x == -1 || point.x == row || point.y == col || point.y == -1)
            return false;

        return true;
    }
    private void CreateBlock(int x, int y, int? blockNum = null)
    {
        Vector2Int loc = new Vector2Int(x, y);

        if (nodeMap[loc].realNodeObj != null) return;

        // 생성된 Node 위치 Send
        snedGameEvent.CreateRandomNode(loc);

        GameObject realNodeObj = Instantiate(nodePrefab, realNodeRect.transform, false);
        var node = nodeMap[loc];
        var pos = node.position;
        realNodeObj.GetComponent<RectTransform>().localPosition = pos;
        realNodeObj.transform.DOPunchScale(new Vector3(.32f, .32f, .32f), 0.15f, 3);
        var nodeObj = realNodeObj.GetComponent<NodeObject2>();
        this.realNodeList.Add(nodeObj);

        if (blockNum == null) nodeObj.InitializeFirstValue();
        else nodeObj.InitializeSavedValue(blockNum.GetValueOrDefault());

        node.value = nodeObj.value;
        node.realNodeObj = nodeObj;
    }



    //============================== Game Rule ==============================//
    public void Combine(Node2 from, Node2 to)
    {
        to.value = to.value * 2;

        from.value = null;
        if (from.realNodeObj != null)
        {
            from.realNodeObj.CombineToNode(from, to);
            from.realNodeObj = null;
            to.combined = true;
        }

        // TODO: Update Score
        currentScore += to.value.GetValueOrDefault();
        highestBlock = Mathf.Max(highestBlock, to.value.GetValueOrDefault());
        snedGameEvent.ModifiedScore(currentScore, highestBlock);
        UpdateScoreScreen();
    }



    public void Move(Node2 from, Node2 to)
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

    public void MoveTo(Node2.Direction dir)
    {
        snedGameEvent.MovedNode(dir);

        if (dir == Node2.Direction.RIGHT)
        {
            for (int j = 0; j < col; j++)
            {
                for (int i = (row - 2); i >= 0; i--)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;
                    var right = node.FindTarget(nodeMap, node, Node2.Direction.RIGHT);
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
        if (dir == Node2.Direction.LEFT)
        {
            for (int j = 0; j < col; j++)
            {
                for (int i = 1; i < row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;

                    var left = node.FindTarget(nodeMap, node, Node2.Direction.LEFT);
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
        if (dir == Node2.Direction.UP)
        {
            for (int j = col - 2; j >= 0; j--)
            {
                for (int i = 0; i < row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;
                    var up = node.FindTarget(nodeMap, node, Node2.Direction.UP);
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
        if (dir == Node2.Direction.DOWN)
        {
            for (int j = 1; j < col; j++)
            {
                for (int i = 0; i < row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;
                    var down = node.FindTarget(nodeMap, node, Node2.Direction.DOWN);
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

        if (IsGameOver())
        {
            // TODO: GameOver Event
        }
    }

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
                if (x.value != null && nearNode.value != null)
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
                // TODO: GameOver Event
            }
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, emptys.Count);
            var pt = emptys[rand].point;
            CreateBlock(pt.x, pt.y);
        }
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
                var removed = new List<NodeObject2>();
                List<NodeObject2> removeTarget = new List<NodeObject2>();
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
        }
    }


    //---------------------------------------------------------------------//
    //---------------------------- Touch Event ----------------------------//
    //---------------------------------------------------------------------//

    private void Update()
    {
        if (gameStart)
        {
            UpdateState();
            UpdateByKeyboard();
            UpdateByTouchscreen();
        }
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
        if (state == State.WAIT)
        {
            if (Input.anyKeyDown == true)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTo(Node2.Direction.RIGHT);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTo(Node2.Direction.LEFT);
                if (Input.GetKeyDown(KeyCode.UpArrow)) MoveTo(Node2.Direction.UP);
                if (Input.GetKeyDown(KeyCode.DownArrow)) MoveTo(Node2.Direction.DOWN);
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
                        if (vectorM.x > 0) MoveTo(Node2.Direction.RIGHT);
                        else MoveTo(Node2.Direction.LEFT);
                    }
                    else if (Mathf.Abs(vectorM.x) < Mathf.Abs(vectorM.y))
                    {
                        if (vectorM.y > 0) MoveTo(Node2.Direction.UP);
                        else MoveTo(Node2.Direction.DOWN);
                    }
                }

                TouchGameBoard = false;
            }
        }
    }
}