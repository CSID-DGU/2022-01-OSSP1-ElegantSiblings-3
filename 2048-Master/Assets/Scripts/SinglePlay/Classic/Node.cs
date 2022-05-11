
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class  Node
{
    /// <summary>
    /// initialize node data, array size must be set '4'
    /// </summary> 
    public Node(Vector2Int?[] foundedLinkedNode)
    {
        linkedNode = foundedLinkedNode;
    }
    [NonSerialized] public GameObject nodeRectObj;
    [NonSerialized] public NodeObject realNodeObj;
    public enum Direction
    {
        RIGHT = 0,
        DOWN = 1,
        LEFT = 2,
        UP = 3
    } 

    public int? value = null; 
    public Vector2Int point; 
    public Vector2 position;
    public bool combined = false;
    public Vector2Int?[] linkedNode = null; 

    public Node FindTarget(Node originalNode, Direction dir, Node farNode = null)
    {
        if (linkedNode[(int)dir].HasValue == true)
        {  
            var dirNode = Board.Instance.nodeMap[linkedNode[(int)dir].Value];
            // if already combined, return prev block
            if (dirNode != null && dirNode.combined) 
                return this; 
            // if two value equal return latest finded value.
            if (dirNode.value != null && originalNode.value != null)
            { 
                if (dirNode.value == originalNode.value) 
                    return dirNode; 

                if (dirNode.value != originalNode.value) 
                    return farNode; 
            }  
            return dirNode.FindTarget(originalNode, dir, dirNode);
        }
        return farNode;
    } 
}


[System.Serializable]
public class NodeClone
{
    public int value = -1;
    public Vector2Int point;
    public Vector2 position;
    public bool combined = false;
    public List<LinkedNode> linkedNode = new List<LinkedNode>();

    public NodeClone() { }
    public NodeClone(Node node)
    {
        this.value = node.value == null ? -1 : node.value.GetValueOrDefault();
        this.point = new Vector2Int(node.point.x, node.point.y);
        this.position = new Vector2(node.position.x, node.position.y);
        this.combined = node.combined;
        foreach (var linked in node.linkedNode) linkedNode.Add(linked == null ? new LinkedNode() : new LinkedNode { IsNull = false, link = new Vector2Int(linked.GetValueOrDefault().x, linked.GetValueOrDefault().y) });
        
    }

    public NodeClone Copy()
    {
        NodeClone temp = new NodeClone();

        temp.value = this.value;
        temp.point = new Vector2Int(this.point.x, this.point.y);
        temp.position = new Vector2(this.position.x, this.position.y);
        temp.combined = this.combined;
        foreach (var linked in this.linkedNode) temp.linkedNode.Add(linked.Copy());

        return temp;
    }
}

[System.Serializable]
public class LinkedNode
{
    public bool IsNull = true;
    public Vector2Int link = new Vector2Int();

    public LinkedNode Copy()
    {
        LinkedNode temp = new LinkedNode();

        temp.IsNull = this.IsNull;
        temp.link = new Vector2Int(this.link.x, this.link.y);

        return temp;
    }
}