using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Scenario
{
    public string Name;
    public List<DialogueSet> DialogueSets;
    public static Scenario ParseJSON(string jsonString)
    {
        return JsonUtility.FromJson<Scenario>(jsonString);
    }
}

[System.Serializable]
public class DialogueSet
{
    public int SetNumber;
    public List<DialoguePiece> DialoguePieces;
}

[System.Serializable]
public class DialoguePiece
{
    public int Type;
    public string Title;
    public string Text;
}