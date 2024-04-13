using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartScript : MonoBehaviour
{
    public PartData.Type type;
    public string defaultPart;
    public bool isPlayer;

    public int currentHP;
    public bool destroyed;
    public PartData currentPart;

    public void Save()
    {
        if (currentPart == null)
        {
            PlayerPrefs.SetString((type.ToString()) + "Name", "null");
            PlayerPrefs.SetInt((type.ToString()) + "HP", 0);
            PlayerPrefs.SetInt((type.ToString()) + "state", 1);
        }
        else
        {
            PlayerPrefs.SetString((type.ToString()) + "Name", currentPart.name);
            PlayerPrefs.SetInt((type.ToString()) + "HP", currentHP);
            int dest;
            if (!destroyed)
            {
                dest = 0;
            }
            else
            {
                dest = 1;
            }
            PlayerPrefs.SetInt((type.ToString()) + "state", dest);
        }
        if (currentPart != null && currentPart.name == "Sowilo's Head")
        {
            GetComponent<ManaScript>().Save();
        }
    }

    public void Load()
    {
        currentPart = OverlordScript.inst.FindPart(PlayerPrefs.GetString((type.ToString()) + "Name", defaultPart));
        if (currentPart != null)
        {
            currentHP = PlayerPrefs.GetInt((type.ToString()) + "HP", currentPart.hp);
            int dest = PlayerPrefs.GetInt((type.ToString()) + "state", 0);
            if (dest == 0)
            {
                destroyed = false;
            }
            else
            {
                destroyed = true;
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = currentPart.sprite;
        }
        else
        {
            currentHP = 0;
            destroyed = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void LoadHP()
    {
        if (currentPart != null)
        {
            currentHP = currentPart.hp;
        }
        else
        {
            currentHP = 0;
        }
    }

    public void Swap(PartData part, int hp, bool state)
    {
        if (part == null)
        {
            currentPart = null;
            currentHP = 0;
            destroyed = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            currentPart = part;
            currentHP = hp;
            destroyed = state;
            if (!isPlayer && (currentPart.type == PartData.Type.Head || currentPart.type == PartData.Type.Body ||
                currentPart.type == PartData.Type.Tail) && currentPart.frontSprite != null)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = currentPart.frontSprite;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = currentPart.sprite;
            }
            // mana isOn
            if (currentPart.name == "Sowilo's Head")
            {
                ManaScript mana = gameObject.GetComponent<ManaScript>();
                mana.isOn = true;
                mana.SwapMana();
                mana.LoadCorrectHair();
            }
            else if (currentPart.type == PartData.Type.Head)
            {
                gameObject.GetComponent<ManaScript>().isOn = false;
            }

            if (!isPlayer && currentPart.name == "Sowilo's Head")
            {
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = -9;
            }
            else if (!isPlayer && currentPart.type == PartData.Type.Head)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = -4;
            }
        }
    }

    public void Damage(int atk)
    {
        int dmg = atk - currentPart.def;
        if (dmg > 0)
        {
            int newHP = currentHP - dmg;
            if (newHP <= 0)
            {
                currentHP = 0;
                PartDestroyed();
            }
            else
            {
                currentHP = newHP;
            }
        }
        else
        {
            return;
        }
    }

    public void Drain(int dmg)
    {
        if (dmg > 0)
        {
            int newHP = currentHP - dmg;
            if (newHP <= 0)
            {
                currentHP = 0;
                PartDestroyed();
            }
            else
            {
                currentHP = newHP;
            }
        }
        else
        {
            return;
        }
    }

    public void Heal(int hp)
    {
        currentHP += hp;
        if (currentHP > currentPart.hp)
        {
            currentHP = currentPart.hp;
        }
        if (destroyed)
        {
            destroyed = false;
        }
    }

    public void PartDestroyed()
    {
        destroyed = true;
        // add battle log text
        string text = "";
        if (isPlayer)
        {
            text += "Your ";
        }
        else
        {
            text += "The Boss's ";
        }
        text += type.ToString();
        text += " has been destroyed.";
        transform.GetComponentInParent<CharacterScript>().AddLogText(text);

        if (type == PartData.Type.Body)
        {
            // call death func
            if (isPlayer)
            {
                OverlordScript.inst.PlayerDeath();
            }
            else
            {
                OverlordScript.inst.EnemyDeath();
            }
        }
    }
}
