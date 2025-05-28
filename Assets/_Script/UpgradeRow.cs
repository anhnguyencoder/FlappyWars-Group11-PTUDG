using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeRow : MonoBehaviour {
    public Image icon;                // Icon đại diện cho upgrade
    public TextMeshProUGUI upgradeNameText;  // Tên của upgrade (ví dụ: "Tăng máu")
    public TextMeshProUGUI costText;    // Hiển thị giá upgrade hiện tại
    public Button upgradeButton;      // Nút mua upgrade
    public GameObject[] upgradeCogs;  // Danh sách các cọc nâng cấp (tối đa 10)

    [Header("Upgrade Configuration")]
    public int currentLevel = 0;      // Mức nâng cấp hiện tại (0 đến 10)
    public int[] upgradeCosts;        // Giá của từng cấp, ví dụ: upgradeCosts[0] là giá nâng cấp từ 0 lên 1

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
            // Hiển thị cọc nâng cấp: kích hoạt icon cọc tương ứng
            upgradeCogs[currentLevel].SetActive(true);
            currentLevel++;
            // Nếu upgrade này ảnh hưởng đến biến game (ví dụ: tăng maxHealth cho player), hãy gọi hàm cập nhật tương ứng
            ApplyUpgradeEffect();
        }
    }

    void ApplyUpgradeEffect() {
        // Ví dụ: nếu upgrade này là tăng máu cho player
        if (upgradeNameText.text.Contains("Máu")) {
            // Bạn có thể tăng maxHealth, hoặc thay đổi giá trị heal, tùy theo yêu cầu
            PlayerController.Instance.maxHealth += 5;  // tăng thêm 5 máu mỗi lần nâng cấp
            // Cập nhật UI hiển thị máu
            PlayerController.Instance.UpdateHealthUI(PlayerController.Instance.health);
        }
        // Nếu upgrade là thời gian Power-Up, bạn cần cập nhật cấu hình trong PowerUp hoặc UIManager
        // Bạn có thể mở rộng hàm này để áp dụng cho các loại upgrade khác nhau
    }
    public void OnUpgradeButtonClick() {
        StoreManager.Instance.BuyUpgrade(this);
    }

}
