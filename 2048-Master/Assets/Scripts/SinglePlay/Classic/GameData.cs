using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool fixedState = false;
    public int currScore = 0;
    public int highScore = 0;
    public List<NodeClone> nodeData = new List<NodeClone>();

    public string GetJson() => JsonUtility.ToJson(this, true);  

    public void clear()
    {
        fixedState = false;
        currScore = 0;
        nodeData = new List<NodeClone>();
    }
    public void clearAll()
    {
        highScore = 0;
        clear();
    }

    public GameData Copy()
    {
        GameData temp = new GameData();

        temp.fixedState = fixedState;
        temp.currScore = currScore;
        temp.highScore = highScore;
        foreach (NodeClone e in nodeData) temp.nodeData.Add(e.Copy());

        return temp;
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