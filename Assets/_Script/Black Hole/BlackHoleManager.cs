using System.Collections;
using UnityEngine;

public class BlackHoleManager : MonoBehaviour
{
    [Header("Black Hole Settings")]
    [Tooltip("Prefab của hố đen vũ trụ")]
    public GameObject blackHolePrefab;

    [Tooltip("Danh sách các vị trí spawn cho hố đen")]
    public Transform[] spawnPoints;

    [Header("Background Settings")]
    [Tooltip("Danh sách các background trong game")]
    public GameObject[] backgrounds;

    [Header("Flash Effect Settings")]
    [Tooltip("Đối tượng hiệu ứng lóe sáng")]
    public GameObject flashEffect;

    [Tooltip("Thời gian hiển thị hiệu ứng lóe sáng (giây)")]
    public float flashDuration = 1f;
    
    [Header("Spawn Timing")]
    [Tooltip("Thời gian delay lần xuất hiện đầu tiên (giây)")]
    public float initialSpawnDelay = 15f;
    [Tooltip("Thời gian ngẫu nhiên giữa các lần xuất hiện (giây)")]
    public Vector2 spawnIntervalRange = new Vector2(25f, 45f);

    [Header("Game Settings")]
    [Tooltip("Thời gian dừng game khi chạm hố đen (giây)")]
    public float pauseDuration = 1f;

    private GameObject currentBackground; // Background hiện tại

    void Start()
    {
        // Khởi tạo background hiện tại
        currentBackground = GetActiveBackground();

        // Bắt đầu coroutine spawn Black Hole
        StartCoroutine(SpawnBlackHoles());
    }

    IEnumerator SpawnBlackHoles()
    {
        // Chờ delay lần đầu tiên
        yield return new WaitForSeconds(initialSpawnDelay);

        while (true)
        {
            // Tạo Black Hole
            SpawnBlackHole();

            // Random thời gian delay cho lần tiếp theo
            float nextSpawnTime = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            yield return new WaitForSeconds(nextSpawnTime);
        }
    }

    void SpawnBlackHole()
    {
        if (spawnPoints.Length == 0 || blackHolePrefab == null)
        {
            Debug.LogError("BlackHoleManager: Không có spawn point hoặc prefab được cấu hình.");
            return;
        }

        // Random vị trí spawn hố đen
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(blackHolePrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
    }

    private GameObject GetActiveBackground()
    {
        foreach (GameObject bg in backgrounds)
        {
            if (bg.activeSelf) return bg;
        }
        Debug.LogError("BlackHoleManager: Không tìm thấy background đang hoạt động.");
        return null;
    }

    public void ChangeBackground()
    {
        // Tắt background hiện tại
        if (currentBackground != null)
        {
            currentBackground.SetActive(false);
        }

        // Random background mới không trùng với background hiện tại
        GameObject newBackground;
        do
        {
            int randomIndex = Random.Range(0, backgrounds.Length);
            newBackground = backgrounds[randomIndex];
        } while (newBackground == currentBackground);

        // Kích hoạt background mới
        newBackground.SetActive(true);
        currentBackground = newBackground;
    }

    public void OnBlackHoleTriggered()
    {
        StartCoroutine(HandleBlackHoleEffect());
    }

    private IEnumerator HandleBlackHoleEffect()
    {
        // Tạm dừng game
        Time.timeScale = 0f;
        // Hiển thị hiệu ứng lóe sáng
        ShowFlashEffect();
        // Hiệu ứng lóa sáng (nếu cần thêm hiệu ứng, có thể tích hợp tại đây)
        Debug.Log("Black Hole Triggered: Hiệu ứng lóa sáng bắt đầu.");

        // Chờ trong khoảng thời gian dừng game
        yield return new WaitForSecondsRealtime(pauseDuration);

        // Đổi background
        ChangeBackground();

        // Tiếp tục game
        Time.timeScale = 1f;
    }
    
    private void ShowFlashEffect()
    {
        if (flashEffect != null)
        {
            StartCoroutine(FlashCoroutine());
        }
        else
        {
            Debug.LogWarning("BlackHoleManager: Flash effect object chưa được gán.");
        }
    }

    private IEnumerator FlashCoroutine()
    {
        // Bật hiệu ứng lóe sáng
        flashEffect.SetActive(true);

        // Chờ trong khoảng thời gian flashDuration
        yield return new WaitForSecondsRealtime(flashDuration);

        // Tắt hiệu ứng lóe sáng
        flashEffect.SetActive(false);
    }

}
