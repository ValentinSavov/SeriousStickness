using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StickStats : MonoBehaviour
{
    public float attackSpeed = 20; // 0 - 100 = 0 - 1Hz ; it does not work for player, the player depends on weapon firerate and mouse click
    public float moveSpeed = 3; // m/s
    public float jumpStartSpeed = 10; // m/s

    public float totalHitPoints = 1000f;
    public float totalArmorPoints = 0;
    public float currentHitPoints;
    public float currentArmorPoints;

    public bool isDead = false;







}


