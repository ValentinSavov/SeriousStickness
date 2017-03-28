using UnityEngine;
using System.Collections;

public class ExplodeControl : MonoBehaviour 
{
    private Registry registryRef;
    private static RaceController raceControllerScriptRef;
    private float timeExisting = 0;
    private bool active = false;
	// Use this for initialization
	void Start () 
    {
        if (raceControllerScriptRef == null)
        {
            raceControllerScriptRef = FindObjectOfType<RaceController>().GetComponent<RaceController>();
        }
	}

	void Update () 
    {
        if (active == true)
        {
            timeExisting += Time.deltaTime;
            if (timeExisting > 4)
            {
                Deactivate();
                //GameObject.Destroy(this.gameObject);
            }
        }
	}

    void ExplosionEffect()
    {
        if (registryRef == null)
        {
            registryRef = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        }
        registryRef.damageAcceptors.doAreaDamage(
            this.gameObject, 
            this.gameObject.transform.position, 
            6, "Fire", (Vector3.up));
        /*
        if (raceControllerScriptRef == null)
        {
            raceControllerScriptRef = FindObjectOfType<RaceController>().GetComponent<RaceController>();
        }
        foreach (RaceController.RaceData playerData in raceControllerScriptRef.players)
        {
            Player playerComp = (Player)playerData.gameObject.GetComponentInChildren<Player>();
            if (playerComp != null)
            {
                if (Vector3.Distance(playerData.gameObject.transform.position, this.gameObject.transform.position) < 6f)
                {

                    float coef = (6 - Vector3.Distance(playerData.gameObject.transform.position, this.gameObject.transform.position)) / 6;
                    float damage = 250 * coef;

                    Vector3 posForCalc = this.gameObject.transform.position;
                    posForCalc.x += 2;
                    posForCalc.y -= 3.5f;

                    Vector3 knockback = ((playerData.gameObject.transform.position - posForCalc).normalized) * 3.5f * coef;


                    Messaging.DamageMsgArgs args = new Messaging.DamageMsgArgs();
                    args.dmg = damage;
                    args.source = this.gameObject;
                    args.type = "Fire";
                    args.knockback = knockback;

                    playerComp.statsDoDamage(args);
                }
            }
        }*/
    }

    public void Activate(Vector3 argInMyPosition)
    {
        this.transform.position = new Vector3(argInMyPosition.x, argInMyPosition.y, argInMyPosition.z);
        Component[] childComps = this.transform.GetComponentsInChildren<Component>();
        foreach (Component childComp in childComps)
        {
            SetComponentActive(childComp, true);
        }
        ExplosionEffect();
        timeExisting = 0;
        active = true;
    }

    public void Deactivate()
    {
        Component[] childComps = this.transform.GetComponentsInChildren<Component>();
        foreach (Component childComp in childComps)
        {
            SetComponentActive(childComp, false);
        }
        active = false;
    }

    void SetComponentActive(Component component, bool argInSetActive)
    {
        if (component is Renderer)
        {
            (component as Renderer).enabled = argInSetActive;
        }
        else if (component is Collider)
        {
            (component as Collider).enabled = argInSetActive;
        }
        else if (component is Behaviour)
        {
            if (!((component is ExplodeControl) || (component is NotTerrainMarker)))
            {
                (component as Behaviour).enabled = argInSetActive;
            }
        }
        else if (component is ParticleSystem)
        {
            if (argInSetActive == true)
            {
                (component as ParticleSystem).Play();
            }
            else
            {
                (component as ParticleSystem).Stop();
            }
        }
    }
}
