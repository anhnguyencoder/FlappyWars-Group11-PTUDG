using UnityEngine;

public class ThornyScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float startPosition = 10f;  // Vị trí bắt đầu (bên phải)
    public float endPosition = -10f;   // Vị trí kết thúc (bên trái)
    public float scrollSpeed = 2f;     // Tốc độ cuộn

    void Update()
    {
        // Lấy vị trí hiện tại
        Vector3 currentPosition = transform.position;

        // Di chuyển sang trái
        currentPosition.x -= scrollSpeed * Time.deltaTime;

        // Nếu vượt qua điểm kết thúc, quay lại điểm bắt đầu
        if (currentPosition.x <= endPosition)
        {
            currentPosition.x = startPosition;
        }

        // Cập nhật vị trí
        transform.position = currentPosition;
    }
}