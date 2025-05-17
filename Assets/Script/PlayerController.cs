using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Search;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 9f;// Lực nhảy lên
    private Rigidbody2D rb;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    void Start()
    {
        // Lấy tham chiếu tới Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Jump()
    {
        // Đặt vận tốc trục Y về 0 trước khi nhảy (để tránh cộng dồn)
        rb.velocity = new Vector2(rb.velocity.x, 0);
        // Thêm lực đẩy lên trên
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    void Update()
    {
        // Kiểm tra nếu nhấn phím cách
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        //bắn đạn khi nhấn f
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetDirection(Vector2.right);//ban tu trai qua phai
    }
    public void Die()
    {
        // Enemy bị tiêu diệt
        Destroy(gameObject);
        // Xử lý khi Player chết (kết thúc game hoặc giảm mạng)
        Debug.Log("Player is dead!");
    }
}
