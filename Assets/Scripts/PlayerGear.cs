using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGear : MonoBehaviour
{
    [System.Serializable]
    public class InventoryWeapon
    {
        public string name;
        [HideInInspector]
        public GameObject weaponGO;
        [HideInInspector]
        public GameObject weaponUIGO;
        public float bullets;
        public bool infiniteBullets;
        //public float durability = 100f; // 0 - 100
    }
    public List<InventoryWeapon> availableWeapons = new List<InventoryWeapon>();

    public int selectedWeapon = 0;

    public List<AudioClip> changeWeapSounds = new List<AudioClip>();
    public List<AudioClip> takeAmmoSounds = new List<AudioClip>();

    GameObject cursor;
    GearDatabase gearDatabase;
    Text ammotext;
    AudioSource audioSource;
    void Start()
    {
        ammotext = GameObject.Find("UI").transform.Find("Ammo").GetComponent<Text>();
        Transform weaponSpot = GetComponentInChildren<WeaponSpot>().transform;
        audioSource = weaponSpot.gameObject.GetComponent<AudioSource>();
        cursor = GameObject.FindObjectOfType<CursorTag>().gameObject;
        gearDatabase = GameObject.FindObjectOfType<GearDatabase>();
        RectTransform guiWeaponSpot = GameObject.Find("UI").transform.Find("ActiveWeapon").GetComponent<RectTransform>();

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

                //do the same for UI gameobjects
                GameObject weapUI = Instantiate(gearDatabase.weapons.Find(x => x.guiPref.name == "UI"+availableWeapons[i].name).guiPref, guiWeaponSpot) as GameObject;
                weapUI.GetComponent<RectTransform>().localPosition = Vector3.zero;
                availableWeapons[i].weaponUIGO = weapUI;
                weapUI.SetActive(false);

                //ui weapon slots adjust
                GameObject weaponSlot = GameObject.Find("UI").transform.Find("Weapons").Find(i.ToString()).gameObject;
                if(weaponSlot != null)
                {
                    weaponSlot.transform.Find(availableWeapons[i].name).gameObject.SetActive(true);
                }
            }

            availableWeapons[selectedWeapon].weaponGO.SetActive(true);
            availableWeapons[selectedWeapon].weaponUIGO.SetActive(true);

            
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
            //Weapon activeWeap = availableWeapons[selectedWeapon].weaponGO.GetComponent<Weapon>();
            //if (activeWeap != null)
            {
                //if (activeWeap.isAutomatic)
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSelectedWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSelectedWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSelectedWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetSelectedWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetSelectedWeapon(4);
        }

        //vsa javar takuv, mahni tva ot tuka
        if (ammotext != null)
        {
            ammotext.text = availableWeapons[selectedWeapon].bullets.ToString();
        }
    }
    
    void Engage()
    {
        Weapon activeWeap = availableWeapons[selectedWeapon].weaponGO.GetComponent<Weapon>();
        if (activeWeap != null)
        {
            if (((availableWeapons[selectedWeapon].bullets > 0f) || (availableWeapons[selectedWeapon].infiniteBullets)) /*&& (availableWeapons[selectedWeapon].durability > 0f)*/)
            {
                if (activeWeap.Engage(cursor.transform.position) == true)
                {
                    if (!availableWeapons[selectedWeapon].infiniteBullets)
                    {
                        availableWeapons[selectedWeapon].bullets--;
                    }
                    //vsa do something for durability
                }
            }
            else
            {
                //vsa effect for no ammo
                if (!audioSource.isPlaying)
                {
                    PlayAmmoSound();
                }
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
                GearDatabaseItem otherGearItemFromDatabase = gearDatabase.weapons.Find(x => other.gameObject.name.Contains(x.gamePref.name) );
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
                        contained.bullets += Random.Range(5, 50);
                        //contained.durability = 100f;
                        PlayAmmoSound();
                    }
                    else
                    {
                        //add the new weapon to player gear
                        GameObject instantiatedWeap = Instantiate(otherGearItemFromDatabase.gamePref, weaponSpot) as GameObject;
                        instantiatedWeap.transform.localPosition = Vector3.zero;
                        instantiatedWeap.transform.localRotation = Quaternion.identity;
                        instantiatedWeap.transform.localScale = Vector3.one;
                        instantiatedWeap.SetActive(false);

                        //do the same for UI gameobjects
                        RectTransform guiWeaponSpot = GameObject.Find("UI").transform.Find("ActiveWeapon").GetComponent<RectTransform>();
                        GameObject instantiatedWeapUI = Instantiate(otherGearItemFromDatabase.guiPref, guiWeaponSpot) as GameObject;
                        instantiatedWeapUI.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        instantiatedWeapUI.SetActive(false);
                        
                        newiw.weaponGO = instantiatedWeap;
                        newiw.weaponUIGO = instantiatedWeapUI;
                        newiw.bullets = other.gameObject.GetComponent<Weapon>().bullets;
                        availableWeapons.Add(newiw);
                        SetSelectedWeapon(availableWeapons.IndexOf(newiw));

                        //ui weapon slots adjust
                        GameObject weaponSlot = GameObject.Find("UI").transform.Find("Weapons").Find(availableWeapons.IndexOf(newiw).ToString()).gameObject;
                        if (weaponSlot != null)
                        {
                            weaponSlot.transform.Find(otherGearItemFromDatabase.gamePref.name).gameObject.SetActive(true);
                        }
                        PlayChangeWeapSound();
                    }
                    Destroy(other.gameObject);
                }
            }
        }
    }

    public void NextWeapon()
    {
        availableWeapons[selectedWeapon].weaponGO.SetActive(false);
        availableWeapons[selectedWeapon].weaponUIGO.SetActive(false);
        selectedWeapon++;
        if (selectedWeapon >= (availableWeapons.Count))
        {
            selectedWeapon = 0;
        }
        availableWeapons[selectedWeapon].weaponGO.SetActive(true);
        availableWeapons[selectedWeapon].weaponUIGO.SetActive(true);
        PlayChangeWeapSound();
    }
    public void PrevWeapon()
    {
        availableWeapons[selectedWeapon].weaponGO.SetActive(false);
        availableWeapons[selectedWeapon].weaponUIGO.SetActive(false);
        selectedWeapon--;
        if (selectedWeapon < 0)
        {
            selectedWeapon = availableWeapons.Count - 1;
        }
        availableWeapons[selectedWeapon].weaponGO.SetActive(true);
        availableWeapons[selectedWeapon].weaponUIGO.SetActive(true);
        PlayChangeWeapSound();
    }
    public void SetSelectedWeapon(int selected)
    {
        if ((selected >= 0) && (selected < availableWeapons.Count))
        {
            availableWeapons[selectedWeapon].weaponGO.SetActive(false);
            availableWeapons[selectedWeapon].weaponUIGO.SetActive(false);
            selectedWeapon = selected;
            availableWeapons[selectedWeapon].weaponGO.SetActive(true);
            availableWeapons[selectedWeapon].weaponUIGO.SetActive(true);
            PlayChangeWeapSound();
        }
    }

    void PlayChangeWeapSound()
    {
        audioSource.clip = changeWeapSounds[Random.Range(0, changeWeapSounds.Count)];
        audioSource.Play();
    }

    void PlayAmmoSound()
    {
        audioSource.clip = takeAmmoSounds[Random.Range(0, takeAmmoSounds.Count)];
        audioSource.Play();
    }

    public GameObject GetSelectedWeapon()
    {
        return availableWeapons[selectedWeapon].weaponGO;
    }


}
