using UnityEngine;
using System.Collections.Generic;
public class TrollFace : MonoBehaviour, DamageAcceptor
{
    public GameObject minionPrefab;
    public float hitPoints = 20f;
    public List<string> groups { get; set; }
    public int minionsCount = 4;
    public float minionsSpawnCooldown = 3f;
    public bool activated = false;
    GameObject gpParent;
    Registry registry;
    GameObject target;
    float spawnCooldown = 0f;

    Transform spawnPoint;
    
    void Start()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
        gpParent = GameObject.Find("GeneralPurposeParent");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        spawnPoint = transform.Find("SpawnPoint").transform;
    }

    void Update()
    {
        if (!activated)
        {
            if ((transform.position - target.transform.position).magnitude <= 5f)
            {
                activated = true;
            }
        }
        else
        {

            List<string> checkGroups = new List<string>();
            checkGroups.Add("TrollFaceMinions");
            if (registry.damageAcceptors.GetAcceptorsCountInGroup(checkGroups) < minionsCount)
            { 
                spawnCooldown -= Time.deltaTime;
                if (spawnCooldown <= 0)
                {
                    Spawn();
                    spawnCooldown = minionsSpawnCooldown;
                }
            }
        }
    }
    
    void Spawn()
    {
        GameObject spawned = Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity, gpParent.transform) as GameObject;
        spawned.GetComponent<DamageAcceptor>().groups.Add("TrollFaceMinions");
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        float locDamage = argInArgs.dmg;

        if (locDamage > 0)
        {
            if (hitPoints > locDamage)
            {
                hitPoints -= locDamage;
                locDamage = 0;
            }
            else
            {
                hitPoints = 0;
                DamageProvider dp = argInArgs.source.GetComponent<DamageProvider>();
                if (dp != null)
                {
                    dp.ReportKill(this);
                }
            }
        }
    }
}
