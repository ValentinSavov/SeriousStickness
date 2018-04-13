using UnityEngine;
using System.Collections;

public class Launcher : Weapon
{
    public string projectileResourceName = "Rocket";
    public float fireRate = 0.5f;
    float previousShotTime = 0f;
    AudioSource audioSource;
    GameObject barrel;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        gpParent = GameObject.Find("GeneralPurposeParent");
        barrel = transform.Find("barrel").gameObject;
    }

    public override bool Engage(Vector3 newTarget)
    {
        bool result = false;
        if ((Time.time - previousShotTime) >= (1f / fireRate))
        {
            GameObject proj = Instantiate(Resources.Load(projectileResourceName, typeof(GameObject)),
                barrel.transform.position, Quaternion.FromToRotation(Vector3.right,
                newTarget - this.transform.position)) 
                as GameObject;
            previousShotTime = Time.time;
            
            proj.transform.parent = gpParent.transform;
            proj.GetComponent<Projectile>().damageSource = ((Component)GetComponentInParent<DamageProvider>()).gameObject;
            proj.GetComponent<Projectile>().damage = damage;
            audioSource.Play();

            result = true;
        }

        return result;
    }
}