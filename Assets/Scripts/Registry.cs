using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Registry : MonoBehaviour 
{
    public static Registry instance;
    public DamageAcceptorRegistry damageAcceptors;
    public ObjectivesRegistry objectives;

    Registry()
    {
        damageAcceptors = new DamageAcceptorRegistry();
        objectives = new ObjectivesRegistry();
        instance = this;
    }

	void Update ()
    {
        //effects.updateEffects();
    }

    public void Reset()
    {
        damageAcceptors = new DamageAcceptorRegistry();
        objectives = new ObjectivesRegistry();
    }
}




#region DamageAcceptor
/// <summary>
/// ---------------------------------------------------------------          DamageAcceptors
/// </summary>
/// 

public interface DamageProvider
{
    void ReportKill(DamageAcceptor killed);
}

public interface DamageAcceptor
{
    void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs);
    List<string> groups { get; set; }
}

public class DamageAcceptorRegistry
{
    public class DamageArgs
    {
        public float dmg = 0;
        public string type = "";
        //public GameObject affectedPart;
        public Vector2 knockback = new Vector2(0,0);
        public GameObject source;
        //public List<string> affectedGroups;
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

    public List<DamageAcceptor> GetAcceptorsInGroup(List<string> argInGroups)
    {
        return damageAcceptors.FindAll(x => (x.groups.Intersect(argInGroups).Any()) );
    }

    public int GetAcceptorsCountInGroup(List<string> argInGroups)
    {
        return damageAcceptors.FindAll(x => (x.groups.Intersect(argInGroups).Any())).Count;
    }

    public List<DamageAcceptor> GetAcceptorsInRange(Vector3 point, float radius)
    {
        return damageAcceptors.FindAll(x => ( (((Component)x).gameObject.transform.position - point).magnitude < radius ));
    }
    /*
    public void doTargetDamage(DamageAcceptor damageAcceptor, GameObject argInSource, float argInDmg, string argInDamageType, Vector2 argInKnockBack)
    {
        if (damageAcceptor != null)
        {
            if (((Component)damageAcceptor).gameObject != argInSource)
            {
                    DamageArgs args = new DamageArgs();
                    args.knockback = argInKnockBack;
                    args.dmg = argInDmg;
                    args.source = argInSource;
                    args.type = argInDamageType;

                    damageAcceptor.acceptDamage(args);
            }
        }
    }*/
    
    public void doTargetDamage(DamageAcceptor damageAcceptor, GameObject argInSource, float argInDmg, string argInDamageType, Vector2 argInKnockBack/*,  List<string> argInIgnoreGroups*/)
    {
        if (damageAcceptor != null)
        {
            if (((Component)damageAcceptor).gameObject != argInSource)
            {
                //if (!(damageAcceptor.groups.Intersect(argInIgnoreGroups).Any()))
                {
                    DamageArgs args = new DamageArgs();
                    args.knockback = argInKnockBack;
                    args.dmg = argInDmg;
                    args.source = argInSource;
                    args.type = argInDamageType;

                    damageAcceptor.acceptDamage(args);
                }
            }
        }
    }

    public void doAreaDamage(GameObject argInSource, Vector2 argInCentralPoint, float argInAffectRadius, float argInDmg, string argInDamageType, float argInKnockBack)
    {
        foreach(DamageAcceptor damageAcceptor in damageAcceptors.ToList())
        {
            //if (((Component)damageAcceptor).gameObject != argInSource)
            {
                if (Vector2.Distance(((Component)damageAcceptor).gameObject.transform.position, argInCentralPoint) < argInAffectRadius)
                {
                    DamageArgs args = new DamageArgs();
                    args.knockback =
                        (((Vector2)((Component)damageAcceptor).gameObject.transform.position) - argInCentralPoint).normalized 
                        * argInKnockBack
                        * (1 - (((Vector2)((Component)damageAcceptor).gameObject.transform.position) - argInCentralPoint).magnitude / argInAffectRadius );
                    args.dmg = argInDmg;
                    args.source = argInSource;
                    args.type = argInDamageType;
                    
                    damageAcceptor.acceptDamage(args);
                }
            }
        }
    }

    /*
    public void doAreaDamage(GameObject argInSource, Vector2 argInCentralPoint, float argInAffectRadius, float argInDmg, string argInDamageType, float argInKnockBack, List<string> argInIgnoreGroups)
    {
        foreach (DamageAcceptor damageAcceptor in damageAcceptors.ToList())
        {
            //if (((Component)damageAcceptor).gameObject != argInSource)
            {
                Vector2 AcceptorV2Point = new Vector2(((Component)damageAcceptor).gameObject.transform.position.x, ((Component)damageAcceptor).gameObject.transform.position.z);
                if (Vector2.Distance(AcceptorV2Point, argInCentralPoint) < argInAffectRadius)
                {
                    if (!(damageAcceptor.groups.Intersect(argInIgnoreGroups).Any()))
                    {
                        DamageArgs args = new DamageArgs();
                        args.knockback = (((Vector2)((Component)damageAcceptor).gameObject.transform.position) - argInCentralPoint).normalized * argInKnockBack;
                        args.dmg = argInDmg;
                        args.source = argInSource;
                        args.type = argInDamageType;

                        damageAcceptor.acceptDamage(args);
                    }
                }
            }
        }
    }*/
}
#endregion

#region Objectives
public class Trigger : MonoBehaviour
{
    public string objective;
    Registry registry;
    void Start()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        if (registry.objectives.GetObjective(objective) != null)
        {
            registry.objectives.GetObjective(objective).AddTrigger(this);
        }
        else
        {
            
            this.Invoke("Start", 0.1f);
        }   
    }
    public void Set()
    {
        if(registry.objectives.GetObjective(objective))
        registry.objectives.GetObjective(objective).RemoveTrigger(this);
    }
}

public abstract class Objective : MonoBehaviour
{
    public string objectiveName;
    public List<Trigger> unresolvedMandatoryTriggers = new List<Trigger>();
    Registry registry;
    void Start()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.objectives.AddObjective(this);
    }
    public void AddTrigger(Trigger argInTrigger)
    {
        unresolvedMandatoryTriggers.Add(argInTrigger);
    }
    public void RemoveTrigger(Trigger argInTrigger)
    {
        unresolvedMandatoryTriggers.RemoveAll(x => x == argInTrigger);
        if(unresolvedMandatoryTriggers.Count <= 0)
        {
            ProcessCompletionAction();
        }
    }
    public abstract void ProcessCompletionAction();
}

public class ObjectivesRegistry
{
    List<Objective> objectives = new List<Objective>();

    public void AddObjective(Objective argInObjective)
    {
        objectives.Add(argInObjective);
    }
    public void RemoveObjective(Objective argInObjective)
    {
        objectives.RemoveAll(x => x == argInObjective);
    }
    public Objective GetObjective(string name)
    {
        return objectives.Find(x => x.objectiveName == name);
    }
    public int GetObjectivesCount()
    {
        return objectives.Count;
    }
}

#endregion

#region Effects
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
#endregion

#region PlayerTarget
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
        //Vector3 currentPosition = argInMyPosition;
        foreach (PlayerTarget potentialTarget in playerTargets)
        {
            if (potentialTarget.isInvulnerable == false)
            {
                float distance = Vector3.Distance(((Component)potentialTarget).gameObject.transform.position, argInMyPosition);
                float dist = Mathf.Abs(distance - argInDistance);

                if (dist < closestDistanceSqr)
                {
                    closestDistanceSqr = dist;
                    bestTargetNonInv = potentialTarget;
                }

            }
            else
            {
                float distance = Vector3.Distance(((Component)potentialTarget).gameObject.transform.position, argInMyPosition);
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
#endregion

#region Weapons
/// <summary>
/// ---------------------------------------------------------------          Weapons
/// </summary>

//not used for now
public class WeaponsRegistry
{
    private List<GameObject> weapons = new List<GameObject>();

    public void AddWeapon(GameObject weap)
    {
        weapons.Add(weap);
    }

    public void RemoveWeapon(GameObject weap)
    {
        if (weapons.Contains(weap))
        {
            weapons.Remove(weap);
        }
    }

    public bool IsThereAWeapon()
    {
        return (weapons.Count != 0);
    }

    public GameObject FindBestWeaponForMe(GameObject bot)
    {
        if (weapons.Count != 0)
        {
            GameObject best = weapons[0];

            foreach (GameObject w in weapons)
            {
                //vsa can add more conditions for "best"
                if ((w.transform.position - bot.transform.position).magnitude < (best.transform.position - bot.transform.position).magnitude)
                {
                    best = w;
                }
            }
            return best;
        }
        return null;
    }

    public bool WeaponExists(GameObject weap)
    {
        return weapons.Contains(weap);
    }
}
#endregion
