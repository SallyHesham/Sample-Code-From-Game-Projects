using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Entities/Enemy")]
public class BasicEnemy : ScriptableObject
{
    public Sprite image;
    public int maxHealth = 10;
    public int dps = 5;

    public float fireRes = 1f;
    public float waterRes = 1f;
    public float natARes = 1f;
    public float natBRes = 1f;
    public float natCRes = 1f;
    public float lightRes = 1f;
    public float darkRes = 1f;
    public float glassRes = 1f;
    public float bloodRes = 1f;
    public float dustRes = 1f;
}
