﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SticknessLevel : MonoBehaviour
{
    public int level = 50;

    [Tooltip("points per secomd. 100 points is max level")]
    public int decreaseRate = 5;
    Image amountImage;

	void Awake ()
    {
        amountImage = transform.Find("Level").GetComponent<Image>();
        InvokeRepeating("AutoDecrease", 1, 1);
    }
	
	void Update ()
    {

        amountImage.fillAmount = level / 100f;
	}

    void AutoDecrease()
    {
        level -= decreaseRate;
        if (level < 0) level = 0;
    }

    public void DecreaseLevel(int amount)
    {
        level -= amount;
        if (level < 0) level = 0;
    }

    public void IncreaseLevel(int amount)
    {
        level += amount;
        if (level > 100) level = 100;
    }

    void OnDestroy()
    {
        CancelInvoke("AutoDecrease");
    }
}