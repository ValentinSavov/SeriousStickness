using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public enum ItemType
{
    weapon,
}*/
[System.Serializable]
public class GearItem
{
    //public string name;
    //public ItemType type;
    public GameObject gamePref;
    public GameObject guiPref;
}

public class GearDatabase : MonoBehaviour
{
    public List<GearItem> weapons = new List<GearItem>();
}
