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
    // Các giá trị hiệu quả nâng cấp (ví dụ: tăng thêm máu, thời gian Power-Up...) có thể thêm ở đây

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
            // Hiển thị cọc nâng cấp: bật icon tương ứng
            if (upgradeCogs != null && upgradeCogs.Length > currentLevel)
                upgradeCogs[currentLevel].SetActive(true);
            // Lưu giá trị nâng cấp theo cấp hiện tại (trước khi tăng currentLevel)
            float valueToAdd = additionalValues[currentLevel];

            ApplyUpgradeEffect(valueToAdd);
            currentLevel++;
         
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

}
