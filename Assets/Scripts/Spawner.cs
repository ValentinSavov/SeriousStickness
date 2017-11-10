using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRadius = 3f;
    public float spawnDelay = 5f;
    public int maxActiveSpawned = 1;
    public int spawnLimit = 1000;
    public bool spawnImmediately = true;
    float previousSpawnTime;
    int spawnedCounter = 0;

    void Start ()
    {

        if (spawnImmediately)
        {
            Spawn();
        }
        previousSpawnTime = Time.time;
    }

	void Update()
    {
        if (transform.childCount > maxActiveSpawned)
        {
            previousSpawnTime = Time.time;
        }
        else
        {
            if ((Time.time - previousSpawnTime) >= spawnDelay)
            {
                Spawn();
                previousSpawnTime = Time.time;
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
