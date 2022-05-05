using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private int currScore = 0;
    private int maxScore = 0;

    public void AddScore(int points)
    {
        currScore += points;
    }

    void Update()
    {
      //  if(gameObject.CompareTag())
    }

    public void CreatedBlock(Collider other)
    {
       // if(other.gameObject.CompareTag())
    }








    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
