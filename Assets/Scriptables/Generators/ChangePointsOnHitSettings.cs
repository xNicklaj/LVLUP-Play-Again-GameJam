using UnityEngine;

[CreateAssetMenu(fileName = "ChangePointsOnHitSettings", menuName = "Scriptable Objects/ChangePointsOnHitSettings")]
public class ChangePointsOnHitSettings : ScriptableObject
{
    public int Amount = 100;
    public bool DestroyOnHit = true;
    public bool AmIPlayer = false;
}
