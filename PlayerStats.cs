using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour, Player, PlayerTarget, DamageAcceptor 
{

    private float movementSpeedDefaultCoef = 1;
    public float movementSpeedCoefModifier { get; set; }

    public string name;
    public float techFound = 0;
    public float totalHitPoints = 1000f;
    public float totalArmorPoints = 1000f;
    public float currentHitPoints;
    public float currentArmorPoints;
    public int currentHitPointsPerc;
    public int currentArmorPointsPerc;
    public bool isDead = false;
    public bool isInvulnerable { get; set; }
    public Vector3 globalObjective { get; set; }
    public float passedMapDistance;

    private Registry registryRef;
    private FirstPersonController firstPersonControllerRef;
    private RaceController raceControllerRef;
    private RandomTileGeneratorScript randomTileGeneratorScriptRef;

    private List<SourcesAndCooldowns> damageSourcesInCooldown = new List<SourcesAndCooldowns>();

    public RaceController.RaceData myRaceData;


    private class SourcesAndCooldowns
    {
        public GameObject source;
        public float remainingCooldown = 1f;

        public SourcesAndCooldowns(GameObject argInGameObject)
        {
            source = argInGameObject;
            remainingCooldown = 1f;
        }
    }
    // Use this for initialization
    void Start()
    {
        globalObjective = new Vector3(0, 10, 3000);
        isInvulnerable = false;
        firstPersonControllerRef = this.gameObject.GetComponent<FirstPersonController>();
        myRaceData = new RaceController.RaceData();
        currentHitPoints = totalHitPoints;
        currentArmorPoints = totalArmorPoints;
        raceControllerRef = GameObject.FindObjectOfType<RaceController>().GetComponent<RaceController>();
        randomTileGeneratorScriptRef = FindObjectOfType<RandomTileGeneratorScript>().GetComponent<RandomTileGeneratorScript>();
        updateMyPointsPercentages();
        updateBotPassedMapDistance();
        updateMyRaceData();
        raceControllerRef.AddRaceData(myRaceData);

        registryRef = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registryRef.damageAcceptors.AddDamageAcceptor(this);
        registryRef.playerTargets.AddPlayerTarget(this);
        groups = new List<string>();
        groups.Add("players");
        movementSpeedCoefModifier = movementSpeedDefaultCoef;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDamageCooldowns();
        updateMyPointsPercentages();
        updateBotPassedMapDistance();
        updateMyRaceData();
        if ((isDead == true) || (isInvulnerable == true)) { Die(); }

        firstPersonControllerRef.UpdateMovement(movementSpeedCoefModifier);
        movementSpeedCoefModifier = movementSpeedDefaultCoef;
    }

    void updateMyRaceData()
    {
        myRaceData.name = name;
        myRaceData.gameObject = this.gameObject;
        myRaceData.hitPoints = currentHitPointsPerc;
        myRaceData.armorPoints = currentArmorPointsPerc;
        myRaceData.passedMapDistance = passedMapDistance;
        myRaceData.isDead = isDead;
        myRaceData.isUser = true;
        myRaceData.isInvulnerable = isInvulnerable;
        myRaceData.distanceFromPlayer = 0;
    }

    private void updateMyPointsPercentages()
    {
        currentHitPointsPerc = (int)((currentHitPoints / totalHitPoints) * 100);
        currentArmorPointsPerc = (int)((currentArmorPoints / totalArmorPoints) * 100);
    }

    private void updateBotPassedMapDistance()
    {
        passedMapDistance = (int)(this.gameObject.transform.position.x / randomTileGeneratorScriptRef.tileGridSize);
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

    public void statsDoDamage(Messaging.DamageMsgArgs args)
    {
        if (damageSourcesInCooldown.Find(x => x.source == args.source) == null)
        {
            float locDamage = args.dmg;
            args.cbIsAccepted = true;

            damageSourcesInCooldown.Add(new SourcesAndCooldowns(args.source));

            if (currentArmorPoints > 0)
            {
                if (currentArmorPoints >= locDamage)
                {
                    currentArmorPoints -= locDamage;
                    locDamage = 0;
                }
                else
                {
                    currentArmorPoints = 0;
                    locDamage -= currentArmorPoints;
                }
            }
            if (locDamage > 0)
            {
                if (currentHitPoints >= locDamage)
                {
                    currentHitPoints -= locDamage;
                    locDamage = 0;
                }
                else
                {
                    currentHitPoints = 0;
                    isDead = true;
                }
            }
        }

        if (args.knockback != new Vector3(0, 0, 0))
        {
            firstPersonControllerRef.inertia = args.knockback;
        }
    }

    public void statsHeal(Messaging.PowerUpMsgArgs args)
    {
        if (currentHitPoints < totalHitPoints)
        {
            float locHeal = args.heal;
            currentHitPoints += locHeal;
            if (currentHitPoints > totalHitPoints)
            {
                currentHitPoints = totalHitPoints;
            }
            args.cbIsAccepted = true;
        }
    }

    public void statsArmor(Messaging.PowerUpMsgArgs args)
    {
        if (currentArmorPoints < totalArmorPoints)
        {
            float locArmor = args.armor;
            currentArmorPoints += locArmor;
            if (currentArmorPoints > totalArmorPoints)
            {
                currentArmorPoints = totalArmorPoints;
            }
            args.cbIsAccepted = true;
        }
    }

    public void statsTech(Messaging.PowerUpMsgArgs args)
    {
        techFound += args.tech;
        args.cbIsAccepted = true;
    }


    private float penaltyTimeout = 0;
    private float invulnerabilityTimeout = 0;
    private bool inPenalty = false;
    private bool inInvulnerability = false;

    private void Die()
    {
        if ((inPenalty == false) && (inInvulnerability == false))
        {
            penaltyTimeout = 4;
            invulnerabilityTimeout = 7;
            inPenalty = true;
            inInvulnerability = true;
            Vector3 placement = BotBirdView.FindClosestNavmeshPoint(this.gameObject.transform.position);
            this.transform.position = placement + Vector3.up * 2;
            firstPersonControllerRef.Halt();
            InvulnerableOn();
        }
        penaltyTimeout -= Time.deltaTime;
        invulnerabilityTimeout -= Time.deltaTime;
        if ((penaltyTimeout <= 0) && (inPenalty == true) && (inInvulnerability == true))
        {
            inPenalty = false;
            penaltyTimeout = 0;
            currentHitPoints = totalHitPoints;
            firstPersonControllerRef.Reset();
            firstPersonControllerRef.Play();
            isDead = false;
        }

        if ((invulnerabilityTimeout <= 0) && (inInvulnerability == true))
        {
            inInvulnerability = false;
            invulnerabilityTimeout = 0;
            InvulnerableOff();
        }
    }

    private void InvulnerableOn()
    {
        isInvulnerable = true;
    }

    private void InvulnerableOff()
    {
        isInvulnerable = false;
    }

    //DamageAcceptor
    public List<string> groups { get; set; }
    public void acceptDamage(Messaging.DamageMsgArgs argInArgs)
    {
        statsDoDamage(argInArgs);
    }
}
