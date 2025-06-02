using System.Collections;
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
    
    [Header("Player Gold UI")]
    public TextMeshProUGUI goldText; // GoldText hiển thị trên GameScene
    public TextMeshProUGUI goldRewardText; // Hiển thị số vàng vừa nhận được
    public GameObject goldFlyEffectPrefab;  // Prefab GoldFlyEffect, gán qua Inspector
    
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
   
    
    private int highScore = 0;
    private bool isGameStarted = false;
    [Header("Pause Panel UI")]
    public GameObject pausePanel;              // Panel pause, set inactive ban đầu
    public TextMeshProUGUI pauseScoreText;     // Hiển thị điểm hiện tại trên Pause Panel
    public Button resumeButton;                // Nút chơi tiếp (Resume)
    public Button pauseMenuButton;             // Nút Menu
   

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
        UpdateScoreUI();
        isGameStarted = true;

        Time.timeScale = 1;
        
        
        gameOverPanel.SetActive(false);
        
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        UpdateGoldText();
        // Gán sự kiện cho Pause Panel
        resumeButton.onClick.AddListener(ResumeGame);
        pauseMenuButton.onClick.AddListener(ReturnToMainMenu);
   

        // Đảm bảo PausePanel bị ẩn khi game bắt đầu
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Nếu pausePanel đang hiển thị, thì resume; nếu không, pause.
            if (pausePanel.activeSelf)
                ResumeGame();
            else
                PauseGame();
        }
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
        UpdateScoreUI();
    }

  
    
   
    // Cập nhật UI điểm số
    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
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
    public void UpdateGoldText() {
        if(goldText != null)
            goldText.text = "" + PlayerData.gold;
    }
    // Phương thức hiển thị số vàng thưởng sau mỗi enemy bị tiêu diệt
    public void ShowGoldReward(int reward) {
        if (goldRewardText != null) {
            goldRewardText.text = "+" + reward.ToString();
            goldRewardText.gameObject.SetActive(true);
            // Bắt đầu coroutine ẩn sau 2 giây (có thể điều chỉnh thời gian)
            StartCoroutine(FadeOutRewardText());
        }
        
    }
    private IEnumerator FadeOutRewardText() {
        yield return new WaitForSeconds(2f);
        goldRewardText.gameObject.SetActive(false);
    }
    
    public void ShowGoldFlyEffects(Vector3 enemyWorldPosition)
    {
        // Lấy Canvas chứa Gold Text
        Canvas canvas = goldText.GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        // Lấy RectTransform của Canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Chuyển vị trí enemy từ world sang screen
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(enemyWorldPosition);

        // Chuyển screenPoint thành local position của canvas
        Vector2 baseLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera, out baseLocalPoint);

        // Số lượng đồng xu muốn xuất hiện, ví dụ 8
        int coinCount = 8;
        float spreadRadius = 80f; // bán kính phát xung quanh (có thể điều chỉnh)

        for (int i = 0; i < coinCount; i++)
        {
            // Instantiate prefab đồng xu làm con của Canvas
            GameObject coinObj = Instantiate(goldFlyEffectPrefab, canvas.transform);
            RectTransform coinRect = coinObj.GetComponent<RectTransform>();

            // Tính offset ngẫu nhiên trong vòng tròn bán kính spreadRadius
            Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
            coinRect.anchoredPosition = baseLocalPoint + randomOffset;

            // Gán target của hiệu ứng là RectTransform của Gold Text
            GoldFlyEffectDOTween coinEffect = coinObj.GetComponent<GoldFlyEffectDOTween>();
            coinEffect.target = goldText.GetComponent<RectTransform>();

            
            coinEffect.duration = Random.Range(0.8f, 1.2f);
        }
    }
// --- Các phương thức cho Pause Game ---
    public void PauseGame()
    {
        // Set Time.timeScale = 0 để tạm dừng game
        Time.timeScale = 0;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            // Cập nhật score hiện tại vào pauseScoreText
            if (pauseScoreText != null)
                pauseScoreText.text = "Score: " + score;
        }
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        // Khôi phục Time.timeScale
        Time.timeScale = 1;
    }
    
   

}