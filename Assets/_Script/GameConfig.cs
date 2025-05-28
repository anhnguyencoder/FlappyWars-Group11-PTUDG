using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config", order = 0)]
public class GameConfig : ScriptableObject
{
    [Header("Player Settings")]
    public int basePlayerHealth = 10;

    [Header("PowerUp Durations")]
    public float baseBulletSizeX2Duration = 5f;
    public float baseBulletSizeX3Duration = 5f;
    public float baseShieldDuration = 6f;

    [Header("Heal Settings")]
    public int baseHealAmount = 1;
}