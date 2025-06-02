using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial Panels")]
    public GameObject jumpTutorialPanel;  // Panel hướng dẫn nhảy
    public GameObject shootTutorialPanel; // Panel hướng dẫn bắn

    [Header("Jump Tutorial - Animations & Text")]
    public GameObject jumpAnim_PC;       // Animation cho PC
    public GameObject jumpAnim_Mobile;   // Animation cho Mobile
    public TextMeshProUGUI jumpTutorialText;  // Text hướng dẫn nhảy

    [Header("Shoot Tutorial - Animations & Text")]
    public GameObject shootAnim_PC;      // Animation cho PC
    public GameObject shootAnim_Mobile;  // Animation cho Mobile
    public TextMeshProUGUI shootTutorialText; // Text hướng dẫn bắn

    [Header("Timing Settings")]
    public float initialDelay = 0.5f;   // Delay sau khi vào game trước khi hiển thị tutorial
    public float resumeDuration = 1f;   // Thời gian game chạy sau khi nhận input, trước khi tạm dừng để hiển thị bước tiếp theo

    private bool tutorialCompleted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Giữ đối tượng này qua các scene nếu cần:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Kiểm tra trạng thái tutorial từ PlayerPrefs
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            tutorialCompleted = true;
            jumpTutorialPanel.SetActive(false);
            shootTutorialPanel.SetActive(false);
        }
        else
        {
            StartCoroutine(RunTutorial());
        }
    }

    IEnumerator RunTutorial()
    {
        // Đợi 0.5 giây sau khi game bắt đầu
        yield return new WaitForSecondsRealtime(initialDelay);
        
        // Tạm dừng game
        Time.timeScale = 0;
        
        // ---- Hướng dẫn nhảy ----
        jumpTutorialPanel.SetActive(true);
        if (Application.isMobilePlatform)
        {
            // Mobile: hiển thị animation dành cho mobile và text
            if (jumpAnim_Mobile != null) jumpAnim_Mobile.SetActive(true);
            if (jumpAnim_PC != null) jumpAnim_PC.SetActive(false);
            if (jumpTutorialText != null) jumpTutorialText.text = "Tap left to jump";
        }
        else
        {
            // PC: hiển thị animation dành cho PC và text
            if (jumpAnim_PC != null) jumpAnim_PC.SetActive(true);
            if (jumpAnim_Mobile != null) jumpAnim_Mobile.SetActive(false);
            if (jumpTutorialText != null) jumpTutorialText.text = "Press SPACE to jump";
        }
        // Chờ input nhảy
        yield return StartCoroutine(WaitForJumpInput());
        
        // Ẩn panel hướng dẫn nhảy
        jumpTutorialPanel.SetActive(false);
        
        // Cho game chạy 1 giây
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(resumeDuration);
        
        // Tạm dừng game lại
        Time.timeScale = 0;
        
        // ---- Hướng dẫn bắn ----
        shootTutorialPanel.SetActive(true);
        if (Application.isMobilePlatform)
        {
            if (shootAnim_Mobile != null) shootAnim_Mobile.SetActive(true);
            if (shootAnim_PC != null) shootAnim_PC.SetActive(false);
            if (shootTutorialText != null) shootTutorialText.text = "Tap right to shoot";
        }
        else
        {
            if (shootAnim_PC != null) shootAnim_PC.SetActive(true);
            if (shootAnim_Mobile != null) shootAnim_Mobile.SetActive(false);
            if (shootTutorialText != null) shootTutorialText.text = "Click anywhere to shoot";
        }
        // Chờ input bắn
        yield return StartCoroutine(WaitForShootInput());
        
        // Ẩn panel hướng dẫn bắn
        shootTutorialPanel.SetActive(false);
        
        // Tiếp tục game như bình thường
        Time.timeScale = 1;
        
        // Đánh dấu tutorial đã hoàn thành
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        tutorialCompleted = true;
    }

    IEnumerator WaitForJumpInput()
    {
        bool jumpReceived = false;
        while (!jumpReceived)
        {
            if (Application.isMobilePlatform)
            {
                // Mobile: chờ touch ở nửa bên trái
                if (Input.touchCount > 0)
                {
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2)
                        {
                            jumpReceived = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                // PC: chờ nhấn SPACE
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumpReceived = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator WaitForShootInput()
    {
        bool shootReceived = false;
        while (!shootReceived)
        {
            if (Application.isMobilePlatform)
            {
                // Mobile: chờ touch ở nửa bên phải
                if (Input.touchCount > 0)
                {
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.phase == TouchPhase.Began && touch.position.x >= Screen.width / 2)
                        {
                            shootReceived = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                // PC: chờ nhấn chuột (với PC, bất kỳ click nào cũng được, không phân biệt vị trí)
                if (Input.GetMouseButtonDown(0))
                {
                    shootReceived = true;
                }
            }
            yield return null;
        }
    }
}
