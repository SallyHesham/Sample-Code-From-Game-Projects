using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Data
{
    public string lastPlayed;
    public string deltaID;
    public string[] enemyIDs;
    public int[] enemyHealth;
    public int mana;
    public int towerIntegrity;
    public int enemyLineCount;
    public string state;

    public float tillReturnTime;
    public float returnTimer;
    public float tillBattleTime;
    public float battleTimer;
}

public class DataMaster : MonoBehaviour
{
    public Data data;
    public static DataMaster inst;
    private string savePath;
    private string saveFile = "Fehu";
    int ti = 1000;

    void Awake()
    {
        inst = this;
        DontDestroyOnLoad(gameObject);
        // these are important // uncomment them when developement is over please !!!!!!!!!!!!!!!!!!
        //saveFile = SaveFileName.inst.fileName;
        //SaveFileName.inst.destroyThis();
        savePath = Path.Combine(Application.persistentDataPath, "SaveData", saveFile + ".json");
        data = new Data();
        loadData();
    }

    private void Start()
    {
        StateMech.inst.OnWardenReturn += clearBattleTimer;
        StateMech.inst.OnWardenReturn += clearReturnTimer;
    }

    public void saveData()
    {
        data.lastPlayed = DateTime.Now.ToString();
        string saveData = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, saveData);
    }

    private void loadData()
    {
        if (File.Exists(savePath))
        {
            string loadData = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<Data>(loadData);
        }
        else
        {
            data.enemyLineCount = 5;

            string[] newIDs = new string[data.enemyLineCount];
            Array.Fill<string>(newIDs, "");
            int[] newHPs = new int[data.enemyLineCount];
            Array.Fill<int>(newHPs, 0);
            data.enemyIDs = newIDs;
            data.enemyHealth = newHPs;

            data.lastPlayed = DateTime.Now.ToString();
            data.deltaID = "DustDelta";
            data.towerIntegrity = ti;
            data.mana = 10;
            data.state = "Occupied"; //temp
        }
    }


    public void removeDeadEnemy()
    {
        string[] newIDs = new string[data.enemyLineCount];
        Array.Fill<string>(newIDs, "");
        int[] newHPs = new int[data.enemyLineCount];
        Array.Fill<int>(newHPs, 0);
        for (int i = 0; i < data.enemyIDs.Length - 1; i++)
        {
            newIDs[i] = data.enemyIDs[i + 1];
            newHPs[i] = data.enemyHealth[i + 1];
        }
        data.enemyIDs = newIDs;
        data.enemyHealth = newHPs;
    }

    public void enemyDmg(int h)
    {
        data.enemyHealth[0] = h;
    }

    public void changeEnemyArrays(string[] ids, int[] hps)
    {
        data.enemyIDs = ids;
        data.enemyHealth = hps;
    }

    public void replenishManaTick()
    {
        if (data.mana != 10)
        {
            data.mana += 1;
        }
    }

    public bool useMana(int cost)
    {
        if (cost > data.mana)
        {
            return false;
        }
        else
        {
            data.mana -= cost;

            // for debugging
            Debug.Log("Current Mana: " + data.mana);

            return true;
        }
    }

    public void setReturnTimer(float time)
    {
        data.tillReturnTime = time;
        data.returnTimer = 0f;
    }

    public void setBattleTimer(float time)
    {
        data.tillBattleTime = time;
        data.battleTimer = 0f;
    }

    public void returnTick()
    {
        data.returnTimer += Time.deltaTime;

        //for debugging
        //Debug.Log("Return Timer: " + data.returnTimer);

        if (data.returnTimer >= data.tillReturnTime)
        {
            StateMech.inst.WardenReturn();
        }
    }

    public void battleTick()
    {
        data.battleTimer += Time.deltaTime;

        //for debugging
        //Debug.Log("Battle Timer: " + data.battleTimer);

        if (data.battleTimer >= data.tillBattleTime)
        {
            clearBattleTimer();
            StateMech.inst.BattleStart();
        }
    }

    public void clearReturnTimer()
    {
        data.returnTimer = 0f;
        data.tillReturnTime = 0f;
    }

    public void clearBattleTimer()
    {
        data.battleTimer = 0f;
        data.tillBattleTime = 0f;
    }

    public void setState(string state)
    {
        data.state = state;
    }

    // in case i need it
    public void changeDelta(string id)
    {
        data.deltaID = id;
    }

    public void dmgTower(int dmg)
    {
        data.towerIntegrity -= dmg;

        //for debugging
        //Debug.Log("Tower Integrity: " + data.towerIntegrity);


        if (data.towerIntegrity <= 0)
        {
            data.towerIntegrity = 0;
            StateMech.inst.GameNotOver();
        }
    }

    public void repairTower()
    {
        if (data.towerIntegrity < ti)
        {
            if (useMana(1))
            {
                data.towerIntegrity += 10;
            }
            
        }
    }
}
