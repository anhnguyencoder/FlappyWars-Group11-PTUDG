using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string storeSceneName = "StoreScene";  // Scene preload (không hiển thị)
    public string menuScene = "MenuScene";        // Scene chính hiển thị sau Loading

    [Header("Loading UI")]
    public Slider loadingSlider;    // Slider hiển thị % load
    public TextMeshProUGUI percentageText; // Text hiển thị phần trăm load
    public float loadDuration = 2f; // Thời gian slider chạy từ 0 đến 100%
    public float waitAfterLoad = 0.5f; // Thời gian chờ sau khi đạt 100%

    private AsyncOperation asyncOperation;

    void Start()
    {
        StartCoroutine(LoadSceneWithFixedTime());
    }

    IEnumerator LoadSceneWithFixedTime()
    {
        // Load scene additively nhưng không cho phép kích hoạt ngay
        asyncOperation = SceneManager.LoadSceneAsync(storeSceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;

        // Cập nhật slider và phần trăm từ 0 đến 100% trong loadDuration giây
        float timer = 0f;
        while (timer < loadDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / loadDuration);
            if (loadingSlider != null)
                loadingSlider.value = progress;
            if (percentageText != null)
                percentageText.text = (progress * 100f).ToString("F0") + "%";
            yield return null;
        }
        // Đảm bảo slider đạt 100%
        if (loadingSlider != null)
            loadingSlider.value = 1f;
        if (percentageText != null)
            percentageText.text = "100%";

        // Chờ thêm 0.5 giây sau khi load xong
        yield return new WaitForSeconds(waitAfterLoad);

        // Cho phép kích hoạt scene đã load (StoreScene) nếu cần
        asyncOperation.allowSceneActivation = true;

        // Chuyển sang MenuScene
        SceneManager.LoadScene(menuScene);
    }
}
