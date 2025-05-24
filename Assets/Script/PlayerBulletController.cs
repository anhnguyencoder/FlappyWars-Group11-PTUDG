using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    public float speed = 10f; // Tốc độ đạn
    private Vector2 direction; // Hướng di chuyển của viên đạn

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized; // Đảm bảo hướng được chuẩn hóa
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); // Hủy viên đạn khi ra khỏi màn hình
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().Die();
            Destroy(gameObject);
        }
    }
}