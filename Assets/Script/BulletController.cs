using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletController : MonoBehaviour
{
    
// Tốc độ bay của viên đạn
    public float speed = 10f;
    private Vector2 direction;//huong di chuyen cua vien dan
    
    
    public enum BulletType
    {
        PlayerBullet,
        EnemyBullet
    }

    public BulletType bulletType;
    
//ham thiet lap huong cho vien dan
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;// đảm bảo hướng được chuẩn hóa
        
    }
    // Update is called once per frame
    void Update()
    {
        //di chuyen theo huong
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
    // Hủy viên đạn khi nó ra khỏi màn hình
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu viên đạn chạm vào player hoặc enemy 
        if (bulletType == BulletType.EnemyBullet && other.CompareTag("Player"))
        {
           other.GetComponent<PlayerController>().Die();//player bi tieu diet
            Destroy(gameObject);
        }
        else if(bulletType == BulletType.PlayerBullet &&  other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().Die();//enemy bi tieu diet
            Destroy(gameObject);
        }
    }
}
