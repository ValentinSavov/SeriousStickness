using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRadius = 3f;
    public float spawnPeriod = 5f;
    public bool spawnImmediately = true;
    float previousSpawnTime;

    void Start ()
    {
        if(spawnImmediately)
        {
            Spawn();
        }
    }

	void Update()
    {
        if((Time.time - previousSpawnTime) >= spawnPeriod)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        previousSpawnTime = Time.time;
        GameObject spawned = Instantiate(prefab, 
            this.transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0),
            Quaternion.identity
            ) as GameObject;
        spawned.transform.parent = this.transform;
    }
}
