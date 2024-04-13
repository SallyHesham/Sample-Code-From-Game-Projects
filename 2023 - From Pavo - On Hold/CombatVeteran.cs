using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatVeteran : MonoBehaviour
{
    public static CombatVeteran inst;
    public List<DeltaSO> deltaCatalogue;
    private List<BasicEnemy> enemyLineUp;
    private float manaReplenishTime = 3f;
    private float manaTimer = 0f;
    private float dmgTimer = 0f;
    private float dmgTime = 2f;
    private float repairTimer = 0f;
    private float repairTime = 2f;

    void Start()
    {
        inst = this;
        DontDestroyOnLoad(gameObject);
        enemyLineUp = new List<BasicEnemy>();
        loadLineUpFromMaster();

        StateMech.inst.OnWardenLeft += startReturnCountdown;
        StateMech.inst.OnWardenKill += clearLineUp;
        StateMech.inst.OnBattleStart += randomizeLineUp;
        StateMech.inst.OnBattleEnd += startBattleCountdown;
    }

    void Update()
    {
        manaTick();

        switch (DataMaster.inst.data.state)
        {
            case ("Anticipation"):
                DataMaster.inst.returnTick();
                DataMaster.inst.battleTick();
                break;
            case ("Battle"):
                DataMaster.inst.returnTick();
                battleTick();
                break;
            case ("Idle"):
                repairTick();
                break;
            case ("The Between"):
                // ???
                break;
            default: break;
        }
    }

    private void loadLineUpFromMaster()
    {
        enemyLineUp = new List<BasicEnemy>();
        if (DataMaster.inst.data.enemyIDs[0] == "") return;

        int deltaIndex = 0;

        for (int i = 0; i < deltaCatalogue.Count; i++)
        {
            if (DataMaster.inst.data.deltaID == deltaCatalogue[i].name)
            {
                deltaIndex = i;
                break;
            }
        }

        for (int i = 0; i < DataMaster.inst.data.enemyIDs.Length; i++)
        {
            string eID = DataMaster.inst.data.enemyIDs[i];
            if (eID == "") break;
            else
            {
                for (int j = 0; j < deltaCatalogue[deltaIndex].enemyList.Count; j++)
                {
                    if (deltaCatalogue[deltaIndex].enemyList[j].name == eID)
                    {
                        enemyLineUp.Add(deltaCatalogue[deltaIndex].enemyList[j]);
                    }
                }
            }
        }
    }

    public List<BasicEnemy> getLineUp()
    {
        return enemyLineUp;
    }

    public void removeDeadEnemy()
    {
        enemyLineUp.RemoveAt(0);
        if (enemyLineUp.Count == 0 ) StateMech.inst.BattleEnd();
    }

    private void startReturnCountdown()
    {
        // time select code ???
        DataMaster.inst.setReturnTimer(15 * 60f);
        startBattleCountdown();
    }

    private void startBattleCountdown()
    {
        int deltaIndex = findDeltaIndex(DataMaster.inst.data.deltaID);
        int time = UnityEngine.Random.Range(deltaCatalogue[deltaIndex].enemyRangeStart,
            deltaCatalogue[deltaIndex].enemyRangeEnd + 1);
        DataMaster.inst.setBattleTimer(time * 60f);
    }

    private void randomizeLineUp()
    {
        string[] eIDs = new string[DataMaster.inst.data.enemyLineCount];
        Array.Fill<string>(eIDs, "");
        int[] hps = new int[DataMaster.inst.data.enemyLineCount];
        Array.Fill<int>(hps, 0);
        enemyLineUp = new List<BasicEnemy>();

        int deltaIndex = findDeltaIndex(DataMaster.inst.data.deltaID);
        
        int enemyNum = UnityEngine.Random.Range(deltaCatalogue[deltaIndex].enemyRangeStart,
            deltaCatalogue[deltaIndex].enemyRangeEnd + 1);
        for (int i = 0; i < enemyNum; i++)
        {
            int eIndex = UnityEngine.Random.Range(0, deltaCatalogue[deltaIndex].enemyList.Count);
            enemyLineUp.Add(deltaCatalogue[deltaIndex].enemyList[eIndex]);
            eIDs[i] = enemyLineUp[i].name;
            hps[i] = enemyLineUp[i].maxHealth;
        }

        DataMaster.inst.changeEnemyArrays(eIDs, hps);
    }

    public int findDeltaIndex(string name)
    {
        for (int i = 0; i < deltaCatalogue.Count; i++)
        {
            if (name == deltaCatalogue[i].name)
            {
                return i;
            }
        }
        // error state
        return -1;
    }

    private void manaTick()
    {
        manaTimer += Time.deltaTime;
        if (manaTimer >= manaReplenishTime)
        {
            DataMaster.inst.replenishManaTick();
            manaTimer -= manaReplenishTime;
        }
    }

    public bool useMana(int c)
    {
        return DataMaster.inst.useMana(c);
    }

    public void clearLineUp()
    {
        string[] eIDs = new string[DataMaster.inst.data.enemyLineCount];
        Array.Fill<string>(eIDs, "");
        int[] hps = new int[DataMaster.inst.data.enemyLineCount];
        Array.Fill<int>(hps, 0);
        enemyLineUp = new List<BasicEnemy>();
        DataMaster.inst.changeEnemyArrays(eIDs, hps);
    }

    private void battleTick()
    {
        dmgTimer += Time.deltaTime;

        if (dmgTimer < dmgTime) return;
        else dmgTimer -= dmgTime;

        battleDMG();
    }

    private void battleDMG()
    {
        int dmg = 0;
        for (int i = 0; i < enemyLineUp.Count; i++)
        {
            dmg += enemyLineUp[i].dps;
        }
        DataMaster.inst.dmgTower(dmg);
    }

    private void repairTick()
    {
        repairTimer += Time.deltaTime;

        if (repairTimer < repairTime) return;
        else repairTimer -= repairTime;

        DataMaster.inst.repairTower();
    }
}
