using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.IO;
using System.Text;


public class DataManager : MonoBehaviour
{
    // ---싱글톤으로 선언--- 
    static GameObject _container;
    static GameObject Container
    {
        get
        {
            return _container;
        }
    }
    static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (!_instance)
            {
                _container = new GameObject();
                _container.name = "DataController";
                _instance = _container.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(_container);
            }
            return _instance;
        }
    }

    // --- 게임 데이터 파일이름 설정 ---
    public string GameDataFileName = "StarfishData.json";

    // "원하는 이름(영문).json"
    public GameData _gameData;
    public GameData gameData
    {
        get
        {
            // 게임이 시작되면 자동으로 실행되도록
            if (_gameData == null)
            {
                LoadGameData();
                SaveGameData();
            }
            return _gameData;
        }
    }

    private void Start()
    {
        LoadGameData();
        SaveGameData();
    }

    // 저장된 게임 불러오기
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            print("불러오기 성공");
            string FromJsonData = File.ReadAllText(filePath);
            _gameData = JsonUtility.FromJson<GameData>(FromJsonData);
        }

        // 저장된 게임이 없다면
        else
        {
            print("새로운 파일 생성");
            _gameData = new GameData();
        }
    }

    // 게임 저장하기
    public void SaveGameData()
    {
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + GameDataFileName;

        // 이미 저장된 파일이 있다면 덮어쓰기
        File.WriteAllText(filePath, ToJsonData);

        // 올바르게 저장됐는지 확인 (자유롭게 변형)
        print("저장완료");
        print("1) " + gameData.currScore);
        print("2) " + gameData.maxScore);
    }

    // 게임을 종료하면 자동저장되도록
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}


/*public class DataManager : MonoBehaviour
{
    private ScoreData score = new ScoreData();
    private string scorePath = "";

    public DataManager()
    {

    }

    private void Start()
    {
        scorePath = Path.Combine(Application.dataPath, "scoreSav.json");
        LoadScore();
    }

    private void LoadScore()
    {
        ScoreData tempScore = new ScoreData();

        if (!File.Exists(scorePath)) SaveScore();
        else
        {
            string loadJson = File.ReadAllText(scorePath);
            tempScore = JsonUtility.FromJson<ScoreData>(loadJson);

            Debug.Log(tempScore);
        }
    }

    private void SaveScore()
    {
        string jscore = JsonUtility.ToJson(new ScoreData(score));
        File.WriteAllText(scorePath, jscore);
    }




    //

    public ScoreData GetScore() => new ScoreData(score);

    public ScoreData ScoreUpdate(int? s)
    {
        score.currScore += s.GetValueOrDefault();
        score.maxScore = Mathf.Max(score.maxScore, score.currScore);

     //   SaveScore();

        return new ScoreData(score);
    }





    private class BoardData
    {

    }
}*/
