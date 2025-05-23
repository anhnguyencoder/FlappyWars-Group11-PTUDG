using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] enemyPrefabs; // Mảng chứa Prefab của các loại Enemy
    public Transform spawnPoint; // Điểm sinh Enemy
    private int currentEnemyLevel = 1; // Cấp độ hiện tại của Enemy

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
        // Tăng cấp độ mỗi khi tiêu diệt kẻ địch
        currentEnemyLevel++;
        if (currentEnemyLevel > enemyPrefabs.Length)
        {
            currentEnemyLevel = enemyPrefabs.Length; // Giới hạn cấp độ
        }

        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("Enemy Prefabs array is not assigned or is empty in the GameManager!");
            return;
        }

        // Lấy prefab của Enemy
        GameObject enemyPrefab = enemyPrefabs[currentEnemyLevel - 1];
        if (enemyPrefab == null)
        {
            Debug.LogError($"Enemy prefab at index {currentEnemyLevel - 1} is null!");
            return;
        }

        // Tạo Enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, quaternion.identity);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyController == null)
        {
            Debug.LogError("Spawned enemy does not have an EnemyController script!");
            return;
        }

        // Không thay đổi giá trị enemyType - sử dụng giá trị từ prefab
        Debug.Log($"Spawned enemy with type: {enemyController.enemyType}");
    }



    void Update()
    {
        // Logic khác của GameManager (nếu cần)
    }
}