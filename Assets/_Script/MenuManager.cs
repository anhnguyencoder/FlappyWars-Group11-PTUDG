using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button startGameButton;
    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;
    void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Chuyển sang Game Scene
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1; // Bắt đầu game
        
    }
}
