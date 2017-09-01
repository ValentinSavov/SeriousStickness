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
    Registry registry;

    void Start ()
    {
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
            List<DamageAcceptor> das = registry.damageAcceptors.GetAcceptorsInRange(this.transform.position, 20f);
            foreach(DamageAcceptor da in das)
            {
                Debug.Log(((Component)da).gameObject.name);
            }
            Spawn();
            previousSpawnTime = Time.time;
        }
    }

    void Spawn()
    {
        GameObject spawned = Instantiate(prefab, 
            this.transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0),
            Quaternion.identity
            ) as GameObject;
        spawned.transform.parent = this.transform;
        //spawned.GetComponent<DamageAcceptor>().groups.Add(this.gameObject.name);
    }
}
