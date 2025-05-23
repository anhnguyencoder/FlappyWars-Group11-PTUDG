using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f; // Tốc độ đạn
    private Vector2 direction; // Hướng di chuyển của viên đạn
    public bool isSinWave = false; // Quỹ đạo hình sin
    public bool isZigzag = false; // Quỹ đạo zigzag
    public bool isExplosive = false; // Đạn nổ
    private Transform homingTarget; // Mục tiêu tự dẫn
    public float sinAmplitude = 1f; // Biên độ hình sin
    public float sinFrequency = 2f; // Tần số hình sin
    private float sinTime = 0f; // Thời gian để tính sin
    private float zigzagDirection = 1f; // Hướng zigzag

    
    public enum BulletType
    {
        PlayerBullet,
        EnemyBullet
    }

    public BulletType bulletType; // Thuộc tính xác định loại đạn

    
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized; // Đảm bảo hướng được chuẩn hóa
    }

    public void SetHomingTarget(Transform target)
    {
        homingTarget = target;
    }

    void Update()
    {
        if (isSinWave)
        {
            sinTime += Time.deltaTime;
            float sinOffset = Mathf.Sin(sinTime * sinFrequency) * sinAmplitude;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            transform.position += new Vector3(0, sinOffset, 0);
        }
        else if (isZigzag)
        {
            float zigzagOffset = Mathf.Sin(Time.time * sinFrequency) * sinAmplitude;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            transform.position += new Vector3(0, zigzagOffset * zigzagDirection, 0);
        }
        else if (homingTarget != null)
        {
            Vector2 homingDirection = (homingTarget.position - transform.position).normalized;
            transform.position += (Vector3)homingDirection * speed * Time.deltaTime;
        }
        else
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); // Hủy viên đạn khi ra khỏi màn hình
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (bulletType == BulletType.PlayerBullet && other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().Die();
            Destroy(gameObject);
        }
        else if (bulletType == BulletType.EnemyBullet && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Die();
            Destroy(gameObject);
        }
    }

}
