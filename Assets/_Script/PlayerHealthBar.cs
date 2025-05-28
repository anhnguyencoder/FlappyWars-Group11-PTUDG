using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBarFill; // Tham chiếu đến phần Fill của thanh máu
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Đảm bảo thanh máu luôn hướng về camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        healthBarFill.fillAmount = health / maxHealth;
    }
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

}