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
    Homing,
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

    void Start()
    {
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

    void ShootStraight()
    {
        GameObject bullet = ObjectPool.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(Vector2.left);
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
            GameObject bullet = ObjectPool.Instance.GetBullet();
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

            GameObject bullet = ObjectPool.Instance.GetBullet();
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
            GameObject bullet = ObjectPool.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
            bulletController.SetDirection(Vector2.left);

            yield return new WaitForSeconds(0.2f);
        }
    }

    void ShootHoming()
    {
        GameObject bullet = ObjectPool.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(Vector2.left); // Homing logic có thể thêm vào nếu cần
    }

    IEnumerator ShootSpiral()
    {
        int bulletsCount = 20;
        float angle = 0f;
        for (int i = 0; i < bulletsCount; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject bullet = ObjectPool.Instance.GetBullet();
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
        
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        GameObject bullet = ObjectPool.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(randomDirection);
    }

    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    public void Die()
    {
        Destroy(gameObject);
        GameManager.Instance.EnemyKilled(enemyType);
    }
}
