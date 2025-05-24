using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 9f; // Lực nhảy lên
    private Rigidbody2D rb;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, quaternion.identity);
        PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();
        bulletController.SetDirection(Vector2.right);
    }

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log("Player is dead!");
    }
}