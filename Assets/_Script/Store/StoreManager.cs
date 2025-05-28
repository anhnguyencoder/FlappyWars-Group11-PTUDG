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
        LoadUpgrades(); // Tải dữ liệu trước
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
            // Gọi lại để hiển thị giá mới (hoặc "Max" nếu đã hết cấp)
            row.UpdateRow(playerGold);
            // Lưu lại dữ liệu sau khi mua
            SaveUpgrades();
        }
    }

    void UpdateGoldUI() {
        if (goldText != null)
            goldText.text = "Gold: " + playerGold;
    }
    // Hàm xử lý sự kiện chuyển về MenuScene
    public void ReturnToMenu() {
        SceneManager.LoadScene("MenuScene");
    }
    public void SaveUpgrades() {
        // Lưu số vàng
        PlayerPrefs.SetInt("PlayerGold", playerGold);

        // Lưu cấp độ của mỗi upgrade row
        for (int i = 0; i < upgradeRows.Length; i++) {
            PlayerPrefs.SetInt("UpgradeRow_Level_" + i, upgradeRows[i].currentLevel);
        }

        // Lưu giá trị bonus trong UpgradeManager (nếu muốn load lại nhanh)
        PlayerPrefs.SetInt("AdditionalPlayerHealth", UpgradeManager.Instance.additionalPlayerHealth);
        PlayerPrefs.SetFloat("AdditionalBulletSizeX2Duration", UpgradeManager.Instance.additionalBulletSizeX2Duration);
        PlayerPrefs.SetFloat("AdditionalBulletSizeX3Duration", UpgradeManager.Instance.additionalBulletSizeX3Duration);
        PlayerPrefs.SetFloat("AdditionalShieldDuration", UpgradeManager.Instance.additionalShieldDuration);
        PlayerPrefs.SetInt("AdditionalHealAmount", UpgradeManager.Instance.additionalHealAmount);

        PlayerPrefs.Save(); // Ghi xuống ổ cứng
    }
    public void LoadUpgrades() {
        // Lấy số vàng, mặc định 1000 nếu chưa có lưu
        playerGold = PlayerPrefs.GetInt("PlayerGold", 1000);

        // Lấy cấp độ cho mỗi Upgrade Row
        for (int i = 0; i < upgradeRows.Length; i++) {
            // currentLevel = giá trị đã lưu (nếu chưa có thì = 0)
            int savedLevel = PlayerPrefs.GetInt("UpgradeRow_Level_" + i, 0);
            upgradeRows[i].currentLevel = savedLevel;

            // Do đã set currentLevel, cần ApplyEffect cho đúng
            // => Gọi UpgradeRow.ApplyUpgradeEffect() đủ số lần,
            // hoặc gán thẳng bonus vào UpgradeManager.
            // Ở đây, ta gán thẳng bonus vào UpgradeManager 
            // dựa trên savedLevel và additionalValues.
            if (savedLevel > 0) {
                float totalBonus = 0f;
                for (int level = 0; level < savedLevel; level++) {
                    totalBonus += upgradeRows[i].additionalValues[level];
                }
                upgradeRows[i].ApplyUpgradeEffect(totalBonus);
            }
            // Hiển thị cogs dựa theo currentLevel
            upgradeRows[i].UpdateCogsDisplay();
            upgradeRows[i].UpdateStatText(); 
        }

        // Tải giá trị bonus trong UpgradeManager
        UpgradeManager.Instance.additionalPlayerHealth 
            = PlayerPrefs.GetInt("AdditionalPlayerHealth", 0);
        UpgradeManager.Instance.additionalBulletSizeX2Duration 
            = PlayerPrefs.GetFloat("AdditionalBulletSizeX2Duration", 0f);
        UpgradeManager.Instance.additionalBulletSizeX3Duration 
            = PlayerPrefs.GetFloat("AdditionalBulletSizeX3Duration", 0f);
        UpgradeManager.Instance.additionalShieldDuration 
            = PlayerPrefs.GetFloat("AdditionalShieldDuration", 0f);
        UpgradeManager.Instance.additionalHealAmount 
            = PlayerPrefs.GetInt("AdditionalHealAmount", 0);
    }

}