
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Node2
{
    /// <summary>
    /// initialize node data, array size must be set '4'
    /// </summary> 
    public Node2(Vector2Int?[] foundedLinkedNode)
    {
        linkedNode = foundedLinkedNode;
    }
    [NonSerialized] public GameObject nodeRectObj;
    [NonSerialized] public NodeObject2 realNodeObj;
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

    public Node2 FindTarget(Dictionary<Vector2Int, Node2> nodeMap, Node2 originalNode, Direction dir, Node2 farNode = null)
    {
        if (linkedNode[(int)dir].HasValue == true)
        {  
            var dirNode = nodeMap[linkedNode[(int)dir].Value];
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
            return dirNode.FindTarget(nodeMap, originalNode, dir, dirNode);
        }
        return farNode;
    } 
}


[System.Serializable]
public class Node2Clone
{
    public int value = -1;
    public Vector2Int point;

    public Node2Clone() { }
    public Node2Clone(Node2 node)
    {
        this.value = node.value == null ? -1 : node.value.GetValueOrDefault();
        this.point = new Vector2Int(node.point.x, node.point.y);
    }

    public Node2Clone Copy() => new Node2Clone { value = this.value, point = new Vector2Int(this.point.x, this.point.y) };
}