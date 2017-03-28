using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class WindsOfChangeEffectControl : MonoBehaviour
{
    bool active = false;
    private Registry registryRef;
    Transform effectTransform;

    void Start ()
    {
        registryRef = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        effectTransform = this.gameObject.transform.FindChild("WindsOfChangeEffectPosition").gameObject.transform;
        Deactivate();
    }
	

	void Update ()
    {
        if (active == true)
        {
            List<PlayerTarget> affected = registryRef.playerTargets.GetAllPlayersInRange(new Vector2(effectTransform.position.x, effectTransform.position.z), 8f);
            foreach (PlayerTarget aff in affected)
            {
                EffectsRegistry.Effect myEffect = new EffectsRegistry.Effect(aff, myEffectHandler);
                myEffect.isSingleFrame = true;
                registryRef.effects.AddEffect(myEffect);
            }
        }
	}

    void myEffectHandler(EffectsRegistry.Effect argInEffect)
    {
        PropertyInfo prop = argInEffect.player.GetType().GetProperty("movementSpeedCoefModifier", BindingFlags.Public | BindingFlags.Instance);
        if (null != prop && prop.CanWrite)
        {
            prop.SetValue(argInEffect.player, 0.5f, null);
        }
    }

    public void Shoot(GameObject argInTargetGO)
    {
        this.gameObject.transform.position = new Vector3(argInTargetGO.transform.position.x,2,argInTargetGO.transform.position.z);
        Activate();
    }

    public void Activate()
    {
        Component[] childComps = this.transform.GetComponentsInChildren<Component>();
        foreach (Component childComp in childComps)
        {
            SetComponentActive(childComp, true);
        }
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
            if (!((component is WindsOfChangeEffectControl) || (component is NotTerrainMarker)))
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
