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
        private Board_2P board;

        public RecvGameEvent(Board_2P _board) { board = _board; }

        public void Modified_Score(CPacket msg)
        {
            int curr = msg.pop_int32();
            int highest = msg.pop_int32();
            board.curr_score = curr;
            board.highest_node_value = highest;
            board.Update_Score_Screen();
        }

        public void Moved_Node(CPacket msg)
        {
            int dir = msg.pop_int32();
            if (dir == 0) board.MoveTo(Node2.Direction.RIGHT);
            if (dir == 2) board.MoveTo(Node2.Direction.LEFT);
            if (dir == 3) board.MoveTo(Node2.Direction.UP);
            if (dir == 1) board.MoveTo(Node2.Direction.DOWN);
        }

        public void Create_Random_Node(CPacket msg)
        {
            int x = msg.pop_int32();
            int y = msg.pop_int32();
            board.CreateBlock(x, y);
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
        recv_game_event = new RecvGameEvent(this);
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

        // TODO: Update Score
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
                        else if (right == null) return;
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

        if (state == State.END)
        {
            nodeData.ForEach(x => x.combined = false);
            state = State.WAIT;
            //CreateRandom();

            //--- State Save For UndoRedo ---//
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
}