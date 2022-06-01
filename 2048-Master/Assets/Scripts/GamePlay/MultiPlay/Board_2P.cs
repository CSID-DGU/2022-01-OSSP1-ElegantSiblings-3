using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;
//
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;
using FreeNet;

public class Board_2P : MonoBehaviour
{
    public enum State
    {
        WAIT, PROCESSING, END
    }

    public State state = State.WAIT;

    public static Board_2P Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<Board_2P>();
            return _inst;
        }
    }


    private static Board_2P _inst;
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
    public RecvGameEvent recv_game_event;
    public bool first_load;
    public int curr_score;
    public int highest_node_value;


    // Touch Event
    public Vector2 vectorS = new Vector2();
    public Vector2 vectorE = new Vector2();
    public Vector2 vectorM = new Vector2();


    private void Awake()
    {
        ThemeRoad();
        CreateGameBoard();
    }

    public void OnApplicationQuit()
    {

    }

    private void Start() { }





    //============================== Button Object Event ==============================//
    public void ReturnPrevPageButton()
    {
        //   SaveGame();
        // SaveToJson();
        //   SceneManager.LoadScene("SinglePlayPage");
    }


    //============================== Game Data Method ==============================//

    private void ThemeRoad()
    {

    }


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

        public void Receive_Moved_Direction(CPacket msg)
        {
            moved_direction.Enqueue(msg.pop_int32());
        }

        public void Receive_Created_Node_Location(CPacket msg)
        {
            created_node_location.Enqueue(new Vector2Int(msg.pop_int32(), msg.pop_int32()));
        }

        public int? Moved_Direction()
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

        public void Changed_Block_State()
        {
            prev_event = GameEvent.CHANGED_BLOCK_STATE;
        }

        public Vector2Int? Created_Node_Location()
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
        GameObject.Find("CurrScore_Rival").GetComponent<TextMeshProUGUI>().text = curr_score.ToString();
        GameObject.Find("HighestNodeValue_Rival").GetComponent<TextMeshProUGUI>().text = highest_node_value.ToString();

        Color color = new Color(1f, 0.42f, 0.42f);

        switch (highest_node_value)
        {
            case 2:
                color = new Color(0.14f, 0.62f, 1f);
                break;
            case 4:
                color = new Color(0.14f, 0.62f, 1f);
                break;
            case 8:
                color = new Color(1f, 0.45f, 0f);
                break;
            case 16:
                color = new Color(1f, 0.45f, 0f);
                break;
            case 32:
                color = new Color(1f, 0.42f, 0.42f);
                break;
            case 64:
                color = new Color(1f, 0.42f, 0.42f);
                break;
            case 128:
                color = new Color(1f, 0.35f, 0.35f);
                break;
            case 256:
                color = new Color(1f, 0.35f, 0.35f);
                break;
            case 512:
                color = new Color(1f, 0.15f, 0.15f);
                break;
            case 1024:
                color = new Color(1f, 0.15f, 0.15f);
                break;
            case 2048:
                color = new Color(1f, 0, 0);
                break;
            case 4096:
                color = new Color(1f, 0, 0);
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
        recv_game_event = new RecvGameEvent();
        first_load = true;
        curr_score = 0;
        highest_node_value = 2;
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
        curr_score += to.value.GetValueOrDefault();
        highest_node_value = Mathf.Max(highest_node_value, to.value.GetValueOrDefault());
        Update_Score_Screen();
    }

    public void Move(Node2 from, Node2 to)
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
                recv_game_event.Changed_Block_State();
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
                if (x.value != null && nearNode.value != null)
                    if (x.value == nearNode.value)
                    {
                        gameOver = false;
                    }
            }
        });

        return gameOver;
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


        if (state == State.END || first_load == true)
        {
            Vector2Int? loc = recv_game_event.Created_Node_Location();

            if (loc != null)
            {
                nodeData.ForEach(x => x.combined = false);
                first_load = false;
                state = State.WAIT;
                CreateBlock_By_Event(loc.GetValueOrDefault());
            }
        }
    }


    private void CreateBlock_By_Event(Vector2Int loc)
    {
        // TODO: Board 1P에서 게임오버된 상태에서는 데이터를 보내면 안 된다
        if (nodeMap[loc].realNodeObj == null)
        {
            CreateBlock(loc.x, loc.y);
        }
    }


    private void Show()
    {
        string v = null;
        for (int i = col - 1; i >= 0; i--)
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
            int? dir = recv_game_event.Moved_Direction();

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