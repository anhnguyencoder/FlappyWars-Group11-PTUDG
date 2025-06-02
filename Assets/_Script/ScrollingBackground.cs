using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Tooltip("Tốc độ cuộn của background")]
    public float scrollSpeed = 0.5f;  // Điều chỉnh tốc độ cuộn

    private Renderer rend; // Tham chiếu đến Renderer của object

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogError("ScrollingBackground: Không tìm thấy Renderer trên object!");
    }

    void Update()
    {
        // Tính toán offset dựa trên thời gian
        float offset = Time.time * scrollSpeed;
        // Cập nhật thuộc tính mainTextureOffset của material, chỉ di chuyển theo trục X
        rend.material.mainTextureOffset = new Vector2(offset, 0);
    }
}