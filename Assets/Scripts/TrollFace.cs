using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrollFace : AIControl
{
    public GameObject minionPrefab;
    public int minionsCount = 4;
    public float minionsSpawnCooldown = 3f;
    public bool activated = false;
    public float moveRange = 5f;
    public float moveSpeed = 2f;
    public float damage = 14f;

    float spawnCooldown = 0f;
    float lookAtDirection = 1;
    float moveDirection = 1f;
    Transform spawnPoint;
    Vector3 startPosition;
    float moveCooldown = 3f;

    new void Start()
    {
        base.Start();
        spawnPoint = transform.Find("SpawnPoint").transform;
        startPosition = transform.position;
    }
    void Update()
    {

        lookAtDirection = Mathf.Sign(target.transform.position.x - transform.position.x);
        this.transform.localScale = new Vector3(lookAtDirection, 1, 1);

        float deltaX = (transform.position.x - startPosition.x);
        if ( (Mathf.Abs(deltaX) >= moveRange) && (Mathf.Sign(moveDirection) == Mathf.Sign(deltaX)))
        {
            moveCooldown -= Time.deltaTime;
            if (moveCooldown <= 0)
            {
                moveCooldown = 3f;
                moveDirection = -deltaX;
            }
        }
        else
        {
            transform.Translate(new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0, 0));
        }

        if (!activated)
        {
            if ((transform.position - target.transform.position).magnitude <= (moveRange*3))
            {
                activated = true;
            }
        }
        else
        {
            List<string> checkGroups = new List<string>();
            checkGroups.Add("TrollFaceMinions");
            if (Registry.instance.damageAcceptors.GetAcceptorsCountInGroup(checkGroups) < minionsCount)
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

    protected override void Die(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        Spawn();
        Spawn();
        Spawn();
        Spawn();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.gameObject);
        if (Registry.instance != null)
        {
            if (other.gameObject.GetComponent<PlayerTag>() != null)
            {
                DoDamage(other.GetComponent<DamageAcceptor>(), damage, 
                    new Vector2(Mathf.Sign(other.transform.position.x - transform.position.x), 0.5f) * 3000);
            }
        }
    }
}
