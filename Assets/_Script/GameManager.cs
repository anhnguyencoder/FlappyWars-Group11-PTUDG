using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject enemyPrefab; // Một Prefab duy nhất cho Enemy
    public Transform spawnPoint; // Điểm sinh Enemy

    private GameObject currentEnemy; // Kẻ địch hiện tại

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

        // Đặt currentEnemy là null để cho phép sinh kẻ địch mới
        currentEnemy = null;

        // Gọi spawn enemy sau một khoảng thời gian delay
        StartCoroutine(SpawnEnemyAfterDelay(1f));
    }

    void SpawnEnemy()
    {
        // Kiểm tra nếu đã có kẻ địch hiện tại
        if (currentEnemy != null) return;

        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned in the GameManager!");
            return;
        }

        // Tạo Enemy mới
        currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, quaternion.identity);
        EnemyController enemyController = currentEnemy.GetComponent<EnemyController>();

        if (enemyController == null)
        {
            Debug.LogError("Spawned enemy does not have an EnemyController script!");
            return;
        }

        // Random loại bắn cho enemy
        enemyController.enemyType = (EnemyType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);
        Debug.Log($"Spawned enemy with type: {enemyController.enemyType}");
    }

    private IEnumerator SpawnEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemy();
    }
    public EnemyController GetCurrentEnemy()
    {
        return FindObjectOfType<EnemyController>();
    }

}