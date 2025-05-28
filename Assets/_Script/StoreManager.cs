using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour {
    public static StoreManager Instance;

    [Header("Player Resources")]
    public int playerGold = 1000;  // Số vàng ban đầu (có thể được lấy từ GameManager hoặc lưu trong PlayerPrefs)
    public TextMeshProUGUI goldText;          // Text hiển thị số vàng hiện tại

    [Header("Upgrade Rows")]
    public UpgradeRow[] upgradeRows;  // Liên kết các UpgradeRow (được kéo thả từ ScrollView)

    void Awake() {
        Instance = this;
    }

    void Start() {
        UpdateGoldUI();
        foreach (var row in upgradeRows) {
            row.UpdateRow(playerGold);
        }
    }

    public void BuyUpgrade(UpgradeRow row) {
        int cost = row.GetUpgradeCost();
        if (playerGold >= cost) {
            playerGold -= cost;
            row.Upgrade();
            UpdateGoldUI();
        }
    }

    void UpdateGoldUI() {
        if (goldText != null)
            goldText.text = "Gold: " + playerGold;
    }
}