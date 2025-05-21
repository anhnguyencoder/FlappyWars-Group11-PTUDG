using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;// Vị trí bắn đạn
    public float shootInterval = 1f;// Tốc độ bắn
    public float moveSpeed = 2f; // Tốc độ di chuyển theo trục Y
    public float moveInterval = 2f; // Thời gian giữa các lần đổi hướng
    private float targetY; // Vị trí Y mục tiêu


    
    void Start()
    {

       
        // Đặt giá trị ban đầu cho bộ đếm thời gian bắn
        InvokeRepeating(nameof(Shoot), shootInterval, shootInterval); // Bắt đầu bắn định kỳ
        InvokeRepeating(nameof(ChangeDirection), 0f, moveInterval); // Đổi hướng di chuyển định kỳ
    }

   
    void ChangeDirection()
    {
        // Đặt vị trí Y mục tiêu ngẫu nhiên trong khoảng -1 đến 1
        targetY = Random.Range(-3f, 3f);
    }
    void Shoot()
    {
        // Tạo viên đạn tại vị trí bắn
       GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
       BulletController bulletController = bullet.GetComponent<BulletController>();
       bulletController.SetDirection(Vector2.left);//ban tu phai qua trai
       bulletController.bulletType = BulletController.BulletType.EnemyBullet;

    }
    public void Die()
    {
        // Xử lý khi Enemy chết
        Debug.Log("Enemy is dead!");
        // Enemy bị tiêu diệt
        Destroy(gameObject);
        GameManager.Instance.EnemyKilled(); // Thông báo với GameManager
        ScoreManager.Instance.AddScore(1);
    }
    void Update()
    {
        // Di chuyển dần dần về vị trí mục tiêu trên trục Y
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
        transform.position = newPosition;
    }
}
