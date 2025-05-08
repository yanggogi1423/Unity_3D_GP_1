using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// public class GameManager : Singleton<GameManager>
public class GameManager : MonoBehaviour
{
    [Header("Spawn Points")] public List<Transform> points = new List<Transform>();

    [Header("Monster Prefab")]
    public GameObject monsterPrefab;
    public List<GameObject> monsterPool = new List<GameObject>();
    public int maxMonster = 10;
    private int curMonster;

    //  Game Handling
    private bool isGameOver;
    private bool isPlaying;
    
    //  Game Score
    public int totalScore;
    public TMP_Text scoreText;

    #region SingleTon
    private static GameManager _instance;
    public static GameManager Instance { 
        get
        {
            if (_instance == null)
            {
                //  존재하는 지 확인한다.
                _instance = FindObjectOfType<GameManager>();
                if (_instance != null) return _instance;

                //  존재하지 않는다면 생성한다.
                _instance = new GameManager().AddComponent<GameManager>();
                _instance.name = "GameMgr";
            }

            return _instance;
        }
    }
    
    private void Awake()
    {
        //  존재하지 않는다면 this를 GM으로
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            //  존재하지만 this가 아니라면 destroy
            Destroy(gameObject);
        }
    }
    #endregion

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }
    
    public bool IsPlaying
    {
        get { return isPlaying; }
        set
        {
            isPlaying = value;
            if (isPlaying) Time.timeScale = 1f;
            else Time.timeScale = 0f;
        }
    }
    
    private void Start()
    {
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup").transform;
        
        Transform [] groupChildren = spawnPointGroup.GetComponentsInChildren<Transform>();

        foreach (var point in groupChildren)
        {
            points.Add(point);
        }
        
        //  Init Status Values
        IsGameOver = false;
        IsPlaying = false;   //  Default : false이어야 함 (현재는 테스트 시 true)
        
        //  Regard to Monster Spawn
        curMonster = 0;
        
        //  Score Reset
        totalScore = 0;
        
        SetMonsterPool();
        
        InvokeRepeating("SpawnMonster", 3f, 0.5f);
    }

    private void SetMonsterPool()
    {
        for (int i = 0; i < maxMonster; i++)
        {
            var _monster = Instantiate<GameObject>(monsterPrefab);
            _monster.name = "Monster(" + i + ")";
            _monster.SetActive(false);
            monsterPool.Add(_monster);
        }
    }

    public GameObject GetMonsterInPool()
    {
        for (int i = 0; i < monsterPool.Count; i++)
        {
            if (monsterPool[i].activeSelf == false)
            {
                return monsterPool[i];
            }
        }

        return null;
    }

    public void MonsterDisable()
    {
        curMonster--;
    }

    private void SpawnMonster()
    {
        Debug.Log("Spawn monster");
        if (IsGameOver)
        {
            Debug.Log("Cannot create monster.");
            CancelInvoke("SpawnMonster");
            return;
        }
        
        int index = Random.Range(0, points.Count);
        
        GameObject _monster = GetMonsterInPool();

        if (_monster != null && curMonster < maxMonster)
        {
            _monster.transform.SetPositionAndRotation(points[index].position, points[index].rotation);
            _monster.SetActive(true);
            curMonster++;
            
            Debug.Log("Current Monster " + curMonster);
        }
        else
        {
            Debug.Log("All Monster is Full");
        }
    }
    
    //  Score
    public void DisplayScore(int score)
    {
        totalScore += score;
        scoreText.text = $"<color=#00ff00>SCORE :</color>: <color=#ff0000>{totalScore:#,##0}</color>";
    }
}
