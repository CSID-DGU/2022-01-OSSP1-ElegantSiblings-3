using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exsits, destroying object!");
            Destroy(this);
        }
    }

  /*  public void SpawnPlayer(int _id, string _userName)
    {
        GameObject _player;

        if (_id == Client.instance.myId)
        {
            _player=Instantiate(localPlayerPrefab)
        }
        else
        {

        }
    }*/
}
