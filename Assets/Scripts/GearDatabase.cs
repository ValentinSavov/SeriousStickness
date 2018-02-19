using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GearDatabaseItem
{
    public GameObject gamePref;
    public GameObject guiPref;
}

public class GearDatabase : MonoBehaviour
{
    public List<GearDatabaseItem> weapons = new List<GearDatabaseItem>();
}
