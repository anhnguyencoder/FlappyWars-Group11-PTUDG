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
    
    [Header("Enemy Health Upgrade")]
    public int totalEnemiesKilled = 0;
    public int currentEnemyMaxHealth = 2; // Enemy đầu tiên có maxHealth = 2
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnEnemy();
        AudioManager.Instance.PlayRandomGameplayMusic();

    }

    public void EnemyKilled(EnemyType enemyType)
    {
        totalEnemiesKilled++;
        // Mỗi khi player tiêu diệt được 3 enemy, tăng max health của enemy mới sinh lên 1
        if (totalEnemiesKilled % 3 == 0)
        {
            currentEnemyMaxHealth++;
            Debug.Log("Enemy upgraded! New max enemy health: " + currentEnemyMaxHealth);
        }
        // Tính toán số gold thưởng dựa theo số enemy đã bị tiêu diệt
        // Enemy thứ n: thưởng random từ n*10 đến n*10 + 10
        int minGold = totalEnemiesKilled * 10;
        int maxGold = totalEnemiesKilled * 10 + 10; // Random.Range(int, int) với max là exclusive, nên dùng maxGold + 1
        int goldReward = UnityEngine.Random.Range(minGold, maxGold + 1);
        Debug.Log("Awarded gold: " + goldReward);
        PlayerData.gold += goldReward;
        PlayerPrefs.SetInt("PlayerGold", PlayerData.gold);
        PlayerPrefs.Save();
    
        // Nếu UIManager đã khởi tạo thì cập nhật lại GoldText trên UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGoldText();
            UIManager.Instance.ShowGoldReward(goldReward);
            // Giả sử vị trí của enemy là currentEnemy.transform.position
            if (currentEnemy != null && UIManager.Instance.goldText != null && UIManager.Instance.goldFlyEffectPrefab != null)
            {
                UIManager.Instance.ShowGoldFlyEffects(currentEnemy.transform.position);
            }
        }
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