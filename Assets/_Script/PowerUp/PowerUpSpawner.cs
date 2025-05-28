using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{


    public List<GameObject> powerUpPrefabs; // Danh sách các prefab Power-Up
    public float minSpawnTime = 20f;
    public float maxSpawnTime = 30f;
    void Start()
    {
        StartCoroutine(SpawnPowerUps());
    }

    IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            SpawnPowerUp();
        }
    }

    void SpawnPowerUp()
    {
        if (powerUpPrefabs.Count == 0) return;
        
        float randomX = Random.Range(-7f, 7f);
        Vector3 spawnPos = new Vector3(randomX, 6f, 0);
        
        int randomIndex = Random.Range(0, powerUpPrefabs.Count);
        GameObject powerUpObj = Instantiate(powerUpPrefabs[randomIndex], spawnPos, Quaternion.identity);
    }
}
