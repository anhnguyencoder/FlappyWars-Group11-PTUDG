using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;
    private Transform homingTarget;

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    public void SetHomingTarget(Transform target)
    {
        homingTarget = target;
    }

    void Update()
    {
        if (homingTarget != null)
        {
            Vector2 homingDirection = (homingTarget.position - transform.position).normalized;
            transform.position += (Vector3)homingDirection * speed * Time.deltaTime;
        }
        else
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // other.GetComponent<PlayerController>().Die();
            Destroy(gameObject);
        }
    }
}