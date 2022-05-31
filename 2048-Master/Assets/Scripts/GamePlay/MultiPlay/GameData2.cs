using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData2
{
    public int curr_score { get; set; }
    public int highest_block_value { get; set; }

    public int move_direction { get; set; }
    public Coordinate2D new_node_loc { get; set; }

    public GameData2() 
    {
        curr_score = 0;
        highest_block_value = 0;
        move_direction = -1;
        new_node_loc = new Coordinate2D();
    }

    //public List<Node2Clone> node2Clone = new List<Node2Clone>();
}

[System.Serializable]
public class Coordinate2D
{
    public int x { get; set; }
    public int y { get; set; }

    public Coordinate2D() { x = -1; y = -1; }
}

public static class GameData2Extensions
{
    public static string Serializer(this GameData2 gameData) => JsonUtility.ToJson(gameData);
}