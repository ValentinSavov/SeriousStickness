using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrollFace : MonoBehaviour, DamageAcceptor
{
    public GameObject minionPrefab;
    public float hitPoints = 20f;
    public List<string> groups { get; set; }
    public int minionsCount = 4;
    public float minionsSpawnCooldown = 3f;
    public bool activated = false;
    public float moveRange = 5f;
    public float moveSpeed = 2f;
    public float damage = 14f;

    GameObject gpParent;
    Registry registry;
    GameObject target;
    float spawnCooldown = 0f;
    float lookAtDirection = 1;
    float moveDirection = 1f;
    Transform spawnPoint;
    Rigidbody2D rbd;
    Vector3 startPosition;
    float startHitPoints;
    void Start()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
        gpParent = GameObject.Find("GeneralPurposeParent");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        spawnPoint = transform.Find("SpawnPoint").transform;
        rbd = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        startHitPoints = hitPoints;
    }

    float moveCooldown = 3f;
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
            activated = true;
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

                    Spawn();
                    Spawn();
                    Spawn();
                    Spawn();

                    this.gameObject.SetActive(false);
                    Destroy(this.gameObject, 0.1f);
                }
            }
            GameObject healthbar = transform.Find("HealthBar").gameObject;
            if (healthbar != null)
            {
                healthbar.transform.Find("Level").GetComponent<Image>().fillAmount = hitPoints / startHitPoints;
            }
        }
    }

    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.gameObject);
        if (registry != null)
        {
            if (other.gameObject.GetComponent<PlayerTag>() != null)
            {
                Attack(other.GetComponent<DamageAcceptor>(), damage, 
                    new Vector2(Mathf.Sign(other.transform.position.x - transform.position.x), 0.5f) * 3000);
            }
        }
    }

    void Attack(DamageAcceptor acceptor, float argInDamage, Vector2 knockback)
    {
        registry.damageAcceptors.doTargetDamage(
                    acceptor,
                    GetComponentInParent<Tag>().gameObject,
                    argInDamage,
                    "normal",
                    knockback);
    }
}
