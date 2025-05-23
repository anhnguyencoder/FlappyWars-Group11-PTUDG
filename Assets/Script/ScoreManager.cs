using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;//Singleton để dễ truy cập
    public TextMeshProUGUI scoreText;// Text UI
    private int score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);//đảm bảo chỉ có một ScoreManager
        }
    }

    public void AddScore(int points)
    {
        score += points;// Cộng thêm điểm
        UpdateScoreUI(); // Cập nhật UI
    }
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score; // Hiển thị điểm
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
