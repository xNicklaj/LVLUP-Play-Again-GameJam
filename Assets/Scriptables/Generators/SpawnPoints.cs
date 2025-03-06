using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPoints", menuName = "Scriptable Objects/SpawnPoints")]
public class SpawnPointsScriptable : ScriptableObject
{
    public List<Vector3> list;
    public float spawnRadius = 5f;
}
