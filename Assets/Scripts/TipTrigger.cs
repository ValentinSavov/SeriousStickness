using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipTrigger : MonoBehaviour
{
    [TextArea]
    public string tipText;
    GameObject ui;

    void Start()
    {
        ui = GameObject.Find("UI");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponentInParent<PlayerTag>())
        {
            if(ui.transform.Find("Tip"))
            {
                GameObject tip = ui.transform.Find("Tip").gameObject;
                tip.SetActive(true);
                tip.transform.Find("Text").GetComponent<Text>().text = tipText;
            }
            this.gameObject.SetActive(false);
        }
    }
}
