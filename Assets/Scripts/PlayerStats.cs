using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Player/Stats")]
[System.Serializable]
public class PlayerStats : ScriptableObject
{
    public float MoveSpeed = 5f;
    public float RotationSpeed = 10f;
}