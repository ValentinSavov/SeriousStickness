using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class RailGun : Weapon
{
    public float fireRate = 1f;
    float previousShotTime = 0f;
    bool done = false;
    public float laserLifetime = 0.1f;
    private float timepassed = 0f;
    AudioSource audioSource;

    public RailGun()
    {
        
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        registry.weapons.AddWeapon(this.gameObject); //AddToRegistry();
        gpParent = GameObject.Find("GeneralPurposeParent");
        fireRate += Random.Range(-(fireRate*0.1f), fireRate*0.1f);
    }

    

    void Update()
    {
        //Debug.Log("UpdateWeaponChild");
        if (GetComponentInChildren<LineRenderer>().enabled)
        { 
            timepassed += Time.deltaTime;
            if (timepassed >= laserLifetime)
            {
                timepassed = 0f;
                GetComponentInChildren<LineRenderer>().enabled = false;
            }
        }
    }
    
    public override bool Engage(GameObject newTarget)
    {
        bool result = false;
        if ((Time.time - previousShotTime) >= (1f / fireRate))
        {
            //transform.rotation = Quaternion.FromToRotation(Vector3.right, newTarget.transform.position - this.transform.position);
            LineRenderer lr = GetComponentInChildren<LineRenderer>();
            Vector3[] positions = new Vector3[2];
            positions[0] = this.transform.position;
            positions[1] = this.transform.position + ((newTarget.transform.position - this.transform.position).normalized * 100);
            //lr.SetPositions(positions);
            //lr.enabled = true;

            previousShotTime = Time.time;
            done = true;
            Invoke("ResetDone", 0.2f / fireRate);
            audioSource.Play();
            RaycastHit2D[] hits = Physics2D.RaycastAll(positions[0], positions[1] - positions[0]);
            lr.SetPositions(positions);
            lr.enabled = true;
            foreach (RaycastHit2D hit in hits)
            {
                DamageAcceptor acceptor = hit.collider.gameObject.GetComponent<DamageAcceptor>();
                //DamageAcceptor myacceptor = GetComponentInParent<DamageAcceptor>();
                //if(myacceptor == null)
                {
                    //Debug.Log("AcceptorIsNull");
                }
                //List<string> groups = myacceptor.groups;
                if ((acceptor != null) && (acceptor != this.GetComponentInParent<DamageAcceptor>())) 
                {
                    //positions[1] = hit.transform.position;
                    registry.damageAcceptors.doTargetDamage(
                        acceptor,
                        GetComponentInParent<Tag>().gameObject,
                        damage,
                        "normal",
                        new Vector2(0, 0),
                        groups);
                }
                //lr.SetPositions(positions);
                //lr.enabled = true;
            }
            result = true;
        }
        return result;
    }

    public override bool isDone()
    {
        bool tdone = done;
        //done = false;
        return tdone;
    }
    
    void ResetDone()
    {
        done = false;
    }


    public override void Arm()
    {

    }
    public override void Disarm()
    {

    }


}