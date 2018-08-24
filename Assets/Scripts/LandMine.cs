using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour, DamageAcceptor
{ 
    public List<string> groups { get; set; }

    public List<GameObject> effectsOnDestruct;
    public float damage = 50f;
    public float damageRadius = 5f;
    public float knockback = 10000f;
    GameObject gpParent;
    bool activated = false;
    public float explosionDelay = 2f;
    float timeFromActivation = 0f;
    AudioSource beepSound;
    Animator pulseAnim;
    void Start()
    {
        Registry.instance.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
        gpParent = GameObject.Find("GeneralPurposeParent");
        beepSound = GetComponent<AudioSource>();
        pulseAnim = transform.Find("Pulse").GetComponent<Animator>();
        pulseAnim.enabled = false;
    }

    void Update()
    {
        if(activated)
        {
            timeFromActivation += Time.deltaTime;
            if(timeFromActivation >= explosionDelay)
            {
                Explode();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerTag>())
        {
            Activate();
        }
    }
    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (argInArgs.dmg > 0)
        {
            Activate();
        }
    }
    void Activate()
    {
        if (!activated)
        {
            activated = true;
            beepSound.Play();
            pulseAnim.enabled = true;
            transform.Find("Circle").gameObject.SetActive(false);
        }
    }
    void Explode()
    {
        Registry.instance.damageAcceptors.doAreaDamage(this.gameObject, (Vector2)transform.position, damageRadius, damage, "normal", knockback);
        foreach (GameObject effectOnDestruct in effectsOnDestruct)
        {
            GameObject effect = Instantiate(effectOnDestruct, gpParent.transform);
            effect.transform.position = this.transform.position;
            effect.transform.localScale *= damageRadius;
        }
        Destroy(this.gameObject);
    }
    void OnDestroy()
    {
        Registry.instance.damageAcceptors.RemoveDamageAcceptor(this);
    }
}
