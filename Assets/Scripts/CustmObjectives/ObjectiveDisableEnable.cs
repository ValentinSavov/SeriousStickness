using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveDisableEnable : Objective
{
    public bool completed = false;
    public GameObject toDisable;
    public GameObject toEnable;
    public bool tipOnActivate = true;
    [TextArea]
    public string tipText;

    public override void ProcessCompletionAction()
    {
        completed = true;
        toDisable.SetActive(false);
        toEnable.SetActive(true);

        if(tipOnActivate)
        {
            GameObject ui = GameObject.Find("UI");
            if (ui.transform.Find("Tip"))
            {
                GameObject tip = ui.transform.Find("Tip").gameObject;
                tip.SetActive(true);
                tip.transform.Find("Text").GetComponent<Text>().text = tipText;
            }
        }
    }
}
