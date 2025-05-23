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


        // // Giới hạn currentEnemyLevel trong phạm vi hợp lệ
        // currentEnemyLevel = Mathf.Clamp(currentEnemyLevel, 1, enemyPrefabs.Length);

        // Kiểm tra lại sau khi Clamp
        if (currentEnemyLevel - 1 < 0 || currentEnemyLevel - 1 >= enemyPrefabs.Length)
        {
            Debug.LogError("currentEnemyLevel is out of bounds or enemyPrefabs is not set properly!");
            return;
        }
        
        EnemyType type = (EnemyType)(currentEnemyLevel - 1); // Lấy loại Enemy dựa trên cấp độ
        GameObject enemyPrefab = enemyPrefabs[currentEnemyLevel - 1]; // Lấy Prefab tương ứng
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, quaternion.identity);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.enemyType = type; // Gán loại bắn cho Enemy
    }

    void Update()
    {
        // Logic khác của GameManager (nếu cần)
    }
}