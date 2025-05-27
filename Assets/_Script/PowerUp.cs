using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum PowerUpType
{
    BulletSizeX2,
    BulletSizeX3,
    BodySizeX2,
    Shield,
    Heal,
    Freeze,
    Bomb
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public float fallSpeed = 2f;
    private float explosionRadius = 2f;
    
    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        if (transform.position.y < -6f) // Ra khỏi màn hình thì biến mất
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            ApplyEffect(PlayerController.Instance);
            Destroy(gameObject);
        }
        else if (other.CompareTag("EnemyBullet"))
        {
            ApplyEffect(GameManager.Instance.GetCurrentEnemy());
            Destroy(gameObject);
        }
    }
    
    void ApplyEffect(MonoBehaviour target)
    {
        if (target is PlayerController player)
        {
            switch (type)
            {
                case PowerUpType.BulletSizeX2:
                    player.ModifyBulletSize(2);
                    break;
                case PowerUpType.BulletSizeX3:
                    player.ModifyBulletSize(3);
                    break;
                case PowerUpType.BodySizeX2:
                    player.ModifyBodySize(2);
                    break;
                case PowerUpType.Shield:
                    player.ActivateShield(10);
                    break;
                case PowerUpType.Heal:
                    player.Heal(1);
                    break;
                case PowerUpType.Freeze:
                    PlayerController.Instance.Freeze(3);
                    break;
                case PowerUpType.Bomb:
                    Explode();
                    break;
            }
        }
        else if (target is EnemyController enemy)
        {
            switch (type)
            {
                case PowerUpType.BulletSizeX2:
                case PowerUpType.BulletSizeX3:
                    enemy.ModifyBulletSize(type == PowerUpType.BulletSizeX2 ? 2 : 3);
                    break;
                
                case PowerUpType.Freeze:
                    GameManager.Instance.GetCurrentEnemy()?.Freeze(3);
                    break;
                case PowerUpType.BodySizeX2:
                    enemy.ModifyBodySize(2);
                    break;
                case PowerUpType.Bomb:
                    Explode();
                    break;
                case PowerUpType.Shield:
                    enemy.ActivateShield(10);
                    break;
            }
        }
    }
    
    void Explode()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var obj in hitObjects)
        {
            if (obj.CompareTag("Player"))
            {
                PlayerController.Instance.TakeDamage();
            }
            else if (obj.CompareTag("Enemy"))
            {
                obj.GetComponent<EnemyController>()?.Die();
            }
        }
    }
}