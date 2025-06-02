using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public GameConfig Config; // Lưu trữ tham chiếu đến GameConfig
    
    public float jumpForce = 9f; // Lực nhảy lên
    private static Rigidbody2D rb;
    public Transform bulletSpawnPoint;
    private EnemyType currentShootingStyle = EnemyType.Straight; // Mặc định kiểu bắn ban đầu
    private float lastShootTime = 0f; // Thời điểm bắn lần cuối
    private bool isShieldActive = false;


    private Dictionary<EnemyType, float> shootingCooldowns = new Dictionary<EnemyType, float>
    {
        { EnemyType.Straight, 0.5f }, //bắn thẳng
        { EnemyType.Spread, 0.5f }, // bắn 3 hướng
        { EnemyType.Circular, 0.5f }, //bắn 8 hướng
        { EnemyType.Burst, 1.5f }, //bắn theo đợt 

        { EnemyType.Spiral, 1.5f }, //bắn xoắn ốc
        { EnemyType.Random, 1.5f } //bắn ngẫu nhiên hướng
    };

    public float bulletSize ;
    private float _bulletSize;

    public float bodySize = 1f;

    // Quản lý health trong PlayerController
    public int health;

    private int maxHealth;

    //lưu trạng thái vật lí của Player
    private RigidbodyConstraints2D originalConstraints;

    //đóng băng
    public bool isFrozen = false;

    private Animator animator; // Tham chiếu đến Animator

    [Header("Health UI")]
    public TextMeshProUGUI healthText;
    
    void Awake()
    {
        maxHealth = Config.basePlayerHealth;
        Instance = this;
     
        animator = GetComponent<Animator>();
        _bulletSize = bulletSize;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing on Player!");
        }
        else
        {
            // Lưu trạng thái constraints ban đầu sau khi rb đã được gán
            originalConstraints = rb.constraints;
        }
        //cộng thêm máu đã nâng cấp
        maxHealth += UpgradeManager.Instance.additionalPlayerHealth;
        
        // Đảm bảo health = maxHealth ở thời điểm bắt đầu
        health = maxHealth;
        // Cập nhật UI health khi game bắt đầu
        
       UpdateHealthUI(health);
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
            health--;
           UpdateHealthUI(health);
         //   health = Mathf.Clamp(health, 0, maxHealth); // Đảm bảo health không âm
            // Gọi cập nhật thanh máu
            PlayerHealthBar healthBar = FindObjectOfType<PlayerHealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(health, maxHealth);
            }
            if (health <= 0)
            {
                Die();
               // healthBar.SetActive(health > 0);

            }
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHitClip);

        }   
    }

    void Update()
    {
        if (Application.isMobilePlatform)  // Hoặc: if (Input.touchSupported)
        {
            // Xử lý input trên điện thoại
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Nếu chạm vào nửa bên trái màn hình thì nhảy, bên phải thì bắn
                        if (touch.position.x < Screen.width / 2)
                        {
                            Jump();
                        }
                        else
                        {
                            Shoot();
                        }
                    }
                }
            }
        }
        else
        {
            // Xử lý input trên PC: Ví dụ, khi nhấn chuột bất kỳ chỗ nào, chỉ bắn
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
            // Ngoài ra, bạn có thể sử dụng phím để nhảy, ví dụ Space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        // Cập nhật các  cooldown
        UpdateCooldownUI();
    }


    void Shoot()
    {
        if (isFrozen) return; // Không bắn nếu đang bị đóng băng
        
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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

        GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(Vector2.right);
        bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước
    }

    void ShootSpread()
    {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);
        Vector2[] directions =
        {
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

            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước
        }
    }

    void ShootCircular()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

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
            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước
        }
    }

    IEnumerator ShootBurst()
    {
        

        int burstCount = 3;
        for (int i = 0; i < burstCount; i++)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

            if (isFrozen) yield break; // Dừng bắn nếu đang bị đóng băng

            GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(Vector2.right);

            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước
            yield return new WaitForSeconds(0.2f);
        }
    }


    IEnumerator ShootSpiral()
    {
        int bulletsCount = 10;
        float angle = 90f;
        for (int i = 0; i < bulletsCount; i++)
        {

            if (isFrozen) yield break; // Dừng bắn nếu đang bị đóng băng
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation =  Quaternion.identity;

            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(direction);

            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);
            angle -= Mathf.PI / 10;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShootRandom()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);
    
        int bulletCount = 5; // Số viên đạn bắn cùng lúc (có thể điều chỉnh)
        for (int i = 0; i < bulletCount; i++)
        {
            // Chọn góc ngẫu nhiên từ -45 đến 45 độ, chuyển sang radians (-PI/4, PI/4)
            float angle = UnityEngine.Random.Range(-Mathf.PI / 4, Mathf.PI / 4);
            // Hướng cơ sở của Player là về bên phải (1,0)
            Vector2 randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        
            GameObject bullet = ObjectPoolForPlayer.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;
        
            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(randomDirection);
            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
        UIManager.Instance.GameOver();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDieClip);

    }

    void UpdateCooldownUI()
    {
        float cooldown = shootingCooldowns[currentShootingStyle];
        float timeSinceLastShoot = Time.time - lastShootTime;
        float timeLeft = Mathf.Max(0, cooldown - timeSinceLastShoot);

        // Gọi UIManager để cập nhật UI
        UIManager.Instance.UpdateCooldownUI(timeLeft, cooldown);
    }


    private Coroutine bodySizeCoroutine; // Lưu tham chiếu riêng của coroutine kích thước cơ thể

    public void ModifyBodySize(float multiplier, float duration)
    {
        if (bodySizeCoroutine != null)
        {
            StopCoroutine(bodySizeCoroutine);
        }

        bodySizeCoroutine = StartCoroutine(BodySizeCoroutine(multiplier, duration));
    }

    private IEnumerator BodySizeCoroutine(float multiplier, float duration)
    {
        float originalSize = 0.31f;
        bodySize *= multiplier;
        transform.localScale = Vector3.one * bodySize;
        yield return new WaitForSeconds(duration);
        bodySize = originalSize;
        transform.localScale = Vector3.one * bodySize;
        bodySizeCoroutine = null; // Khi coroutine hoàn thành, đặt lại tham chiếu
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
        if (health < maxHealth)
        {
        animator.SetTrigger("Heal");
            health += amount;
            if (health > maxHealth) health = maxHealth;
            UpdateHealthUI(health);
            
          //  health = Mathf.Clamp(health, 0, maxHealth);

            // Gọi cập nhật thanh máu
            PlayerHealthBar healthBar = FindObjectOfType<PlayerHealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(health, maxHealth);
            }
        }
    }

    public void Freeze(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        // Đóng băng toàn bộ chuyển động
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        // (Tùy chọn) Thêm hiệu ứng trực quan
        GetComponent<SpriteRenderer>().color = Color.gray;
        isFrozen = true; // Hủy trạng thái đóng băng
        yield return new WaitForSeconds(duration);
        isFrozen = false; // Hủy trạng thái đóng băng
        // Khôi phục constraints ban đầu
        rb.constraints = originalConstraints;
        // Kích hoạt lại Rigidbody để nó không bị ở trạng thái "sleep"
        rb.WakeUp();

        GetComponent<SpriteRenderer>().color = Color.white;
    }


    // Hàm thay đổi kích thước đạn
    private Coroutine bulletSizeCoroutine; // Lưu tham chiếu riêng của coroutine kích thước đạn

    public void ModifyBulletSize(float multiplier, float duration)
    {
        if (bulletSizeCoroutine != null)
        {
            StopCoroutine(bulletSizeCoroutine);
        }

        bulletSizeCoroutine = StartCoroutine(BulletSizeCoroutine(multiplier, duration));
    }

    private IEnumerator BulletSizeCoroutine(float multiplier, float duration)
    {
       
        _bulletSize *= multiplier;
        yield return new WaitForSeconds(duration);
        _bulletSize = bulletSize;
        bulletSizeCoroutine = null;
    }
    // // Cập nhật UI Player Health
    public void UpdateHealthUI(int health)
    {
        healthText.text = health+"/"+ maxHealth;
    }
}