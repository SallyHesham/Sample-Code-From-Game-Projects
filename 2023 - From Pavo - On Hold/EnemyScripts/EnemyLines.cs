using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLines : MonoBehaviour
{
    public List<EnemyClass> enemyList;

    private void Start()
    {
        StateMech.inst.OnWardenKill += clearLineUp;
        StateMech.inst.OnBattleStart += battleStarted;

        if (DataMaster.inst.data.state == "Battle")
        {
            battleStarted();
        }
        else
        {
            clearLineUp();
        }
    }

    private void battleStarted()
    {
        clearLineUp();
        List<BasicEnemy> beList;
        beList = CombatVeteran.inst.getLineUp();
        setLineUp(beList, DataMaster.inst.data.enemyHealth);
    }

    public void setLineUp(List<BasicEnemy> enemyLineUp, int[] health)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (i < enemyLineUp.Count)
            {
                enemyList[i].changeEnemy(enemyLineUp[i], health[i]);
            }
            else
            {
                enemyList[i].disableEnemy();
            }
        }
    }

    public void clearLineUp()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].disableEnemy();
        }
    }

    public void enemyDeath()
    {
        for (int i = 0; i < enemyList.Count - 1; i++)
        {
            if (enemyList[i + 1].enemyData != null)
            {
                enemyList[i].changeEnemy(enemyList[i + 1].enemyData, enemyList[i + 1].health);
            }
            else
            {
                enemyList[i].disableEnemy();
            }
        }
        enemyList[enemyList.Count - 1].disableEnemy();
        DataMaster.inst.removeDeadEnemy();
        CombatVeteran.inst.removeDeadEnemy();
    }

    public void castSigil(string type, int power)
    {
        bool alive = enemyList[0].takeSigil(type, power);
        DataMaster.inst.enemyDmg(enemyList[0].health);
        if(!alive) enemyDeath();
    }
}
