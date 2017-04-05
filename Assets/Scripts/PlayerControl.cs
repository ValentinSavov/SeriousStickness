using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour, DamageAcceptor
{
    public int jumpForce = 1;
    public bool grounded = false;
    public bool wantToJumpDown = false;
    
    public List<string> groups { get; set; }

    private StickStats stats;
    //private GameObject cursor;
    private Registry registry;

    RectTransform healthBar;
    private List<SourcesAndCooldowns> damageSourcesInCooldown = new List<SourcesAndCooldowns>();
    Animator anim;
    PlayerGear gear;
    Character character;
    void Start()
    {
        gear = GetComponent<PlayerGear>();
        anim = GetComponent<Animator>();
        stats = GetComponent<StickStats>();
        character = GetComponent<Character>();
        //CharacterInit();
        //cursor = GameObject.FindObjectOfType<Cursor>().gameObject;
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);

        groups = new List<string>();
        groups.Add("players");

        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;
    }


    void FixedUpdate()
    {
        //character.mInputs[(int)KeyInput.GoRight] = false;
        //character.mInputs[(int)KeyInput.GoLeft] = false;
        //character.mInputs[(int)KeyInput.Jump] = false;
        //character.mInputs[(int)KeyInput.GoDown] = false;

        UpdateDamageCooldowns();

        CheckGround();
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * stats.moveSpeed;
        if (anim != null) anim.SetFloat("Speed", Input.GetAxis("Horizontal"));

        //turn to needed position
        if (x > 0.05f)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        if (x < -0.05f)
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }

        transform.Translate(x, 0, 0);
        //float x = Input.GetAxis("Horizontal");
        if (x < 0)
        {
            //character.mInputs[(int)KeyInput.GoLeft] = true;
        }
        else if (x > 0)
        {
            //character.mInputs[(int)KeyInput.GoRight] = true;
        }

        if (Input.GetAxis("Vertical") < -0.5f)
        {
            //character.mInputs[(int)KeyInput.GoDown] = true;
            wantToJumpDown = true;
            transform.Translate(0, 0.001f, 0); // this is for trigger detection - if it does not move no trigger event is generated
        }
        else if (Input.GetButtonDown("Jump"))
        {
            //character.mInputs[(int)KeyInput.Jump] = true;
            if (grounded)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            gear.Engage();
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            gear.NextWeapon();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            gear.PrevWeapon();
        }

        //if (gameObject.activeInHierarchy)
            //character.CharacterUpdate();
    }

    void CharacterInit()
    {
        character.mInputs = new bool[(int)KeyInput.Count];
        character.mPrevInputs = new bool[(int)KeyInput.Count];

        character.mMap = GameObject.FindObjectOfType<Map>();
        character.mAudioSource = GetComponent<AudioSource>();
        character.mPosition = transform.position;
        character.mAnimator = GetComponent<Animator>();
        
        character.mAABB.HalfSize = new Vector2(6.0f, 7.0f);
        
        character.mJumpSpeed = stats.jumpSpeed;// Constants.cJumpSpeed;
        character.mWalkSpeed = stats.moveSpeed;// Constants.cWalkSpeed;

        //transform.localScale = new Vector3(mAABB.HalfSizeX / 8.0f, mAABB.HalfSizeY / 8.0f, 1.0f);
    }

    #region damage stuff
    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (damageSourcesInCooldown.Find(x => x.source == argInArgs.source) == null)
        {
            float locDamage = argInArgs.dmg;
            //argInArgs.cbIsAccepted = true;

            damageSourcesInCooldown.Add(new SourcesAndCooldowns(argInArgs.source));

            if (stats.currentArmorPoints > 0)
            {
                if (stats.currentArmorPoints > locDamage)
                {
                    stats.currentArmorPoints -= locDamage;
                    locDamage = 0;
                }
                else
                {
                    stats.currentArmorPoints = 0;
                    locDamage -= stats.currentArmorPoints;
                }
            }
            if (locDamage > 0)
            {
                if (stats.currentHitPoints > locDamage)
                {
                    stats.currentHitPoints -= locDamage;
                    locDamage = 0;
                }
                else
                {
                    stats.currentHitPoints = 0;
                    stats.isDead = true;
                    this.enabled = false;
                    GameObject.FindObjectOfType<SceneControl>().Invoke("ReloadScene", 1f); ;
                }
            }
        }

        if (argInArgs.knockback != new Vector2(0, 0))
        {
            // vsa do something for knockback
            //firstPersonControllerRef.inertia = argInArgs.knockback;
        }

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject.name == "Foreground")
            {
                healthBar = child.gameObject.GetComponent<RectTransform>();
            }
        }
        healthBar.sizeDelta = new Vector2(200 * stats.currentHitPoints / stats.totalHitPoints, healthBar.sizeDelta.y);

    }
    
    private class SourcesAndCooldowns
    {
        public GameObject source;
        public float remainingCooldown = 0.5f;

        public SourcesAndCooldowns(GameObject argInGameObject)
        {
            source = argInGameObject;
            remainingCooldown = 1f;
        }
    }

    private void UpdateDamageCooldowns()
    {
        foreach (SourcesAndCooldowns dmgSrc in damageSourcesInCooldown)
        {
            dmgSrc.remainingCooldown -= Time.deltaTime;
        }

        List<SourcesAndCooldowns> newList = new List<SourcesAndCooldowns>();

        foreach (SourcesAndCooldowns dmgSrc in damageSourcesInCooldown)
        {
            if (dmgSrc.remainingCooldown > 0)
            {
                newList.Add(dmgSrc);
            }
        }
        damageSourcesInCooldown.Clear();
        damageSourcesInCooldown = newList;
    }
    #endregion

    #region old jump and coliders stuff

    void CheckGround()
    {
        grounded = false;
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position + Vector3.down, 0.1f );
        foreach (Collider2D col in colls)
        {
            if (col.GetComponent<FloorTag>() != null)
            {
                grounded = true;
                break;
            }
        }
    }
    
    // stuff for jump down
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<FloorTag>() != null)
        {
            if (Physics2D.GetIgnoreCollision(other, GetCollider()))
            {
                Physics2D.IgnoreCollision(other, GetCollider(), false);
                wantToJumpDown = false;
            }
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if ((other.gameObject.GetComponent<FloorTag>() != null) && (other.gameObject.GetComponent<PlatformEffector2D>()))
        {
            if (other.transform.position.y < this.transform.position.y)
            {
                if (wantToJumpDown)
                {
                    Physics2D.IgnoreCollision(other, GetCollider(), true);
                }
            }
        }
    }
    

    Collider2D GetCollider()
    {
        Collider2D found = null;
        Collider2D[] colls = GetComponents<Collider2D>();
        foreach (Collider2D col in colls)
        {
            if (!col.isTrigger)
            {
                found = col;
            }
        }
        return found;
    }
    #endregion
    
}
