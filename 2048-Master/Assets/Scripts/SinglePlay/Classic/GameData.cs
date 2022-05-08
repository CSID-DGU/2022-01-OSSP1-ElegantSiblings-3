using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currScore = 0;
    public int maxScore = 0;
    public List<NodeClone> nodeData = new List<NodeClone>();

    public string GetJson() => JsonUtility.ToJson(this, true);  

    public void clear()
    {
        currScore = 0;
        maxScore = 0;
        nodeData = new List<NodeClone>();
    }
}


[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}