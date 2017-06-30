using UnityEngine;
using System.Collections;

public class MachineGun : Weapon
{
    public float fireRate = 0.5f;
    float previousShotTime = 0f;
    bool done = false;
    //int shotsCounter = 0;
    AudioSource audioSource;
    ParticleSystem mgShootEffect;
    public MachineGun()
    {
        
    }

    void Start()
    {
        mgShootEffect = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        //registry.weapons.AddWeapon(this.gameObject); //AddToRegistry();
        gpParent = GameObject.Find("GeneralPurposeParent");
        //fireRate += Random.Range(-(fireRate * 0.1f), fireRate * 0.1f);
    }

    public override bool isDone()
    {
        bool tdone = done;
        done = false;
        return tdone;
    }
    
    public override bool Engage(GameObject newTarget)
    {
        bool result = false;
        //this.transform.rotation = Quaternion.FromToRotation((this.transform.position).normalized, (this.transform.position + newTarget.transform.position).normalized);
        if ((Time.time - previousShotTime) >= (1f / fireRate))
        {
            previousShotTime = Time.time;
            mgShootEffect.Play();
            RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, newTarget.transform.position - this.transform.position);
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.collider.GetComponent<BorderTag>())
                {
                    GameObject bulletHitEffect = Instantiate(Resources.Load("BulletHitEffect", typeof(GameObject)), gpParent.transform) as GameObject;
                    bulletHitEffect.transform.position = new Vector3(hit.point.x, hit.point.y, -2f);
                }
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

                    GameObject bulletHitEffect = Instantiate(Resources.Load("BulletHitEffect", typeof(GameObject)), gpParent.transform) as GameObject;
                    bulletHitEffect.transform.position = new Vector3(hit.point.x, hit.point.y, -2f);
                    bulletHitEffect.GetComponent<ParticleSystem>().startColor = Color.red;
                }
            }

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