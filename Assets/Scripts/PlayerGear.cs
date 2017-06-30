using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGear : MonoBehaviour
{
    [System.Serializable]
    public class InventoryWeapon
    {
        public string name;
        [HideInInspector]
        public GameObject weaponGO;
        public float bullets;
        public float durability = 100f; // 0 - 100
    }
    public List<InventoryWeapon> availableWeapons = new List<InventoryWeapon>();

    public int selectedWeapon = 0;

    GameObject cursor;
    GearDatabase gearDatabase;
    TextMesh ammotext;

    void Start()
    {
        ammotext = GameObject.Find("Ammo").GetComponent<TextMesh>();
        Transform weaponSpot = GetComponentInChildren<WeaponSpot>().transform;
        cursor = GameObject.FindObjectOfType<Cursor>().gameObject;
        gearDatabase = GameObject.FindObjectOfType<GearDatabase>();

        // get from database and instantiate all weapons that shall be available
        if (availableWeapons.Count != 0)
        {
            selectedWeapon = (int)Mathf.Clamp(selectedWeapon, 0, availableWeapons.Count);

            for (int i = 0; i < availableWeapons.Count; i++)
            {
                GameObject weap = Instantiate(gearDatabase.weapons.Find(x => x.gamePref.name == availableWeapons[i].name).gamePref, weaponSpot) as GameObject;
                availableWeapons[i].weaponGO = weap;
                weap.transform.localPosition = Vector3.zero;
                weap.transform.localRotation = Quaternion.identity;
                weap.transform.localScale = Vector3.one;
                weap.SetActive(false);

                //vsa do the same for UI prefabs
            }

            availableWeapons[selectedWeapon].weaponGO.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Engage();
        }
        else if (Input.GetButton("Fire1") == true)
        {
            Weapon activeWeap = availableWeapons[selectedWeapon].weaponGO.GetComponent<Weapon>();
            if (activeWeap != null)
            {
                if (activeWeap.isAutomatic)
                {
                    Engage();
                }
            }
        }


        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            PrevWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            NextWeapon();
        }

        //vsa javar takuv, mahni tva ot tuka
        if (ammotext != null)
        {
            ammotext.text = "Ammo: " + availableWeapons[selectedWeapon].bullets.ToString();
        }
    }

    void Engage()
    {
        Weapon activeWeap = availableWeapons[selectedWeapon].weaponGO.GetComponent<Weapon>();
        if (activeWeap != null)
        {
            if ((availableWeapons[selectedWeapon].bullets > 0f) && (availableWeapons[selectedWeapon].durability > 0f))
            {
                if (activeWeap.Engage(cursor) == true)
                {
                    availableWeapons[selectedWeapon].bullets--;
                    //vsa do something for durability
                }
            }
            else
            {
                //vsa effect for no ammo
            }
        }
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Weapon>() != null)
        {
            if (other.gameObject.GetComponent<Weapon>().GetComponentInParent<WeaponSpot>() == null)
            {
                //if it is a real weapon, existing in the database
                GearItem otherGearItemFromDatabase = gearDatabase.weapons.Find(x => x.gamePref.name == other.gameObject.name);
                if (otherGearItemFromDatabase != null)
                {
                    Transform weaponSpot = GetComponentInChildren<WeaponSpot>().transform;
                    InventoryWeapon newiw = new InventoryWeapon();
                    newiw.name = otherGearItemFromDatabase.gamePref.name;

                    // if already have it
                    InventoryWeapon contained = availableWeapons.Find(x => x.name == newiw.name);
                    if (contained != null)
                    {
                        //just add some stuff to the existing
                        contained.bullets += 10f;
                        contained.durability = 100f;
                    }
                    else
                    {
                        //add the new weapon to player gear
                        GameObject instantiatedWeap = Instantiate(otherGearItemFromDatabase.gamePref, weaponSpot) as GameObject;
                        instantiatedWeap.transform.localPosition = Vector3.zero;
                        instantiatedWeap.transform.localRotation = Quaternion.identity;
                        instantiatedWeap.transform.localScale = Vector3.one;
                        instantiatedWeap.SetActive(false);


                        newiw.weaponGO = instantiatedWeap;
                        newiw.bullets = 10f;
                        newiw.durability = 100f;
                        availableWeapons.Add(newiw);
                    }
                    Destroy(other.gameObject);
                }
            }
        }
    }

    public void NextWeapon()
    {
        availableWeapons[selectedWeapon].weaponGO.SetActive(false);
        selectedWeapon++;
        if (selectedWeapon >= (availableWeapons.Count))
        {
            selectedWeapon = 0;
        }
        availableWeapons[selectedWeapon].weaponGO.SetActive(true);
        //vsa do the same for UI
    }
    public void PrevWeapon()
    {
        availableWeapons[selectedWeapon].weaponGO.SetActive(false);
        selectedWeapon--;
        if (selectedWeapon < 0)
        {
            selectedWeapon = availableWeapons.Count - 1;
        }
        availableWeapons[selectedWeapon].weaponGO.SetActive(true);

        //vsa do the same for UI
    }
    public void SetSelectedWeapon(int selected)
    {
        if ((selected >= 0) && (selected < availableWeapons.Count))
        {
            availableWeapons[selectedWeapon].weaponGO.SetActive(false);
            selectedWeapon = selected;
            availableWeapons[selectedWeapon].weaponGO.SetActive(true);
        }

        //vsa do the same for UI
    }

    public GameObject GetSelectedWeapon()
    {
        return availableWeapons[selectedWeapon].weaponGO;
    }


}
