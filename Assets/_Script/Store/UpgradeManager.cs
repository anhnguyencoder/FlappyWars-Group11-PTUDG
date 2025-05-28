using UnityEngine;

public class UpgradeManager : MonoBehaviour {
    public static UpgradeManager Instance;

    [Header("Upgrade Cumulative Values")]
    public int additionalPlayerHealth = 0;
    public float additionalBulletSizeX2Duration = 0f;
    public float additionalBulletSizeX3Duration = 0f;
    public float additionalShieldDuration = 0f;
    public int additionalHealAmount = 0;

    void Awake() {
        Instance = this;
        
    }
}