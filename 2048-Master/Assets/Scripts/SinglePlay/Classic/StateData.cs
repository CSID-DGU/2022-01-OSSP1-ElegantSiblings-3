using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateData
{
    public int maxStateNum = 1;
    public List<GameData> mainState = new List<GameData>();
    public List<GameData> subState = new List<GameData>();

    public string GetJson() => JsonUtility.ToJson(this, true);

    public void clear()
    {
        mainState = new List<GameData>();
        subState = new List<GameData>();
    }

    public void clearAll()
    {
        maxStateNum = 1;
        clear();
    }

    public GameData Undo()
    {
        int mainSize = mainState.Count;

        if (mainSize >= 2)
        {
            subState.Add(mainState[mainSize - 1]);
            mainState.RemoveAt(mainSize - 1);
        }

        return mainState[mainState.Count - 1].Copy();
    }

    public GameData Redo()
    {
        int subSize = subState.Count;

        if (subSize >= 1)
        {
            mainState.Add(subState[subSize - 1]);
            subState.RemoveAt(subSize - 1);
        }

        return mainState[mainState.Count - 1].Copy();
    }

    public void AddState(GameData newState)
    {
        mainState.Add(newState.Copy());
        mainState[mainState.Count - 1].fixedState = true;

        if (mainState.Count > maxStateNum + 1) mainState.RemoveAt(0);
        subState = new List<GameData>();
    }
}
