using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public enum EnemyType
{
    Straight,
    Spread,
    Circular,
    Burst,

    Spiral,
    Random
}

public class EnemyController : MonoBehaviour
{
    [Header("Movement & Shooting")]
    public Transform bulletSpawnPoint;
    public EnemyType enemyType;
    public float moveSpeed = 2f;
    public float moveInterval = 2f;
    private float targetY;
    [Header("Sizes")]
    public float bulletSize;
    private float _bulletSize;
    public float bodySize = 0.31f;
    
    [Header("Health")]
    public int maxHealth;         // Sức khỏe tối đa của enemy (được khởi tạo từ GameManager)
    public int currentHealth;     // Sức khỏe hiện tại
    
    // Flag để kiểm tra trạng thái freeze
    private bool isFrozen = false;


   

    private bool isShieldActive = false;

    private Animator animator; // Tham chiếu tới Animator
    private Vector3 originalScale;
    private Coroutine freezeCoroutine;
    
    [Header("Visuals")]
    private SpriteRenderer spriteRenderer;

    private Dictionary<EnemyType, Color> shootingStyleColors = new Dictionary<EnemyType, Color>()
    {
        { EnemyType.Straight, Color.white },
        { EnemyType.Spread, Color.green },
        { EnemyType.Circular, Color.blue },
        { EnemyType.Burst, Color.yellow },
        { EnemyType.Spiral, Color.magenta },
        { EnemyType.Random, Color.cyan }
    };

    [Header("Enemy UI")]

    public TextMeshProUGUI enemyHealthText; // UI hiển thị sức khỏe của enemy (gán qua Inspector)
    void Awake()
    {
        originalScale = transform.localScale; // Lưu lại scale ban đầu của enemy
        animator = GetComponent<Animator>();

        InitializeHealth(); // Khởi tạo health cho enemy
        spriteRenderer = GetComponent<SpriteRenderer>();
        _bulletSize = bulletSize;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating(nameof(Shoot), 1f, 2f);
        InvokeRepeating(nameof(ChangeDirection), 0f, moveInterval);
        
        // Cập nhật màu sắc dựa trên kiểu bắn hiện tại
        UpdateColorBasedOnShootingStyle();
    }
    // Hàm khởi tạo health dựa trên currentEnemyMaxHealth từ GameManager
    void InitializeHealth()
    {
        maxHealth = GameManager.Instance.currentEnemyMaxHealth;
        currentHealth = maxHealth;
        UpdateEnemyHealthUI(currentHealth, maxHealth);
    }
    // Cập nhật UI của enemy (ví dụ: hiển thị "1/2")
   

    // Hàm nhận sát thương (được gọi từ va chạm với đạn của player)
    public void TakeDamage(int damage)
    {
        if (isShieldActive) return;

        currentHealth -= damage;
        // Cập nhật UI enemy health qua UIManager
        UpdateEnemyHealthUI(currentHealth, maxHealth);
      //  currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Đảm bảo health không âm
        // Gọi cập nhật thanh máu
        EnemyHealthBar healthBar = FindObjectOfType<EnemyHealthBar>();
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHitClip);

        // Tăng điểm cho người chơi 
        UIManager.Instance.AddScore(1);
    }
    void ChangeDirection()
    {
        targetY = UnityEngine.Random.Range(-5f, 5f);
    }

    void Shoot()
    {
        if (isFrozen)
            return; // Nếu đang freeze thì không bắn
        switch (enemyType)
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

    void ShootStraight()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

        GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        bulletController.SetDirection(Vector2.left);
        bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
    }

    void ShootSpread()
    {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);
        Vector2[] directions =
        {
            new Vector2(-1, -0.5f).normalized,
            Vector2.left,
            new Vector2(-1, 0.5f).normalized
        };

        foreach (Vector2 direction in directions)
        {

            GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
            bulletController.SetDirection(direction);
            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
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

            GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
            bulletController.SetDirection(direction);
            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
        }
    }

    IEnumerator ShootBurst()
    {
        int burstCount = 3;
        for (int i = 0; i < burstCount; i++)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

            if (isFrozen)
                yield break; // Dừng bắn nếu enemy đang freeze
            
            GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
            bulletController.SetDirection(Vector2.left);
            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước đạn hiện tại của enemy

            yield return new WaitForSeconds(0.2f);
        }
    }


    IEnumerator ShootSpiral()
    {
        int bulletsCount = 20;
        float angle = 0f;
        for (int i = 0; i < bulletsCount; i++)
        {
           

            if (isFrozen)
                yield break; // Dừng bắn nếu enemy đang freeze
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
            bulletController.SetDirection(direction);
            bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

            angle += Mathf.PI / 10;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShootRandom()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))
            .normalized;
        GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        bulletController.SetDirection(randomDirection);
        bulletController.SetBulletSize(_bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
    }

    void Update()
    {
        if (isFrozen)
            return; // Bỏ qua logic di chuyển khi đang freeze
        
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        
    }

    public void Die()
    {
        if (!isShieldActive)
        {
            if (animator != null)
            {
                animator.SetTrigger("Die"); // Kích hoạt trạng thái Die
            }
            else
            {
                Destroy(gameObject); // Hủy ngay nếu không có Animator
            }
// Thông báo cho GameManager để cập nhật số enemy bị tiêu diệt và nâng cấp enemy mới
            GameManager.Instance.EnemyKilled(enemyType);

            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDieClip);

        }
    }

    // Hàm này được gọi từ Animation Event
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
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
    

    public void Freeze(float duration)
    {
        // Dừng coroutine nếu nó đang chạy
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }

        // Bắt đầu một coroutine mới và lưu tham chiếu
        freezeCoroutine = StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        // Lưu lại màu ban đầu
       Color originalColor = GetComponent<SpriteRenderer>().color;
        // Bật trạng thái freeze
        isFrozen = true;

        // (Tùy chọn) Thêm hiệu ứng trực quan, ví dụ: thay đổi màu sắc
        GetComponent<SpriteRenderer>().color = Color.gray;

        // Chờ thời gian freeze
        yield return new WaitForSeconds(duration);

        // Kết thúc freeze, khôi phục lại trạng thái ban đầu
        isFrozen = false;
        GetComponent<SpriteRenderer>().color = originalColor;

        // Đặt tham chiếu coroutine về null khi kết thúc
        freezeCoroutine = null;
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
        transform.localScale = originalScale * multiplier; // Giữ nguyên hướng ban đầu
        yield return new WaitForSeconds(duration);
        transform.localScale = originalScale; // Trở về kích thước ban đầu
        bodySizeCoroutine = null; // Khi coroutine hoàn thành, đặt lại tham chiếu
    }
    private IEnumerator ShieldCoroutine(float duration)
    {
        // Activate shield effect here (e.g., visual effect)
        yield return new WaitForSeconds(duration);
        isShieldActive = false;
    }

    public void ActivateShield(float duration)
    {
        if (!isShieldActive)
        {
            isShieldActive = true;
            StartCoroutine(ShieldCoroutine(duration));
        }
    }
    public void Heal(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            UpdateEnemyHealthUI(currentHealth, maxHealth);
            
            // Gọi cập nhật thanh máu
            EnemyHealthBar healthBar = FindObjectOfType<EnemyHealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(currentHealth, maxHealth);
            }
        }
    }
    void UpdateColorBasedOnShootingStyle()
    {
        if (shootingStyleColors.ContainsKey(enemyType))
        {
            spriteRenderer.color = shootingStyleColors[enemyType];
        }
        else
        {
            spriteRenderer.color = Color.white; // Mặc định nếu không tìm thấy kiểu bắn
        }
    }
    
    public void UpdateEnemyHealthUI(int current, int max)
    {
        if (enemyHealthText != null)
        {
            enemyHealthText.text =  current + "/" + max;
        }
    }
}