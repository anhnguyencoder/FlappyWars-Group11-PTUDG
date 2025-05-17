using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance;
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;

    public Transform spawnPoint;//diem sinh enemy
    private int enemiesKilled = 0;//so luong enemy bi tieu diet
    private int currentEnemyLevel = 1;//level cua enemy hien tai

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SpawnEnemy();
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled % 10 == 0)
        {
            currentEnemyLevel++;
        }

        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        GameObject enemyPrefab;
        switch (currentEnemyLevel)
        {
            case 1:
                enemyPrefab =  enemy1Prefab;
                break;
            case 2:
                enemyPrefab = enemy2Prefab;
                break;
            case 3:
                enemyPrefab = enemy3Prefab;
                break;
            default:
                return;//khong sinh ra neu vuot qua cap 3
            
            
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, quaternion.identity);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.shootInterval = currentEnemyLevel;
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
