using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class RailGun : Weapon
{
    public float fireRate = 1f;
    float cooldown;
    AudioSource audioSource;
    GameObject barrel;
    GameObject loadEffect;
    public RailGun()
    {
        
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        gpParent = GameObject.Find("GeneralPurposeParent");
        fireRate += Random.Range(-(fireRate*0.1f), fireRate*0.1f);
        barrel = transform.Find("barrel").gameObject;
        loadEffect = barrel.transform.Find("LoadEffect").gameObject;

        cooldown = 0;
    }
    
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            cooldown = 0f;
            loadEffect.transform.localScale = new Vector3(1 - (cooldown * fireRate), 1, 1);
        }

    }

    public override bool Engage(Vector3 newTarget)
    {
        bool result = false;
        if (cooldown <= 0f)
        {
            //LineRenderer lr = GetComponentInChildren<LineRenderer>();
            Vector3[] positions = new Vector3[2];
            positions[0] = this.transform.position;
            positions[1] = this.transform.position + ((newTarget - this.transform.position).normalized * 100);

            cooldown = (1f / fireRate);
            audioSource.Play();
            RaycastHit2D[] hits = Physics2D.RaycastAll(positions[0], positions[1] - positions[0], range);

            //lazer effect
            /*GameObject lazer =*/ Instantiate(Resources.Load("Lazer", typeof(GameObject)),
                barrel.transform.position, 
                Quaternion.FromToRotation(Vector3.right, newTarget- this.transform.position),
                gpParent.transform)/* as GameObject*/;
            ////

            foreach (RaycastHit2D hit in hits)
            {
                DamageAcceptor acceptor = hit.collider.gameObject.GetComponent<DamageAcceptor>();
                if ((acceptor != null) && (acceptor != this.GetComponentInParent<DamageAcceptor>())) 
                {
                    registry.damageAcceptors.doTargetDamage(
                        acceptor,
                        GetComponentInParent<Tag>().gameObject,
                        damage,
                        "normal",
                        new Vector2(0, 0));
                }
            }
            result = true;
        }
        return result;
    }
}