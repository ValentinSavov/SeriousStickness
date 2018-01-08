using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GearItem
{
    public GameObject gamePref;
    public GameObject guiPref;
}

public class GearDatabase : MonoBehaviour
{
    public List<GearItem> weapons = new List<GearItem>();
}
