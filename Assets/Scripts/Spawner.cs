using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRadius = 3f;
    public float spawnPeriod = 5f;
    public int maxActiveSpawned = 1;
    public int spawnLimit = 1000;
    public bool spawnImmediately = true;
    float spawnCooldown;
    int spawnedCounter = 0;

    void Start ()
    {
        spawnCooldown = spawnPeriod;
        if (spawnImmediately)
        {
            spawnCooldown = 0;
        }
    }

	void Update()
    {
        if (transform.childCount < maxActiveSpawned)
        {
            spawnCooldown -= Time.deltaTime;
            if (spawnCooldown <= 0)
            {
                Spawn();
                spawnCooldown = spawnPeriod;
            }
        }
    }

    void Spawn()
    {
        GameObject spawned = Instantiate(prefab, 
            this.transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0),
            Quaternion.identity
            ) as GameObject;
        spawned.transform.parent = this.transform;
        spawnedCounter++;
        if(spawnedCounter >= spawnLimit)
        {
            this.enabled = false;
        }
    }
}
