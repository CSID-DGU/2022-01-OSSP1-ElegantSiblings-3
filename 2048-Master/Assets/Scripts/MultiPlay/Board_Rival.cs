using DG.Tweening;
using GameNetwork;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board_Rival : MonoBehaviour
{
    public enum State
    {
        WAIT, PROCESSING, END
    }

    public State state = State.WAIT;

    public static Board_Rival Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<Board_Rival>();
            return _inst;
        }
    }


    private static Board_Rival _inst;
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


    // ������ ���
    public RecvGameEvent recvGameEvent;
    public bool firstLoad;
    public int currentScore;
    public int highestBlock;


    // Touch Event
    public Vector2 vectorS = new Vector2();
    public Vector2 vectorE = new Vector2();
    public Vector2 vectorM = new Vector2();


    private void Awake()
    {
        CreateGameBoard();
    }

    public void OnApplicationQuit() { }

    private void Start() { }



    //============================== Send Game Data ==============================//
    public class RecvGameEvent
    {
        private enum GameEvent : int
        {
            FIRST_LOAD, MOVE_BLOCK, CHANGED_BLOCK_STATE, CREATE_BLOCK // 0, 1, 2, 3
        }

        private GameEvent prev_event;
        private Queue<int?> moved_direction;
        private Queue<Vector2Int?> created_node_location;

        public RecvGameEvent()
        {
            prev_event = GameEvent.FIRST_LOAD;
            moved_direction = new Queue<int?>();
            created_node_location = new Queue<Vector2Int?>();
        }

        public void Receive_MovedDirection(Packet msg)
        {
            moved_direction.Enqueue(msg.PopInt32());
        }

        public void Receive_CreatedNodeLocation(Packet msg)
        {
            created_node_location.Enqueue(new Vector2Int(msg.PopInt32(), msg.PopInt32()));
        }

        public int? MovedDirection()
        {
            if ((prev_event == GameEvent.MOVE_BLOCK || prev_event == GameEvent.CREATE_BLOCK) && moved_direction.Count > 0)
            {
                Debug.Log("Move Node..." + " (prev event is " + prev_event + ")");
                int? dir = moved_direction.Dequeue();
                prev_event = GameEvent.MOVE_BLOCK;
                return dir;
            }
            return null;
        }

        public void ChangedBlockState()
        {
            prev_event = GameEvent.CHANGED_BLOCK_STATE;
        }

        public Vector2Int? CreatedNodeLocation()
        {
            if ((prev_event == GameEvent.FIRST_LOAD || prev_event == GameEvent.CHANGED_BLOCK_STATE) && created_node_location.Count > 0)
            {
                Debug.Log("Create Node..." + " (prev event is " + prev_event + ")");
                Vector2Int? loc = created_node_location.Dequeue();
                prev_event = GameEvent.CREATE_BLOCK;
                return loc;
            }
            return null;
        }
    }


    //============================== Update Screen ==============================//
    private void Update_Score_Screen()
    {
        GameObject.Find("CurrScore_Rival").GetComponent<TextMeshProUGUI>().text = currentScore.ToString();
        GameObject.Find("HighestNodeValue_Rival").GetComponent<TextMeshProUGUI>().text = highestBlock.ToString();

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

        GameObject.Find("HighestNodeValue_Rival").GetComponent<TextMeshProUGUI>().color = color;
    }


    //============================== Make Game Board ==============================//
    private void CreateGameBoard()
    {
        /* first initialize Score Board */
        recvGameEvent = new RecvGameEvent();
        firstLoad = true;
        currentScore = 0;
        highestBlock = 2;
        Update_Score_Screen();


        /* first initialize empty Node rect */
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

        //CreateRandom();
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

        // Update Score
        currentScore += to.value.GetValueOrDefault();
        highestBlock = Mathf.Max(highestBlock, to.value.GetValueOrDefault());
        Update_Score_Screen();
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
                recvGameEvent.ChangedBlockState();
                state = State.PROCESSING;
                data.StartMoveAnimation();
            }
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


        if (state == State.END || firstLoad == true)
        {
            Vector2Int? loc = recvGameEvent.CreatedNodeLocation();

            if (loc != null)
            {
                nodeData.ForEach(x => x.combined = false);
                firstLoad = false;
                state = State.WAIT;
                CreateBlock_By_Event(loc.GetValueOrDefault());
            }
        }
    }


    private void CreateBlock_By_Event(Vector2Int loc)
    {
        // TODO: Board 1P���� ���ӿ����� ���¿����� �����͸� ������ �� �ȴ�
        if (nodeMap[loc].realNodeObj == null)
        {
            CreateBlock(loc.x, loc.y);
        }
    }


    // Update
    private void Update()
    {
        UpdateState();
        Move_By_Receive_Event();
    }

    private void Move_By_Receive_Event()
    {
        if(state == State.WAIT)
        {
            int? dir = recvGameEvent.MovedDirection();

            if (dir != null)
            {
                if (dir.GetValueOrDefault() == 0) MoveTo(Node2.Direction.RIGHT);
                if (dir.GetValueOrDefault() == 2) MoveTo(Node2.Direction.LEFT);
                if (dir.GetValueOrDefault() == 3) MoveTo(Node2.Direction.UP);
                if (dir.GetValueOrDefault() == 1) MoveTo(Node2.Direction.DOWN);
            }
        }
    }
}