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
    //tốc độ rơi của Power Up
    public float fallSpeed = 2f;
    //bán kính vụ nổ
    private float explosionRadius = 2f;
    private Animator animator; // Tham chiếu đến Animator
// bom chưa đươc kích hoạt
    public bool isBomb = false;

    // Dictionary chứa thời gian hiệu lực cho từng Power-Up
    private Dictionary<PowerUpType, float> powerUpDurations = new Dictionary<PowerUpType, float>
    {
        { PowerUpType.BulletSizeX2, 5f },
        { PowerUpType.BulletSizeX3, 5f },
        { PowerUpType.BodySizeX2, 4f },
        { PowerUpType.Shield, 6f },
        { PowerUpType.Heal, 0f },
        { PowerUpType.Freeze, 3f },
        { PowerUpType.Bomb, 0f }
    };
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
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
            Destroy(other.gameObject);
            if (isBomb)
            {
                return;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("EnemyBullet"))
        {
            ApplyEffect(GameManager.Instance.GetCurrentEnemy());
            Destroy(other.gameObject);
            if (isBomb)
            {
               return;
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }
    
    void ApplyEffect(MonoBehaviour target)
    {
        float duration = powerUpDurations[type]; // Lấy thời gian từ Dictionary
        bool isPlayer = target is PlayerController;

        UIManager.Instance.AddPowerUpUI(type, isPlayer, duration);
        if (target is PlayerController player)
        {


            switch (type)
            {
                case PowerUpType.BulletSizeX2:
                    player.ModifyBulletSize(2, duration);
                    break;
                case PowerUpType.BulletSizeX3:
                    player.ModifyBulletSize(3, duration);
                    break;
                case PowerUpType.BodySizeX2:
                    player.ModifyBodySize(2, duration);
                    break;
                case PowerUpType.Shield:
                    player.ActivateShield(duration);
                    break;
                case PowerUpType.Heal:
                    player.Heal(1);
                    break;
                case PowerUpType.Freeze:
                    player.Freeze(duration);
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
                    enemy.ModifyBulletSize(type == PowerUpType.BulletSizeX2 ? 2 : 3, duration);
                    break;
                case PowerUpType.Freeze:
                    enemy.Freeze(duration);
                    break;
                case PowerUpType.BodySizeX2:
                    enemy.ModifyBodySize(2, duration);
                    break;
                case PowerUpType.Bomb:
                    Explode();
                    break;
                case PowerUpType.Shield:
                    enemy.ActivateShield(duration);
                    break;
            }
        }
    }
    
    void Explode()
    {
        if (animator != null)
        {
            animator.SetTrigger("Explode");  // Kích hoạt animation nổ
        }
        else
        {
            // Nếu không có animator, thực hiện nổ ngay lập tức
            DoExplosion();
        }
    }
    // Hàm này sẽ được gọi thông qua Animation Event (hoặc bạn có thể gọi sau khi delay)
    public void DoExplosion()
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
    
        // Sau khi hoàn thành nổ, destroy object
      Destroy(gameObject);
    }


}