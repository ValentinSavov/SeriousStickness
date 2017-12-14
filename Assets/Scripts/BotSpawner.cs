using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public GameObject prefab;
    public string startWeapon = "RocketLauncher";
    public float spawnRadius = 3f;
    public float spawnPeriod = 5f;
    public int maxActiveSpawned = 5;
    public int spawnLimit = 1000;
    public bool spawnImmediately = true;
    float previousSpawnTime;
    int spawnedCounter = 0;
    string dinamicSpawnerID = "";
    Registry registry;

    void Start ()
    {
        dinamicSpawnerID = Random.Range(0, 9999).ToString();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        if (spawnImmediately)
        {
            Spawn();
        }
        previousSpawnTime = Time.time;
    }

	void Update()
    {
        if((Time.time - previousSpawnTime) >= spawnPeriod)
        {
            List<string> groups = new List<string>();
            groups.Add(dinamicSpawnerID);
            if (registry.damageAcceptors.GetAcceptorsInGroup(groups).Count < maxActiveSpawned)
            {
                Spawn();
            }
            previousSpawnTime = Time.time;
        }
    }

    void Spawn()
    {
        GameObject spawned = Instantiate(prefab, 
            this.transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0),
            Quaternion.identity
            ) as GameObject;
        //Debug.Log("Spawned");
        spawned.GetComponent<BotControl>().startWeapon = startWeapon;
        spawned.transform.parent = this.transform;
        spawned.GetComponent<DamageAcceptor>().groups.Add(dinamicSpawnerID);
        spawnedCounter++;
        if(spawnedCounter >= spawnLimit)
        {
            this.enabled = false;
        }
    }
}
