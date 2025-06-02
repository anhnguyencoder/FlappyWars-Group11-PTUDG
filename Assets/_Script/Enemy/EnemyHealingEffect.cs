using UnityEngine;

public class EnemyHealingEffect : MonoBehaviour
{
    public float duration = 1f;  // Thời gian hiệu ứng hiển thị

    void Start()
    {
        Debug.Log("EnemyHealingEffect started, will be destroyed in " + duration + " seconds.");
        Destroy(gameObject, duration);
    }
}