using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SticknessLevel : MonoBehaviour
{
    public float level = 50;

    [Tooltip("points per second. 100 points is max level")]
    public int decreaseRate = 5;
    Image amountImage;
    Animator anim;
	void Awake ()
    {
        amountImage = GameObject.Find("UI").transform.Find("SticknessLevel").transform.Find("Level").GetComponent<Image>();
        StartDecreasing();
        anim = GameObject.Find("UI").transform.Find("SticknessLevel").GetComponent<Animator>();
    }
	
	void Update ()
    {
        amountImage.fillAmount = level / 100f;
	}

    void AutoDecrease()
    {
        level -= decreaseRate;
        if (level < 0) level = 0;
        anim.SetTrigger("decrease");
    }

    public void StartDecreasing()
    {
        if(!IsInvoking("AutoDecrease"))
        {
            InvokeRepeating("AutoDecrease", 1, 1);
        }
    }

    public void DecreaseLevel(float amount)
    {
        level -= amount;
        if (level < 0f) level = 0f;
    }

    public void IncreaseLevel(float amount)
    {
        level += amount;
        if (level > 100f) level = 100f;
    }

    public void Restart()
    {
        level = 100f;
    }

    void OnDestroy()
    {
        CancelInvoke("AutoDecrease");
    }
}
