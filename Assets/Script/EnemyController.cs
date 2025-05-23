using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum EnemyType
{
    Straight,      // Bắn thẳng
    Spread,        // Bắn tia
    SinWave,       // Hình sin
    Circular,      // Quỹ đạo tròn
    Burst,         // Bắn theo đợt
    Zigzag,        // Đạn zigzag
    Homing,        // Đạn tự dẫn
    Spiral,        // Quỹ đạo xoắn ốc
    Explosive,     // Đạn nổ khi chạm
    Random         // Đạn bay ngẫu nhiên
}

public class EnemyController : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab của đạn
    public Transform bulletSpawnPoint; // Vị trí bắn đạn
    public EnemyType enemyType; // Loại bắn của Enemy
    public float moveSpeed = 2f; // Tốc độ di chuyển
    public float moveInterval = 2f; // Thời gian đổi hướng di chuyển
    private float targetY; // Vị trí Y mục tiêu

    void Start()
    {
        InvokeRepeating(nameof(Shoot), 1f, 2f); // Gọi hàm Shoot sau 1 giây và lặp lại mỗi 2 giây
        InvokeRepeating(nameof(ChangeDirection), 0f, moveInterval); // Đổi hướng di chuyển
    }


    void ChangeDirection()
    {
        targetY = UnityEngine.Random.Range(-3f, 3f); // Vị trí ngẫu nhiên trên trục Y
    }

    void Shoot()
    {
        switch (enemyType)
        {
            case EnemyType.Straight:
                ShootStraight();
                break;
            case EnemyType.Spread:
                ShootSpread();
                break;
            case EnemyType.SinWave:
                ShootSinWave();
                break;
            case EnemyType.Circular:
                ShootCircular();
                break;
            case EnemyType.Burst:
                StartCoroutine(ShootBurst());
                break;
            case EnemyType.Zigzag:
                ShootZigzag();
                break;
            case EnemyType.Homing:
                ShootHoming();
                break;
            case EnemyType.Spiral:
                StartCoroutine(ShootSpiral());
                break;
            case EnemyType.Explosive:
                ShootExplosive();
                break;
            case EnemyType.Random:
                ShootRandom();
                break;
        }
    }

    void ShootStraight()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetDirection(Vector2.left);
        bulletController.bulletType = BulletController.BulletType.EnemyBullet; 
    }
    void ShootSpread()
    {
        Vector2[] directions = {
            new Vector2(-1, -0.5f).normalized,
            Vector2.left,
            new Vector2(-1, 0.5f).normalized
        };

        foreach (Vector2 direction in directions)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.SetDirection(direction);
            bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

        }
        

    }

    void ShootSinWave()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetDirection(Vector2.left);
        bulletController.isSinWave = true;
        bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

    }

    void ShootCircular()
    {
        int bulletsCount = 8;
        for (int i = 0; i < bulletsCount; i++)
        {
            float angle = i * Mathf.PI * 2 / bulletsCount;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.SetDirection(direction);
            bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

        }
    }

    IEnumerator ShootBurst()
    {
        int burstCount = 3;
        for (int i = 0; i < burstCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.SetDirection(Vector2.left);
            yield return new WaitForSeconds(0.2f); // Tạm dừng giữa mỗi viên đạn
            bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

        }
    }

    void ShootZigzag()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetDirection(Vector2.left);
        bulletController.isZigzag = true; // Thêm logic zigzag trong BulletController
        bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

    }

    void ShootHoming()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetHomingTarget(GameObject.FindWithTag("Player").transform);
        bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

    }

    IEnumerator ShootSpiral()
    {
        int bulletsCount = 20;
        float angle = 0f;
        for (int i = 0; i < bulletsCount; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.SetDirection(direction);
            angle += Mathf.PI / 10; // Tăng góc để tạo xoắn ốc
            yield return new WaitForSeconds(0.1f);
            bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

        }
    }

    void ShootExplosive()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.isExplosive = true; // Thêm logic nổ trong BulletController
        bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

    }

    void ShootRandom()
    {
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetDirection(randomDirection);
        bulletController.bulletType = BulletController.BulletType.EnemyBullet; 

    }


    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    public void Die()
    {
        Destroy(gameObject); // Xóa Enemy khi chết
        GameManager.Instance.EnemyKilled();
    }
}
