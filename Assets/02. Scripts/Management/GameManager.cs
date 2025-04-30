using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [Header("Spawn Points")]
    public List<Transform> points = new List<Transform>();
    
    [Header("Monster Prefab")]
    public GameObject monsterPrefab;
    public List<GameObject> monsterPool = new List<GameObject>();
    public int maxMonster = 10;
    
    //  Game Handling
    private bool isGameOver;

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

    private void Awake()
    {
        Transform spawnPointGroup = GameObject.Find("PointGroup").transform;
        
        Transform [] groupChildren = spawnPointGroup.GetComponentsInChildren<Transform>();

        foreach (var point in groupChildren)
        {
            points.Add(point);
        }
        
        SetMonsterPool();
    }
    

    private void Start()
    {
        InvokeRepeating("SpawnMonster", 0, 0.5f);
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

    private void SpawnMonster()
    {
        int index = Random.Range(0, points.Count);
        
        GameObject _monster = GetMonsterInPool();

        if (_monster != null)
        {
            _monster.transform.SetPositionAndRotation(points[index].position, points[index].rotation);
            _monster.SetActive(true);
        }
        else
        {
            Debug.Log("All Monster is Full");
        }

    }
}
