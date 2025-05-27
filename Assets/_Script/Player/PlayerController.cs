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
    //
    private bool isShieldActive = false;
    //

    private Dictionary<EnemyType, float> shootingCooldowns = new Dictionary<EnemyType, float>
    {
        { EnemyType.Straight, 0.5f },//bắn thẳng
        { EnemyType.Spread, 0.5f },// bắn 3 hướng
        { EnemyType.Circular, 0.5f },//bắn 8 hướng
        { EnemyType.Burst, 1.5f },//bắn theo đợt 
        
        { EnemyType.Spiral, 2.5f },//bắn xoắn ốc
        { EnemyType.Random, 0.1f }//bắn ngẫu nhiên hướng
    };
    
    public float bulletSize = 2f;
    public float bodySize = 1f;

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
    public void TakeDamage()
    {
        if (!isShieldActive)
        {
            UIManager.Instance.TakeDamage();
            if (UIManager.Instance.health <= 0)
            {
                Die();
            }
        }
        
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
        
        // Cập nhật UI cooldown
        UpdateCooldownUI();
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
        bulletController.SetBulletSize(PlayerController.Instance.bulletSize); // Áp dụng kích thước
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
            
            bulletController.SetBulletSize(PlayerController.Instance.bulletSize); // Áp dụng kích thước
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
            bulletController.SetBulletSize(PlayerController.Instance.bulletSize); // Áp dụng kích thước
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

            bulletController.SetBulletSize(PlayerController.Instance.bulletSize); // Áp dụng kích thước
            yield return new WaitForSeconds(0.2f);
        }
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

            bulletController.SetBulletSize(PlayerController.Instance.bulletSize); // Áp dụng kích thước
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
        bulletController.SetBulletSize(PlayerController.Instance.bulletSize); // Áp dụng kích thước
    }
    public void Die()
    {
        gameObject.SetActive(false);
        UIManager.Instance.GameOver();
    }
    void UpdateCooldownUI()
    {
        float cooldown = shootingCooldowns[currentShootingStyle];
        float timeSinceLastShoot = Time.time - lastShootTime;
        float timeLeft = Mathf.Max(0, cooldown - timeSinceLastShoot);

        // Gọi UIManager để cập nhật UI
        UIManager.Instance.UpdateCooldownUI(timeLeft, cooldown);
        
    }
    
    
    public void ModifyBodySize(float multiplier)
    {
        StopAllCoroutines(); // Dừng các coroutine cũ nếu có
        StartCoroutine(BodySizeCoroutine(multiplier, 3f)); // Kích hoạt coroutine
    }

    private IEnumerator BodySizeCoroutine(float multiplier, float duration)
    {
        float originalSize = 0.31f; // Kích thước cơ thể mặc định
        bodySize *= multiplier; // Tăng kích thước cơ thể
        transform.localScale = Vector3.one * bodySize;
        yield return new WaitForSeconds(duration); // Đợi trong 3 giây
        bodySize = originalSize; // Trả về kích thước ban đầu
        transform.localScale = Vector3.one * bodySize; // Áp dụng lại kích thước
    }


    public void ActivateShield(float duration)
    {
        if (!isShieldActive)
        {
            isShieldActive = true;
            StartCoroutine(ShieldCoroutine(duration));
        }
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        // Activate shield effect here (e.g., visual effect)
        yield return new WaitForSeconds(duration);
        isShieldActive = false;
    }

    public void Heal(int amount)
    {
        if (UIManager.Instance.health < UIManager.Instance.maxHealth)
        {
            UIManager.Instance.health += amount;
            UIManager.Instance.UpdateUI();  
        }
        

    }
    public void Freeze(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        float originalSpeed = bodySize;
        bodySize = 0;
        yield return new WaitForSeconds(duration);
        bodySize = originalSpeed;
    }
    // Hàm thay đổi kích thước đạn
    public void ModifyBulletSize(float multiplier)
    {
        StopAllCoroutines(); // Dừng các coroutine cũ nếu có
        StartCoroutine(BulletSizeCoroutine(multiplier, 3f)); // Kích hoạt coroutine
    }
    private IEnumerator BulletSizeCoroutine(float multiplier, float duration)
    {
        float originalSize = 0.3229f;
        bulletSize *= multiplier; // Tăng kích thước đạn
        yield return new WaitForSeconds(duration); // Đợi trong 3 giây
        bulletSize = originalSize; // Trả về kích thước ban đầu
    }
}