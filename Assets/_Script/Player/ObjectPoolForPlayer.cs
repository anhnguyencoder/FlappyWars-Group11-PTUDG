using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolForPlayer : MonoBehaviour
{
    public static ObjectPoolForPlayer Instance;
    public GameObject bulletPrefab;
    public int poolSize = 20;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShootClip);

        //if (PlayerController.Instance.isFrozen) return null; // Không cấp viên đạn nếu đóng băng
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            GameObject newBullet = Instantiate(bulletPrefab);
            return newBullet;
        }

    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}