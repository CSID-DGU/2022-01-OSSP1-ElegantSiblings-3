using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class DataManager : MonoBehaviour
{
    public class Score
    {
        public int currScore = 0;
        public int maxScore = 0;

        public Score() { }
        public Score(Score score) { currScore = score.currScore; maxScore = score.maxScore; }
    }

    public class ScoreData
    {
        private Score score = new Score();

        public ScoreData()  // LoadSavData
        {

        }

        public Score Update(int? s)
        {
            score.currScore += s.GetValueOrDefault();
            score.maxScore = Mathf.Max(score.maxScore, score.currScore);

            SavScore();

            return new Score(score);
        }

        private void SavScore()
        {

        }

        public Score GetScore() => new Score(score);
    }

    private class BoardData
    {

    }
}
