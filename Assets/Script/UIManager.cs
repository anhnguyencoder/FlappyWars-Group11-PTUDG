using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton để dễ truy cập

    [Header("CooldownUI")]
    public TextMeshProUGUI cooldownText;
    public Slider cooldownSlider;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;

    private int score = 0; // Điểm số của người chơi

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
        UpdateScoreUI(); // Cập nhật UI điểm số khi bắt đầu
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
        score += points;
        UpdateScoreUI();
    }

    // Cập nhật UI điểm số
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}