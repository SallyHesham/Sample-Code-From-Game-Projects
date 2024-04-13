using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SigilDataClass
{
    public List<Partition> partitions;

    public static SigilDataClass hash(List<Sigil> list)
    {
        SigilDataClass data = new SigilDataClass();
        data.partitions = new List<Partition>();

        for (int i = 0; i < 10; i++){
            data.partitions.Add(new Partition());
            data.partitions[i].num = i;
            data.partitions[i].sigils = new List<Sigil>();
        }

        for (int y = 0; y < list.Count; y++)
        {
            int index = int.Parse(list[y].id.Split(" ")[0]);
            index = index % 10;
            data.partitions[index].sigils.Add(list[y]);
        }

        return data;
    }
}

public class Partition
{
    public int num;
    public List<Sigil> sigils;
}

[CreateAssetMenu(fileName = "New Sigil", menuName = "Entities/Sigils")]
public class Sigil : ScriptableObject
{
    public string id;
    public string type;
    public int power;
}