using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton để dễ truy cập

    [Header("CooldownUI")]
    public TextMeshProUGUI cooldownText;
    public Slider cooldownSlider;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    
    [Header("Health UI")]
    public TextMeshProUGUI healthText;
    
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public Button restartButton;
    public Button mainMenuButton;
    
 
    [Header("Power-Up UI")]
    public Transform playerPowerUpPanel;  // Panel hiển thị Power-Up của Player
    public Transform enemyPowerUpPanel;   // Panel hiển thị Power-Up của Enemy
    public GameObject powerUpUIPrefab;    // Prefab hiển thị Power-Up

    
    
    private int score = 0; // Điểm số của người chơi
    [Header("Health")]
    public int health = 100;
    public int maxHealth=100;
    
    private int highScore = 0;
    private bool isGameStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
       

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
        isGameStarted = true;

        Time.timeScale = 1;
        
        
        gameOverPanel.SetActive(false);
        
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);

    }
   
    // Cập nhật UI cooldown
    public void UpdateCooldownUI(float currentCooldown, float maxCooldown)
    {
        if (cooldownText != null)
        {
            cooldownText.text = currentCooldown > 0 ? currentCooldown.ToString("F1") + "s" : "Ready";
        }
        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = maxCooldown;
            cooldownSlider.value = maxCooldown - currentCooldown;
        }
    }

    // Tăng điểm và cập nhật UI
    public void AddScore(int points)
    {
        if (!isGameStarted) return;
        score += points;
        if (score % 10 == 0 && health < 3)
        {
            health++;
        }
        UpdateUI();
    }

    public void TakeDamage()
    {
        if (!isGameStarted) return;
        health--;
        UpdateUI();
        if (health <= 0)
        {
            GameOver();
        }
    }
    
    // Cập nhật UI điểm số
    public void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        healthText.text = "Health: " + health;
    }
    
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + score;
        Time.timeScale = 0; // Dừng game khi thua
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        
        highScoreText.text = "High Score:" + highScore;
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
        
        
        isGameStarted = true; // Xác định rằng game đã bắt đầu
    }
    
    public void ReturnToMainMenu()
    {

        Time.timeScale = 1;
        SceneManager.LoadScene("MenuScene");
        //coi như game chưa bắt đầu
        PlayerPrefs.SetInt("IsGameStarted", 0);

    }
    public void AddPowerUpUI(PowerUpType type, bool isPlayer, float duration)
    {
        Transform panel = isPlayer ? playerPowerUpPanel : enemyPowerUpPanel;

        // Tạo UI mới cho Power-Up
        GameObject powerUpUI = Instantiate(powerUpUIPrefab, panel);
        PowerUpUI ui = powerUpUI.GetComponent<PowerUpUI>();
        ui.Setup(type, duration);

        // Xếp theo chiều dọc, Power-Up mới xuất hiện bên dưới
        powerUpUI.transform.SetAsLastSibling();
    }

}