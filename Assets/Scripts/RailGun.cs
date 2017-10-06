using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class RailGun : Weapon
{
    public float fireRate = 1f;
    float previousShotTime = 0f;
    
    //private float timepassed = 0f;
    AudioSource audioSource;
    GameObject barrel;

    public RailGun()
    {
        
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        gpParent = GameObject.Find("GeneralPurposeParent");
        fireRate += Random.Range(-(fireRate*0.1f), fireRate*0.1f);
        barrel = transform.FindChild("barrel").gameObject;
    }

    

    
    
    public override bool Engage(Vector3 newTarget)
    {
        bool result = false;
        if ((Time.time - previousShotTime) >= (1f / fireRate))
        {
            //LineRenderer lr = GetComponentInChildren<LineRenderer>();
            Vector3[] positions = new Vector3[2];
            positions[0] = this.transform.position;
            positions[1] = this.transform.position + ((newTarget - this.transform.position).normalized * 100);


            previousShotTime = Time.time;
            audioSource.Play();
            RaycastHit2D[] hits = Physics2D.RaycastAll(positions[0], positions[1] - positions[0]);

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
                        new Vector2(0, 0),
                        groups);
                }
            }
            result = true;
        }
        return result;
    }

    public override void Arm()
    {

    }
    public override void Disarm()
    {

    }
}