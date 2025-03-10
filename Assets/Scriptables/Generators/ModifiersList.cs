using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using KaimiraGames;

[CreateAssetMenu(fileName = "ModifiersList", menuName = "Scriptable Objects/ModifiersList")]
public class ModifiersList : ScriptableObject
{
    public List<GameObject> list;
    public List<int> weight;
    public WeightedList<GameObject> weightedList;

    public void OnEnable()
    {
        while(weight.Count < list.Count)
        {
            weight.Add(1);
        }

        weightedList = new WeightedList<GameObject>();
        for (int i = 0; i < list.Count; i++)
        {
            weightedList.Add(list[i], weight[i]);
        }
    }
}
