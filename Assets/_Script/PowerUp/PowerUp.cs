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
            // Trả viên đạn về khi power up va chạm với nó
            ObjectPoolForPlayer.Instance.ReturnBullet(other.gameObject);
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
            // Trả viên đạn về khi power up va chạm với nó

            ObjectPoolForEnemy.Instance.ReturnBullet(other.gameObject);
        
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
        // Lấy thời gian cơ bản từ Dictionary
        float baseDuration = powerUpDurations[type];
        bool isPlayer = target is PlayerController;

        
        // Nếu có upgrade tương ứng, cộng thêm giá trị
        // Nếu target là Player, cộng thêm upgrade tương ứng
        if (isPlayer)
        {
            if (type == PowerUpType.BulletSizeX2)
                baseDuration += UpgradeManager.Instance.additionalBulletSizeX2Duration;
            else if (type == PowerUpType.BulletSizeX3)
                baseDuration += UpgradeManager.Instance.additionalBulletSizeX3Duration;
            else if (type == PowerUpType.Shield)
                baseDuration += UpgradeManager.Instance.additionalShieldDuration;
        }

        UIManager.Instance.AddPowerUpUI(type, isPlayer, baseDuration);
        if (target is PlayerController player)
        {


            switch (type)
            {
                case PowerUpType.BulletSizeX2:
                    player.ModifyBulletSize(2, baseDuration);
                    break;
                case PowerUpType.BulletSizeX3:
                    player.ModifyBulletSize(3, baseDuration);
                    break;
                case PowerUpType.BodySizeX2:
                    player.ModifyBodySize(2, baseDuration);
                    break;
                case PowerUpType.Shield:
                    player.ActivateShield(baseDuration);
                    break;
                case PowerUpType.Heal:
                    // Tăng lượng heal theo upgrade: mặc định là 1, cộng thêm từ UpgradeManager
                    int extraHeal = UpgradeManager.Instance.additionalHealAmount;
                    player.Heal(1 + extraHeal);
                    break;
                case PowerUpType.Freeze:
                    player.Freeze(baseDuration);
                    break;
                case PowerUpType.Bomb:
                    //nếu isbomb=true thì Bomb sẽ không bị destroy ngay mà sẽ destroy sau khi animation bomb nổ xong
                    isBomb = true;
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
                    enemy.ModifyBulletSize(type == PowerUpType.BulletSizeX2 ? 2 : 3, baseDuration);
                    break;
                case PowerUpType.Freeze:
                    enemy.Freeze(baseDuration);
                    break;
                case PowerUpType.BodySizeX2:
                    enemy.ModifyBodySize(2, baseDuration);
                    break;
                case PowerUpType.Bomb:
                    isBomb = true;
                    Explode();
                    break;
                case PowerUpType.Shield:
                    enemy.ActivateShield(baseDuration);
                    break;
                case PowerUpType.Heal:
                    enemy.Heal(1);
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
        Debug.Log("DoExplosion");
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