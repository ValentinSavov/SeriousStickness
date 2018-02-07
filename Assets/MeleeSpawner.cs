﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float AIRange = 10f;

    public float spawnPeriod = 5f;
    public int maxActiveSpawned = 1;
    public int spawnLimit = 1000;
    public bool spawnImmediately = true;
    float spawnCooldown = 0;
    int spawnedCounter = 0;
    string dinamicSpawnerID = "";
    Registry registry;

    void Start ()
    {
        InteractableObject io = GetComponentInParent<InteractableObject>();
        if(io != null)
        {
            io.OnDestruct += DestructionEventHandler;
        }
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
        GameObject spawned = Instantiate(prefab, this.transform.position, Quaternion.identity, this.transform) as GameObject;
        spawned.GetComponent<MeleeBotControl>().range = AIRange;
        spawned.GetComponent<DamageAcceptor>().groups.Add(dinamicSpawnerID);
        spawnedCounter++;
        if (spawnedCounter >= spawnLimit)
        {
            this.enabled = false;
        }
    }

    void DestructionEventHandler()
    {
        transform.Find("Healthy").gameObject.SetActive(false);
        transform.Find("Damaged").gameObject.SetActive(true);
        this.enabled = false;
    }
}
