using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum UpgradeType {
    IncreasePlayerHealth,
    IncreaseBulletSizeX2Duration,
    IncreaseBulletSizeX3Duration,
    IncreaseShieldDuration,
    IncreaseHealAmount
}

public class UpgradeRow : MonoBehaviour {
    [Header("Upgrade Identification")]
    public UpgradeType upgradeType;// Loại upgrade (cấu hình trong Inspector)
    public Image icon;                // Icon đại diện cho upgrade
    public TextMeshProUGUI upgradeNameText;  // Tên của upgrade (ví dụ: "Tăng máu")
    public TextMeshProUGUI costText;    // Hiển thị giá upgrade hiện tại
    public Button upgradeButton;      // Nút mua upgrade
    public GameObject[] upgradeCogs;  // Mảng gồm 10 cọc, ẩn ban đầu

    [Header("Upgrade Configuration")]
    public int currentLevel = 0; // Từ 0 đến 10 (10 cấp)
    public int[] upgradeCosts;  // Mảng giá nâng cấp cho từng cấp (cần có 10 phần tử)
    public float[] additionalValues; // Giá trị nâng cấp cho mỗi cấp
    public TextMeshProUGUI statText;  // Text hiển thị chỉ số nâng cấp
    public GameConfig config; // Gán từ Inspector hoặc load từ Resources
    
    private void Start()
    {
        UpdateCogsDisplay();
        UpdateStatText();

    }

    public void UpdateRow(int playerGold) {
        if (currentLevel < upgradeCosts.Length) {
            costText.text = upgradeCosts[currentLevel] + " Gold";
            bool canBuy = playerGold >= upgradeCosts[currentLevel];
            upgradeButton.interactable = canBuy;
            // Cập nhật màu sắc của nút: màu xanh nếu có đủ, màu xám nếu không
            upgradeButton.GetComponent<Image>().color = canBuy ? Color.green : Color.gray;
        } else {
            costText.text = "Max";
            upgradeButton.interactable = false;
            upgradeButton.GetComponent<Image>().color = Color.gray;
        }
    }

    public int GetUpgradeCost() {
        if (currentLevel < upgradeCosts.Length)
            return upgradeCosts[currentLevel];
        else 
            return int.MaxValue;
    }

    public void Upgrade() {
        if (currentLevel < upgradeCosts.Length) {
            
            
            
            // Lưu giá trị nâng cấp theo cấp hiện tại (trước khi tăng currentLevel)
            float valueToAdd = additionalValues[currentLevel];

            ApplyUpgradeEffect(valueToAdd);
            currentLevel++;
            // Cập nhật lại số cọc hiển thị theo currentLevel
            UpdateCogsDisplay();
            // Cập nhật lại text chỉ số
            UpdateStatText();
        }
    }

    void ApplyUpgradeEffect(float valueToAdd) {
        switch (upgradeType) {
            case UpgradeType.IncreasePlayerHealth:
                // Tăng maxHealth của người chơi
                UpgradeManager.Instance.additionalPlayerHealth += (int)valueToAdd;
              
                break;
            case UpgradeType.IncreaseBulletSizeX2Duration:
                UpgradeManager.Instance.additionalBulletSizeX2Duration += valueToAdd;
                break;
            case UpgradeType.IncreaseBulletSizeX3Duration:
                UpgradeManager.Instance.additionalBulletSizeX3Duration += valueToAdd;
                break;
            case UpgradeType.IncreaseShieldDuration:
                UpgradeManager.Instance.additionalShieldDuration += valueToAdd;
                break;
            case UpgradeType.IncreaseHealAmount:
                UpgradeManager.Instance.additionalHealAmount += (int)valueToAdd;
                break;
        }
    }
    // Hàm này sẽ được gọi khi nhấn nút Upgrade (gán qua Inspector)
    public void OnUpgradeButtonClick() {
        StoreManager.Instance.BuyUpgrade(this);
    }
    public void UpdateCogsDisplay() {
        for (int i = 0; i < upgradeCogs.Length; i++) {
            // Chỉ hiển thị cọc nếu chỉ số i nhỏ hơn currentLevel
            upgradeCogs[i].SetActive(i < currentLevel);
        }
    }
    void UpdateStatText() {
        switch (upgradeType) {
            case UpgradeType.IncreasePlayerHealth:
                float totalPlayerHealth = config.basePlayerHealth + UpgradeManager.Instance.additionalPlayerHealth;
                statText.text = "Health: " + totalPlayerHealth.ToString("F1");
                break;
            case UpgradeType.IncreaseBulletSizeX2Duration:
                float totalBulletSizeX2Duration = config.baseBulletSizeX2Duration + UpgradeManager.Instance.additionalBulletSizeX2Duration;
                statText.text = "Duration: " + totalBulletSizeX2Duration.ToString("F1") + "s";
                break;
            case UpgradeType.IncreaseBulletSizeX3Duration:
                float totalBulletSizeX3Duration = config.baseBulletSizeX3Duration + UpgradeManager.Instance.additionalBulletSizeX3Duration;
                statText.text = "Duration: " + totalBulletSizeX3Duration.ToString("F1") + "s";
                break;
            case UpgradeType.IncreaseShieldDuration:
                float totalShield = config.baseShieldDuration + UpgradeManager.Instance.additionalShieldDuration;
                statText.text = "Duration: " + totalShield.ToString("F1") + "s";
                break;
            case UpgradeType.IncreaseHealAmount:
                float totalHealAmount = config.baseHealAmount + UpgradeManager.Instance.additionalHealAmount;
                statText.text = "Heal Amount: " + totalHealAmount.ToString("F1");
                break;
        }
    }


}
