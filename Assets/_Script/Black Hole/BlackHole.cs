using Unity.VisualScripting;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển của hố đen
    public float rotationSpeed = -100f; // Tốc độ quay của hố đen (độ/giây)
    void Update()
    {
      
        // Di chuyển hố đen từ phải sang trái trong không gian thế giới
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        //quay tròn hố đen
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("EnemyBullet")|| collision.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
        }
        
        if (collision.CompareTag("Player"))
        {
            // Gọi hiệu ứng và thông báo BlackHoleManager
            FindObjectOfType<BlackHoleManager>().OnBlackHoleTriggered();

         
            // Hủy hố đen sau khi player chạm vào
            Destroy(gameObject);
        }
        // Kiểm tra nếu đối tượng chạm vào là hố đen
        else if (collision.CompareTag("BlackHoleDeathZone"))
        {
            Destroy(gameObject); // Phá hủy hố đen
            Debug.Log("va cham death zone");
        }
    }

   
}