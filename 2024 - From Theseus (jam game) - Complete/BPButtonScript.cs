using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BPButtonScript : MonoBehaviour
{
    public PartScript part;
    public GameObject infoObject;
    public GameObject buttonsObject;
    public TMP_Text desc;
    public BattleScript battleScript;

    private List<TMP_Text> info;

    private void Start()
    {
        // get info text pieces
        info = new List<TMP_Text>();
        foreach (Transform child in infoObject.transform)
        {
            info.Add(child.gameObject.GetComponent<TMP_Text>());
        }
    }

    public void OnSelect()
    {
        if (part.currentPart != null)
        {
            // adjust info in ui on click

            info[0].text = part.currentPart.name;
            if (part.destroyed)
            {
                info[1].text = info[1].gameObject.name + ": " + "0";
                info[2].text = info[2].gameObject.name + ": " + "0";
                info[3].text = info[3].gameObject.name + ": " + "0";
                info[5].gameObject.SetActive(true);
            }
            else
            {
                info[1].text = info[1].gameObject.name + ": " + part.currentHP.ToString() + "/" + part.currentPart.hp;
                info[2].text = info[2].gameObject.name + ": " + part.currentPart.atk.ToString();
                info[3].text = info[3].gameObject.name + ": " + part.currentPart.def.ToString();
                info[5].gameObject.SetActive(false);
            }

            if (part.currentPart.name == "Sowilo's Head")
            {
                info[4].gameObject.SetActive(true);
                info[4].text = info[4].gameObject.name + ": " +
                    part.gameObject.GetComponent<ManaScript>().currentMana.ToString() + "/" +
                    part.gameObject.GetComponent<ManaScript>().maxMana;
            }
            else
            {
                info[4].gameObject.SetActive(false);
            }
        }
        else
        {
            // adjust info in ui if part is null
            for (int i = 0; i < info.Count-1; i++)
            {
                info[i].text = string.Empty;
            }
            info[0].text = "Nothing";
            info[5].gameObject.SetActive(false);
        }

        if (part.isPlayer)
        {
            // setup action buttons and clear desc and ability
            battleScript.ClearPlayerAction();
            desc.text = string.Empty;
            SetupButtons();
        }
    }

    private void SetupButtons()
    {
        // compile abs list
        List<ActionButtonScript> buttons = new List<ActionButtonScript>();
        foreach (Transform child in buttonsObject.transform)
        {
            buttons.Add(child.gameObject.GetComponent<ActionButtonScript>());
        }
        // disable all buttons
        foreach (ActionButtonScript button in buttons)
        {
            button.Disable();
        }
        // check if part is destroyed
        if (part.destroyed)
        {
            // sowilo's exception
            if (part.currentPart.name == "Sowilo's Head")
            {
                for (int i = 0; i < part.currentPart.abilities.Count; i++)
                {
                    if (part.currentPart.abilities[i] == PartData.Ability.Bite)
                    {
                        buttons[0].Setup(part.currentPart.abilities[i], part.currentPart.descriptions[i]);
                    }
                }
            }
        }
        // else setup buttons normally
        else
        {
            for (int i = 0; i < part.currentPart.abilities.Count; i++)
            {
                buttons[i].Setup(part.currentPart.abilities[i], part.currentPart.descriptions[i]);
            }
        }
    }
}
