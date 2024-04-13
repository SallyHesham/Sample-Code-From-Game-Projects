using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineDataClass
{
    public List<FirstPointData> firstPoints;
    public static LineDataClass ParseDataFromJSON(string json)
    {
        return JsonUtility.FromJson<LineDataClass>(json);
    }
}

[Serializable]
public class FirstPointData
{
    public int firstPoint;
    public List<SecondPointData> secondPoints;
}

[Serializable]
public class SecondPointData
{
    public int secondPoint;
    public int lineNumber;
}