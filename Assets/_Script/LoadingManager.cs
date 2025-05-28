using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour {
    [Header("UI Elements")]
    public Slider loadingSlider;
    public Text loadingText;

    [Header("Scene Names")]
    public string storeSceneName = "StoreScene";
    public string menuSceneName = "MenuScene";

    void Start() {
        // Bắt đầu load StoreScene additively (để UpgradeManager được khởi tạo)
        StartCoroutine(LoadStoreScene());
        // Bắt đầu hiển thị loading (ít nhất 3 giây) và chuyển sang MenuScene
        StartCoroutine(ShowLoadingAndTransition());
    }

    IEnumerator LoadStoreScene() {
        // Tải StoreScene theo chế độ Additive
        AsyncOperation op = SceneManager.LoadSceneAsync(storeSceneName, LoadSceneMode.Additive);
        while (!op.isDone) {
            yield return null;
        }
        // StoreScene đã được load. Chờ thêm 2 giây để đảm bảo các script (như UpgradeManager) được khởi tạo
        yield return new WaitForSeconds(2f);
        // Ẩn giao diện của StoreScene (giả sử root UI của StoreScene có tên "StoreCanvas")
        GameObject storeRoot = GameObject.Find("StoreCanvas");
        if (storeRoot != null) {
            storeRoot.SetActive(false);
        }
    }

    IEnumerator ShowLoadingAndTransition() {
        float duration = 3f; // Loading tối thiểu 3 giây
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            loadingSlider.value = progress;
            loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";
            yield return null;
        }
        // Đảm bảo hiển thị 100%
        loadingSlider.value = 1f;
        loadingText.text = "100%";
        yield return new WaitForSeconds(0.1f);
        // Chuyển sang MenuScene
        SceneManager.LoadScene(menuSceneName);
    }
}