using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public float jumpForce = 9f; // Lực nhảy lên
    private Rigidbody2D rb;
    public Transform bulletSpawnPoint;
    private EnemyType currentShootingStyle = EnemyType.Straight; // Mặc định kiểu bắn ban đầu
    private float lastShootTime = 0f; // Thời điểm bắn lần cuối

    private Dictionary<EnemyType, float> shootingCooldowns = new Dictionary<EnemyType, float>
    {
        { EnemyType.Straight, 0.5f },
        { EnemyType.Spread, 0.5f },
        { EnemyType.Circular, 0.5f },
        { EnemyType.Burst, 1.5f },
        { EnemyType.Homing, 0.8f },
        { EnemyType.Spiral, 2.5f },
        { EnemyType.Random, 0.1f }
    };
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        float cooldown = shootingCooldowns[currentShootingStyle];
        
        // Kiểm tra thời gian cooldown
        if (Time.time - lastShootTime < cooldown)
        {
            return; // Chưa đủ thời gian, không bắn
        }
        lastShootTime = Time.time; // Cập nhật thời điểm bắn
        switch (currentShootingStyle)
        {
            case EnemyType.Straight:
                ShootStraight();
                break;
            case EnemyType.Spread:
                ShootSpread();
                break;
            case EnemyType.Circular:
                ShootCircular();
                break;
            case EnemyType.Burst:
                StartCoroutine(ShootBurst());
                break;
            case EnemyType.Homing:
                ShootHoming();
                break;
            case EnemyType.Spiral:
                StartCoroutine(ShootSpiral());
                break;
            case EnemyType.Random:
                ShootRandom();
                break;
        }
    }

    public void SetShootingStyle(EnemyType newStyle)
    {
        currentShootingStyle = newStyle;
    }

    void ShootStraight()
    {
        GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(Vector2.right);
    }

    void ShootSpread()
    {
        Vector2[] directions = {
            new Vector2(1, -0.5f).normalized,
            Vector2.right,
            new Vector2(1, 0.5f).normalized
        };

        foreach (Vector2 direction in directions)
        {
            GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(direction);
        }
    }

    void ShootCircular()
    {
        int bulletsCount = 8;
        for (int i = 0; i < bulletsCount; i++)
        {
            float angle = i * Mathf.PI * 2 / bulletsCount;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(direction);
        }
    }

    IEnumerator ShootBurst()
    {
        int burstCount = 3;
        for (int i = 0; i < burstCount; i++)
        {
            GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(Vector2.right);

            yield return new WaitForSeconds(0.2f);
        }
    }

    void ShootHoming()
    {
        GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(Vector2.right); // Homing logic có thể thêm vào nếu cần
    }

    IEnumerator ShootSpiral()
    {
        int bulletsCount = 20;
        float angle = 0f;
        for (int i = 0; i < bulletsCount; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(direction);

            angle += Mathf.PI / 10;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShootRandom()
    {
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(randomDirection);
    }
    public void Die()
    {
        Debug.Log("Player is dead!");
        // Thêm logic xử lý khi Player chết, như reset game hoặc giảm mạng sống
    }
}