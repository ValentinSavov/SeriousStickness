using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StickStats : MonoBehaviour
{
    [Tooltip("0 - 100 = 0 - 1Hz.\nIt does not work for player, the player depends on weapon firerate and mouse click")]
    public float attackSpeed = 20; // 0 - 100 = 0 - 1Hz ; not applicable for some creeps(ex. kamikadze) and player, the player depends on weapon firerate and mouse click
    //[Tooltip("not used, movement controller is the real stuff")]
    //public float moveSpeed = 15f; // m/s
    //public float jumpSpeed = 80000f; // m/s

    public float totalHitPoints = 100;
    public float totalArmorPoints = 0;
    public float currentHitPoints;
    public float currentArmorPoints;

    public bool isDead = false;

    
}


