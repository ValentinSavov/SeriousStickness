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
    float spawnCooldown = 0;
    int spawnedCounter = 0;
    string dinamicSpawnerID = "";
    Registry registry;
    GameObject gpParent;
    void Start ()
    {
        gpParent = GameObject.Find("GeneralPurposeParent");
        dinamicSpawnerID = Random.Range(0, 9999).ToString();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        spawnCooldown = spawnPeriod;
        if (spawnImmediately)
        {
            spawnCooldown = 0;
        }
    }

	void Update()
    {
        List<string> groups = new List<string>();
        groups.Add(dinamicSpawnerID);
        if (registry.damageAcceptors.GetAcceptorsCountInGroup(groups) < maxActiveSpawned)
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
            Quaternion.identity, gpParent.transform
            ) as GameObject;
        //Debug.Log("Spawned");
        spawned.GetComponent<BotControl>().startWeapon = startWeapon;
        spawned.GetComponent<DamageAcceptor>().groups.Add(dinamicSpawnerID);
        spawnedCounter++;
        if(spawnedCounter >= spawnLimit)
        {
            this.enabled = false;
        }
    }
}
