using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject enemyPrefab; // Một Prefab duy nhất cho Enemy
    public Transform spawnPoint; // Điểm sinh Enemy

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnEnemy();
    }

    public void EnemyKilled(EnemyType enemyType)
    {
        PlayerController.Instance.SetShootingStyle(enemyType); // Cập nhật style bắn cho Player
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned in the GameManager!");
            return;
        }

        // Tạo Enemy mới
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, quaternion.identity);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyController == null)
        {
            Debug.LogError("Spawned enemy does not have an EnemyController script!");
            return;
        }

        // Random loại bắn cho enemy
        enemyController.enemyType = (EnemyType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);
        Debug.Log($"Spawned enemy with type: {enemyController.enemyType}");
    }
}