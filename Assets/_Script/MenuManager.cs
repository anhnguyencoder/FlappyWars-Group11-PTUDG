using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button startGameButton;
    public Button storeButton; // Nút mở Store
    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;
    void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        storeButton.onClick.AddListener(OpenStore);
        AudioManager.Instance.PlayRandomMenuMusic();

    }

    void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Chuyển sang Game Scene
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1; // Bắt đầu game
        AudioManager.Instance.PlayRandomMenuMusic();

    }
    public void OpenStore()
    {
        SceneManager.LoadScene("StoreScene"); // Chuyển sang Store Scene
    }
}
