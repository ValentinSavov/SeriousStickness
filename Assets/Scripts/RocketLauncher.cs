using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon
{
    public float fireRate = 0.5f;
    float previousShotTime = 0f;
    bool done = false;
    //int shotsCounter = 0;
    AudioSource audioSource;
    GameObject barrel;
    public RocketLauncher()
    {
        
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        //registry.weapons.AddWeapon(this.gameObject); //AddToRegistry();
        gpParent = GameObject.Find("GeneralPurposeParent");
        barrel = transform.Find("barrel").gameObject;
    }

    public /*override*/ bool isDone()
    {
        bool tdone = done;
        done = false;
        return tdone;
    }
    
    public override bool Engage(Vector3 newTarget)
    {
        bool result = false;
        if ((Time.time - previousShotTime) >= (1f / fireRate))
        {
            GameObject proj = Instantiate(Resources.Load("Projectile", typeof(GameObject)),
                barrel.transform.position, Quaternion.FromToRotation(Vector3.right,
                newTarget - this.transform.position)) 
                as GameObject;
            previousShotTime = Time.time;
            
            proj.transform.parent = gpParent.transform;
            proj.GetComponent<Projectile>().parent = GetComponentInParent<Tag>().gameObject;
            proj.GetComponent<Projectile>().damage = damage;
            audioSource.Play();

            result = true;
        }
        done = true;

        return result;
    }

    public override void Arm()
    {

    }
    public override void Disarm()
    {

    }

}