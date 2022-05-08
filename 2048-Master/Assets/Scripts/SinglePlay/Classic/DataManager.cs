using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.IO;
using System.Text;


public class DataManager : MonoBehaviour
{
    // ---�̱������� ����--- 
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

    // --- ���� ������ �����̸� ���� ---
    public string GameDataFileName = "StarfishData.json";

    // "���ϴ� �̸�(����).json"
    public GameData _gameData;
    public GameData gameData
    {
        get
        {
            // ������ ���۵Ǹ� �ڵ����� ����ǵ���
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

    // ����� ���� �ҷ�����
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        // ����� ������ �ִٸ�
        if (File.Exists(filePath))
        {
            print("�ҷ����� ����");
            string FromJsonData = File.ReadAllText(filePath);
            _gameData = JsonUtility.FromJson<GameData>(FromJsonData);
        }

        // ����� ������ ���ٸ�
        else
        {
            print("���ο� ���� ����");
            _gameData = new GameData();
        }
    }

    // ���� �����ϱ�
    public void SaveGameData()
    {
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + GameDataFileName;

        // �̹� ����� ������ �ִٸ� �����
        File.WriteAllText(filePath, ToJsonData);

        // �ùٸ��� ����ƴ��� Ȯ�� (�����Ӱ� ����)
        print("����Ϸ�");
        print("1) " + gameData.currScore);
        print("2) " + gameData.maxScore);
    }

    // ������ �����ϸ� �ڵ�����ǵ���
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
