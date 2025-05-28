using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpUI : MonoBehaviour
{
    public Image icon; // Ảnh đại diện của Power-Up
    public TextMeshProUGUI timerText; // Thời gian còn lại của Power-Up

    public Sprite bulletSizeX2Icon;
    public Sprite bulletSizeX3Icon;
    public Sprite bodySizeX2Icon;
    public Sprite shieldIcon;
    public Sprite healIcon;
    public Sprite freezeIcon;
    public Sprite bombIcon;

    private Dictionary<PowerUpType, Sprite> powerUpIcons;
    private float duration;

    public void Setup(PowerUpType type, float duration)
    {
        this.duration = duration;

      
        // Khởi tạo Dictionary để ánh xạ PowerUpType với Sprite
        powerUpIcons = new Dictionary<PowerUpType, Sprite>
        {
            { PowerUpType.BulletSizeX2, bulletSizeX2Icon },
            { PowerUpType.BulletSizeX3, bulletSizeX3Icon },
            { PowerUpType.BodySizeX2, bodySizeX2Icon },
            { PowerUpType.Shield, shieldIcon },
            { PowerUpType.Heal, healIcon },
            { PowerUpType.Freeze, freezeIcon },
            { PowerUpType.Bomb, bombIcon }
        };

        // Gán icon dựa trên loại Power-Up
        if (powerUpIcons.ContainsKey(type))
        {
            icon.sprite = powerUpIcons[type];
        }

        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        float timeLeft = duration;
        while (timeLeft > 0)
        {
            timerText.text = timeLeft.ToString("F1") + "s";
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;

            // Hiệu ứng mờ dần khi gần hết thời gian
            if (timeLeft < 1f)
            {
                Color c = icon.color;
                c.a = timeLeft;
                icon.color = c;
            }
        }
        Destroy(gameObject); // Xóa UI khi hết thời gian
    }
}