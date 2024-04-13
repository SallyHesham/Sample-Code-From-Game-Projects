using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Part", menuName = "Scriptable/Part")]
public class PartData : ScriptableObject
{
    public enum Type
    {
        Head, Body, RightArm, LeftArm, RightLeg, LeftLeg, Tail
    }

    public enum Ability
    {
        Attack, AreaAttack, Defend, Swap, Bite, See, Nothing
    }

    public new string name;
    public Type type;
    public Sprite sprite;
    public Sprite frontSprite;
    public int hp;
    public int def;
    public int atk;
    public List<Ability> abilities;
    [TextArea]
    public List<string> descriptions;
}
