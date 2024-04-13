using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    public BasicEnemy enemyData;
    public int health;
    public int maxHealth;
    public int dps;
    public float fireRes;
    public float waterRes;
    public float natARes;
    public float natBRes;
    public float natCRes;
    public float lightRes;
    public float darkRes;
    public float glassRes;
    public float bloodRes;
    public float dustRes;

    private void Start()
    {
        loadEnemyData();
    }

    public void changeEnemy(BasicEnemy enemy, int h)
    {
        enemyData = enemy;
        loadEnemyData();
        loadEnemyHealth(h);
    }

    private void loadEnemyData()
    {
        health = enemyData.maxHealth;
        maxHealth = enemyData.maxHealth;
        dps = enemyData.dps;
        fireRes = enemyData.fireRes;
        waterRes = enemyData.waterRes;
        natARes = enemyData.natARes;
        natBRes = enemyData.natBRes;
        natCRes = enemyData.natCRes;
        lightRes = enemyData.lightRes;
        darkRes = enemyData.darkRes;
        glassRes = enemyData.glassRes;
        bloodRes = enemyData.bloodRes;
        dustRes = enemyData.dustRes;
    }

    public bool takeSigil(string sigilType, int sigilPower)
    {
        switch (sigilType)
        {
            case ("Fire"):
                takeDamage(sigilPower, fireRes);
                break;
            case ("Water"):
                takeDamage(sigilPower, waterRes);
                break;
            case ("NatA"):
                takeDamage(sigilPower, natARes);
                break;
            case ("NatB"):
                takeDamage(sigilPower, natBRes);
                break;
            case ("NatC"):
                takeDamage(sigilPower, natCRes);
                break;
            case ("Light"):
                takeDamage(sigilPower, lightRes);
                break;
            case ("Dark"):
                takeDamage(sigilPower, darkRes);
                break;
            case ("Glass"):
                takeDamage(sigilPower, glassRes);
                break;
            case ("Blood"):
                takeDamage(sigilPower, bloodRes);
                break;
            case ("Dust"):
                takeDamage(sigilPower, dustRes);
                break;
            default:
                Debug.Log("Not Supported");
                break;
        }
        
        Debug.Log(health);
        if (health > 0) return true; else return false;
    }

    private void takeDamage(int power, float res)
    {
        int dmg = Mathf.RoundToInt(power * (1 - res));
        health -= dmg;
        if (health < 0) health = 0;
        else if (health > maxHealth) health = maxHealth;
    }

    private void nullifyEnemy()
    {
        enemyData = null;
        health = 0;
        maxHealth = 0;
        dps = 0;
        fireRes = 0;
        waterRes = 0;
        natARes = 0;
        natBRes = 0;
        natCRes = 0;
        lightRes = 0;
        darkRes = 0;
        glassRes = 0;
        bloodRes = 0;
        dustRes = 0;
    }

    public void loadEnemyHealth(int h)
    {
        health = h;
    }

    public void disableEnemy()
    {
        nullifyEnemy();
        this.gameObject.SetActive(false);
    }
}
