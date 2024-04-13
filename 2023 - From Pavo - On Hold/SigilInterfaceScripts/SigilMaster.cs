using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SigilMaster : MonoBehaviour
{
    public SigilHandler phase1;
    public SigilHandler phase2;
    public EnemyLines enemy;
    public List<Sigil> sigilList;
    int phase = 0;
    bool active = false;
    bool finished = false;
    List<int>sigilLines1 = new List<int>();
    List<int>sigilLines2 = new List<int>();
    SigilDataClass sigilData;


    private void Start()
    {
        sigilData = SigilDataClass.hash(sigilList);
        deactivate();
    }
    private void Update()
    {
        if (finished)
        {
            if (Input.GetMouseButtonUp(1))
            {
                finished = false;
                changePhase();
            }
        }
    }

    public void activate()
    {
        active = true;
        finished = false;
        phase = 0;
        phase1.hide();
        phase2.hide();
        gameObject.SetActive(true);
        changePhase();
    }

    public void deactivate()
    {
        active = false;
        finished = false;
        phase = 0;
        phase1.hide();
        phase2.hide();
        gameObject.SetActive(false);
    }

    public void changePhase()
    {
        switch (phase)
        {
            case 0:
                phase = 1;
                phase1.clearLines();
                phase2.clearLines();
                phase1.activate();
                break;
            case 1:
                phase = 2;
                phase2.activate();
                break;
            case 2:
                phase = 0;
                Sigil s = recognizeSigil();
                if (s != null)
                {
                    enemy.castSigil(s.type, s.power);
                    //for debugging
                    Debug.Log("Name: " + s.name + " Type: " + s.type + " Power: " + s.power);
                }
                else
                {
                    //do nothing
                    //for debugging
                    Debug.Log("No Match");
                }
                finished = true;
                break;
            case 3:
                phase = 0;
                deactivate();
                break;
        }
    }

    public void sendLines(List<int>lines)
    {
        switch(phase)
        {
            case 1:
                sigilLines1 = lines; break;
            case 2:
                sigilLines2 = lines; break;
        }
    }

    private Sigil recognizeSigil()
    {
        string sigil1ID = string.Join(" ", sigilLines1.ToArray());
        string sigil2ID = string.Join(" ", sigilLines2.ToArray());
        string sigilID = sigil1ID + " / " + sigil2ID;
        int num = sigilLines1[0]%10;
        List<Sigil> sigils = sigilData.partitions[num].sigils;

        for(int i = 0; i < sigils.Count; i++)
        {
            if (sigils[i].id == sigilID)
            {
                return sigils[i];
            }
        }

        //temporary. for debugging.
        Debug.Log(sigilID);
        //real
        return null;
    }

    public bool useMana(int c)
    {
        return CombatVeteran.inst.useMana(c);
    }
}
