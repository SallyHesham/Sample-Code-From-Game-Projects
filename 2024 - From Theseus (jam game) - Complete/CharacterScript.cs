using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public List<PartScript> parts;
    public BattleScript battleScript;
    public List<PartData> serpentParts;

    public void Save()
    {
        foreach (PartScript part in parts)
        {
            part.Save();
        }
    }

    public void Load()
    {
        foreach (PartScript part in parts)
        {
            part.Load();
        }
    }

    public void LoadHP()
    {
        foreach (PartScript part in parts)
        {
            part.LoadHP();
        }
    }

    public void DamageAllParts(int atk)
    {
        foreach (PartScript part in parts)
        {
            if (!part.destroyed)
            {
                part.Damage(atk);
            }
        }
    }

    public void RegenerateMana()
    {
        foreach (PartScript part in parts)
        {
            if (part.type == PartData.Type.Head)
            {
                part.gameObject.GetComponent<ManaScript>().Regenerate();
            }
        }
    }

    public PartScript FindPart(PartData.Type type)
    {
        foreach (PartScript partScript in parts)
        {
            if (partScript.type == type)
            {
                return partScript;
            }
        }
        return null;
    }

    public void AddLogText(string text)
    {
        battleScript.AddLogText(text);
    }

    public void LoadSerpent()
    {
        foreach (PartData spart in serpentParts)
        {
            PartScript part = FindPart(spart.type);
            part.Swap(spart, 100, false);
        }
        PartScript rleg = FindPart(PartData.Type.RightLeg);
        PartScript lleg = FindPart(PartData.Type.LeftLeg);
        rleg.Swap(null, 0, true);
        lleg.Swap(null, 0, true);
    }

    public void LoadSerpentLoanedParts()
    {
        PartScript h = FindPart(PartData.Type.Head);
        PartScript t = FindPart(PartData.Type.Tail);
        PartScript rl = FindPart(PartData.Type.RightLeg);
        PartScript ll = FindPart(PartData.Type.LeftLeg);

        PartData fh = OverlordScript.inst.FindPart(PlayerPrefs.GetString("FinalHead", "The Serpent's Head"));
        PartData ft = OverlordScript.inst.FindPart(PlayerPrefs.GetString("FinalTail", "The Serpent's Tail"));
        PartData frl = OverlordScript.inst.FindPart(PlayerPrefs.GetString("FinalRLeg", "The Serpent's Right Leg"));
        PartData fll = OverlordScript.inst.FindPart(PlayerPrefs.GetString("FinalLLeg", "The Serpent's Left Leg"));

        h.Swap(fh, 1, false);
        t.Swap(ft, 1, false);
        rl.Swap(frl, 1, false);
        ll.Swap(fll, 1, false);

        if (h.currentPart.name == "Sowilo's Head")
        {
            ManaScript m = h.gameObject.GetComponent<ManaScript>();
            m.currentMana = m.maxMana;
            m.LoadCorrectHair();
        }
    }
}
