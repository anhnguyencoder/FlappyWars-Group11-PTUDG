using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Tooltip("Danh sách các background GameObject (đã có ScrollingBackground)")]
    public GameObject[] backgrounds;

    void Start()
    {
        if (backgrounds == null || backgrounds.Length == 0)
        {
            Debug.LogWarning("Không có background nào được gán!");
            return;
        }

        // Random chọn một index
        int randomIndex = Random.Range(0, backgrounds.Length);

        // Kích hoạt background được chọn, ẩn các background khác
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].SetActive(i == randomIndex);
        }
    }
}