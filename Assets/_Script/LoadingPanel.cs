using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    public Image loadingFill;               // Image dùng để hiển thị tiến độ (Fill Amount)
    public TextMeshProUGUI progressText;           // Text hiển thị phần trăm
    public float loadingTime = 3f;                 // Thời gian loading (3 giây)

    private static bool hasLoadedOnce = false;    // Kiểm tra xem panel loading đã xuất hiện lần nào chưa

    void Start()
    {
        if (!hasLoadedOnce)
        {
            StartCoroutine(PlayLoading());
            hasLoadedOnce = true;
        }
        else
        {
            // Nếu đã load rồi, ẩn panel luôn
            gameObject.SetActive(false);
        }
    }

    IEnumerator PlayLoading()
    {
        float elapsed = 0f;
        while (elapsed < loadingTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / loadingTime);
            loadingFill.fillAmount = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";
            yield return null;
        }
        // Đảm bảo hiển thị 100%
        loadingFill.fillAmount = 1f;
        progressText.text = "100%";
        yield return new WaitForSeconds(0.5f);

        // Bắt đầu load MenuScene một cách bất đồng bộ
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MenuScene");
        asyncLoad.allowSceneActivation = false;

        // Chờ cho đến khi asyncLoad đạt khoảng 0.9 (thường là 0.9 khi đã load xong)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Cho phép chuyển scene
        asyncLoad.allowSceneActivation = true;

        // Đợi vài giây ngắn để đảm bảo scene mới load xong (hoặc dùng yield return new WaitUntil(() => asyncLoad.isDone);)
        yield return new WaitForSeconds(0.1f);

        // Sau khi chuyển scene, ẩn panel (nếu cần)
        gameObject.SetActive(false);
    }
}
