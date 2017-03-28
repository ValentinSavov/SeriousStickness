using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Registry : MonoBehaviour 
{
    public DamageAcceptorRegistry damageAcceptors;
    public PlayerTargetsRegistry playerTargets;
    public DangerRegistry dangers;
    public EffectsRegistry effects;

    // Use this for initialization
    void Start () 
    {
        damageAcceptors = new DamageAcceptorRegistry();
        playerTargets = new PlayerTargetsRegistry();
        dangers = new DangerRegistry();
        effects = new EffectsRegistry();
    }
	
	// Update is called once per frame
	void Update ()
    {
        effects.updateEffects();
    }
}

/// <summary>
/// ---------------------------------------------------------------          DamageAcceptors
/// </summary>
/// 

public interface DamageAcceptor
{
    void acceptDamage(Messaging.DamageMsgArgs argInArgs);
    List<string> groups { get; set; }
}

public class DamageAcceptorRegistry
{
    public class DamageArgs
    {
        public float dmg = 0;
        public string type = "";
        public float knockbackCoef = 0;
        public GameObject source;
        public List<string> affectedGroups;

        public bool cbIsAccepted = false;
    }

    private List<DamageAcceptor> damageAcceptors = new List<DamageAcceptor>();

    public void AddDamageAcceptor(DamageAcceptor argInDamageAcceptor)
    {
        damageAcceptors.Add(argInDamageAcceptor);
    }

    public void RemoveDamageAcceptor(DamageAcceptor argInDamageAcceptor)
    {
        damageAcceptors.RemoveAll(x => x == argInDamageAcceptor);
    }

    public void doAreaDamage(GameObject argInSource, Vector3 argInCentralPoint, float argInAffectRadius, string argInDamageType, Vector3 argInKnockBack)
    {
        foreach(DamageAcceptor damageAcceptor in damageAcceptors)
        {
            if (((Component)damageAcceptor).gameObject != argInSource)
            {
                if (Vector3.Distance(((Component)damageAcceptor).gameObject.transform.position, argInCentralPoint) < argInAffectRadius)
                {
                    Messaging.DamageMsgArgs args = new Messaging.DamageMsgArgs();
                    Vector3 knockback = argInKnockBack;  //((((Component)damageAcceptor).gameObject.transform.position - argInCentralPoint).normalized) * knockBackCoef;
                    args.knockback = knockback;
                    args.dmg = 250;
                    args.source = argInSource;
                    args.type = argInDamageType;

                    damageAcceptor.acceptDamage(args);
                }
            }
        }
    }

    public void doAreaDamage(GameObject argInSource, Vector2 argInCentralPoint, float argInAffectRadius, string argInDamageType, Vector3 argInKnockBack)
    {
        foreach (DamageAcceptor damageAcceptor in damageAcceptors)
        {
            if (((Component)damageAcceptor).gameObject != argInSource)
            {
                Vector2 AcceptorV2Point = new Vector2(((Component)damageAcceptor).gameObject.transform.position.x, ((Component)damageAcceptor).gameObject.transform.position.z);
                if (Vector2.Distance(AcceptorV2Point, argInCentralPoint) < argInAffectRadius)
                {
                    Messaging.DamageMsgArgs args = new Messaging.DamageMsgArgs();
                    Vector3 knockback = argInKnockBack;  //((((Component)damageAcceptor).gameObject.transform.position - argInCentralPoint).normalized) * knockBackCoef;
                    args.knockback = knockback;
                    args.dmg = 250;
                    args.source = argInSource;
                    args.type = argInDamageType;

                    damageAcceptor.acceptDamage(args);
                }
            }
        }
    }

    public void doAreaDamage(GameObject argInSource, Vector2 argInCentralPoint, float argInAffectRadius, string argInDamageType, Vector3 argInKnockBack, List<string> argInIgnoreGroups)
    {
        foreach (DamageAcceptor damageAcceptor in damageAcceptors)
        {
            if (((Component)damageAcceptor).gameObject != argInSource)
            {
                Vector2 AcceptorV2Point = new Vector2(((Component)damageAcceptor).gameObject.transform.position.x, ((Component)damageAcceptor).gameObject.transform.position.z);
                if (Vector2.Distance(AcceptorV2Point, argInCentralPoint) < argInAffectRadius)
                {
                    if (!(damageAcceptor.groups.Intersect(argInIgnoreGroups).Any()))
                    {
                        Messaging.DamageMsgArgs args = new Messaging.DamageMsgArgs();
                        Vector3 knockback = argInKnockBack;
                        args.knockback = knockback;
                        args.dmg = 250;
                        args.source = argInSource;
                        args.type = argInDamageType;

                        damageAcceptor.acceptDamage(args);
                    }
                }
            }
        }
    }
}

/// <summary>
/// ---------------------------------------------------------------          Effects
/// </summary>
/// 

public class EffectsRegistry
{
    public class Effect
    {
        public delegate void EffectHandler(Effect argInMe);

        public bool isSingleFrame;
        public float duration;
        public float passedDuration;

        public bool hasPulses;
        public int currnetPulse;
        public float pulseDuration;
        public float passedPulseDuration;

        public PlayerTarget player;

        public EffectHandler MyEffectHandler;

        public Effect(PlayerTarget argInPlayer, EffectHandler argInEffectHandler)
        {
            MyEffectHandler = argInEffectHandler;
            player = argInPlayer;
        }
    }

    private List<Effect> effects = new List<Effect>();

    public void AddEffect(Effect argInEffect)
    {
        effects.Add(argInEffect);
    }

    public void RemoveEffect(Effect argInEffect)
    {
        effects.RemoveAll(x => x == argInEffect);
    }

    public void updateEffects()
    {
        foreach (Effect effect in effects)
        {
            if (effect.isSingleFrame == false)
            {
                effect.passedDuration += Time.deltaTime;
            }
            effect.MyEffectHandler(effect);
        }
        effects.RemoveAll(x => x.isSingleFrame==true);
        effects.RemoveAll(x => x.passedDuration>x.duration);
    }
}

/// <summary>
/// ---------------------------------------------------------------          PlayerTarget
/// </summary>

public interface PlayerTarget
{
    bool isInvulnerable { get; set; }
    Vector3 globalObjective { get; set; }
}

public class PlayerTargetsRegistry
{

    private List<PlayerTarget> playerTargets = new List<PlayerTarget>();

    public void AddPlayerTarget(PlayerTarget argInPlayerTarget)
    {
        playerTargets.Add(argInPlayerTarget);
    }
    public void RemovePlayerTarget(PlayerTarget argInPlayerTarget)
    {
        playerTargets.RemoveAll(x => x == argInPlayerTarget);
    }

    public List<PlayerTarget> GetAllPlayersInRange(Vector2 argInCentralPoint, float argInAffectRadius)
    {
        List<PlayerTarget> result = new List<PlayerTarget>();
        foreach (PlayerTarget potentialTarget in playerTargets)
        {
            Vector2 AcceptorV2Point = new Vector2(((Component)potentialTarget).gameObject.transform.position.x, ((Component)potentialTarget).gameObject.transform.position.z);
            if (Vector2.Distance(AcceptorV2Point, argInCentralPoint) < argInAffectRadius)
            {
                result.Add(potentialTarget);
            }
        }
        return result;
    }

    public PlayerTarget GetPlayerClosestToItsObjective(PlayerTarget argInCaller)
    {
        PlayerTarget bestTargetNonInv = null;
        PlayerTarget bestTargetInv = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (PlayerTarget potentialTarget in playerTargets)
        {
            if (potentialTarget != argInCaller)
            { 
                if (potentialTarget.isInvulnerable == false)
                {
                    float dist = Vector3.Distance(((Component)potentialTarget).gameObject.transform.position, potentialTarget.globalObjective);

                    if (dist < closestDistanceSqr)
                    {
                        closestDistanceSqr = dist;
                        bestTargetNonInv = potentialTarget;
                    }
                }
                else
                {
                    float dist = Vector3.Distance(((Component)potentialTarget).gameObject.transform.position, potentialTarget.globalObjective);
                    if (dist < closestDistanceSqr)
                    {
                        closestDistanceSqr = dist;
                        bestTargetInv = potentialTarget;
                    }
                }
            }
        }

        if (bestTargetNonInv != null)
        {
            return bestTargetNonInv;
        }

        return bestTargetInv;
    }

    public PlayerTarget GetClosestPlayerToDistance(Vector3 argInMyPosition, float argInDistance)
    {
        PlayerTarget bestTargetNonInv = null;
        PlayerTarget bestTargetInv = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = argInMyPosition;
        foreach (PlayerTarget potentialTarget in playerTargets)
        {
            if (potentialTarget.isInvulnerable == false)
            {
                float distance = Vector3.Distance(((Component)potentialTarget).gameObject.transform.position,argInMyPosition);
                float dist = Mathf.Abs(distance - argInDistance);

                if (dist < closestDistanceSqr)
                {
                    closestDistanceSqr = dist;
                    bestTargetNonInv = potentialTarget;
                }

            }
            else
            {
                float distance = Vector3.Distance(((Component)potentialTarget).gameObject.transform.position,argInMyPosition);
                float dist = Mathf.Abs(distance - argInDistance);
                if (dist < closestDistanceSqr)
                {
                    closestDistanceSqr = dist;
                    bestTargetInv = potentialTarget;
                }
            }
        }

        if (bestTargetNonInv != null)
        {
            return bestTargetNonInv;
        }

        return bestTargetInv;
    }

    public PlayerTarget GetClosestPlayerTarget(Vector3 argInMyPosition)
    {
        PlayerTarget bestTargetNonInv = null;
        PlayerTarget bestTargetInv = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = argInMyPosition;
        foreach (PlayerTarget potentialTarget in playerTargets)
        {
            if (potentialTarget.isInvulnerable == false)
            {
                Vector3 directionToTarget = ((Component)potentialTarget).gameObject.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTargetNonInv = potentialTarget;
                }
            }
            else
            {
                Vector3 directionToTarget = ((Component)potentialTarget).gameObject.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTargetInv = potentialTarget;
                }
            }
        }

        if (bestTargetNonInv != null)
        {
            return bestTargetNonInv;
        }

        return bestTargetInv;
    }
}


/// <summary>
/// ---------------------------------------------------------------          Danger
/// </summary>

public interface Danger
{
    bool PointIsDangerous(Vector3 argInPoint);
}

public class DangerRegistry
{

    private List<Danger> dangers = new List<Danger>();

    public void AddDanger(Danger argInDanger)
    {
        dangers.Add(argInDanger);
    }
    public void RemoveDanger(Danger argInDanger)
    {
        dangers.RemoveAll(x => x == argInDanger);
    }

    public bool PointIsDangerous(Vector3 argInMyPoint)
    {
        bool result = false;
        foreach (Danger danger in dangers)
        {
            if (danger.PointIsDangerous(argInMyPoint)==true)
            {
                result = true;
                break;
            }
        }
        return result;
    }
}