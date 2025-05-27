    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerBulletController : MonoBehaviour
    {
        public float speed = 10f; // Tốc độ đạn
        private Vector2 direction; // Hướng di chuyển của viên đạn

        public void SetDirection(Vector2 newDirection)
        {
            direction = newDirection.normalized; // Đảm bảo hướng được chuẩn hóa
        }

        void Update()
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;

            if (!IsVisibleOnScreen())
            {
                ObjectPoolForPlayer.Instance.ReturnBullet(gameObject);
            }
        }

        private bool IsVisibleOnScreen()
        {
            Vector3 screenPosition = Camera.main.WorldToViewportPoint(transform.position);
            return screenPosition.x > 0 && screenPosition.x < 1 && screenPosition.y > 0 && screenPosition.y < 1;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyController enemyController = other.GetComponent<EnemyController>();
         
                if (enemyController != null)
                {
                   enemyController.Die();
                }
                ObjectPoolForPlayer.Instance.ReturnBullet(gameObject);
            }
        }
        //hàm thay đổi kích thước viên đạn
        public void SetBulletSize(float sizeMultiplier)
        {
            transform.localScale = Vector3.one * sizeMultiplier;
        }

    }