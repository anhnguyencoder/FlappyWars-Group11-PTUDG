using UnityEngine;

public class ThornyScroll : MonoBehaviour
{
    public float speed = -2f; // Tốc độ di chuyển (âm nghĩa là di chuyển từ phải sang trái).
    public float lowerXValue = -20f; // Giới hạn bên trái của vị trí trên trục X.
    public float upperXValue = 40f; // Khoảng dịch chuyển sang phải khi chạm giới hạn trái.

    void Update() {
        transform.Translate(speed * Time.deltaTime, 0f, 0f);
        // Dịch chuyển đối tượng theo trục X với tốc độ đã định nghĩa.
        // Time.deltaTime đảm bảo chuyển động mượt mà dựa trên thời gian thực.

        if (transform.position.x <= lowerXValue) {
            // Kiểm tra xem vị trí hiện tại của đối tượng có vượt quá giới hạn bên trái hay không.
            transform.Translate(upperXValue, 0f, 0f);
            // Nếu có, di chuyển đối tượng sang phải vị trí mới dựa trên giá trị upperXValue.
        }
    }

}