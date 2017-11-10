using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon
{
    public float fireRate = 0.5f;
    float previousShotTime = 0f;
    bool done = false;
    
    AudioSource audioSource;
    public MeleeWeapon()
    {
        
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        //registry.weapons.AddWeapon(this.gameObject); //AddToRegistry();
        gpParent = GameObject.Find("GeneralPurposeParent");
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
            
            previousShotTime = Time.time;
            

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