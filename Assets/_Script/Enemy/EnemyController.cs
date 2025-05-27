using System.Collections;
using System.Collections.Generic;
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
    public Transform bulletSpawnPoint;
    public EnemyType enemyType;
    public float moveSpeed = 2f;
    public float moveInterval = 2f;
    private float targetY;

    //
    public float bulletSize = 0.3229f;
    public float bodySize = 1f;


    private bool isFrozen = false;

    private bool isShieldActive = false;

    private Animator animator; // Tham chiếu tới Animator

    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating(nameof(Shoot), 1f, 2f);
        InvokeRepeating(nameof(ChangeDirection), 0f, moveInterval);
    }

    void ChangeDirection()
    {
        targetY = UnityEngine.Random.Range(-5f, 5f);
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
        GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        bulletController.SetDirection(Vector2.left);
        bulletController.SetBulletSize(bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
    }

    void ShootSpread()
    {
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
            bulletController.SetBulletSize(bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
        }
    }

    void ShootCircular()
    {
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
            bulletController.SetBulletSize(bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
        }
    }

    IEnumerator ShootBurst()
    {
        int burstCount = 3;
        for (int i = 0; i < burstCount; i++)
        {
            GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
            bulletController.SetDirection(Vector2.left);
            bulletController.SetBulletSize(bulletSize); // Áp dụng kích thước đạn hiện tại của enemy

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

            GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
            bulletController.SetDirection(direction);
            bulletController.SetBulletSize(bulletSize); // Áp dụng kích thước đạn hiện tại của enemy

            angle += Mathf.PI / 10;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShootRandom()
    {
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))
            .normalized;
        GameObject bullet = ObjectPoolForEnemy.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        bulletController.SetDirection(randomDirection);
        bulletController.SetBulletSize(bulletSize); // Áp dụng kích thước đạn hiện tại của enemy
    }

    void Update()
    {
        if (!isFrozen)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
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

            GameManager.Instance.EnemyKilled(enemyType);

            // Tăng điểm cho người chơi
            UIManager.Instance.AddScore(1);
        }
    }

    // Hàm này được gọi từ Animation Event
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
    
    public void ModifyBulletSize(float multiplier)
    {
        StopCoroutine("BulletSizeCoroutine");
        StartCoroutine(BulletSizeCoroutine(multiplier, 3f));
    }

    private IEnumerator BulletSizeCoroutine(float multiplier, float duration)
    {
        float originalSize = 0.3229f;
        bulletSize *= multiplier;
        yield return new WaitForSeconds(duration);
        bulletSize = originalSize;
    }


    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            StartCoroutine(FreezeCoroutine(duration));
        }
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
    }
    public void ModifyBodySize(float multiplier)
    {
        bodySize *= multiplier;
        transform.localScale = Vector3.one * bodySize;
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
}