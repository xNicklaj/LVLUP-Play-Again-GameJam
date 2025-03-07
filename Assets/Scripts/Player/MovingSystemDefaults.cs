using UnityEngine;

[CreateAssetMenu(fileName = "MovingSystemDefaults", menuName = "Scriptable Objects/MovingSystemDefaults")]
public class MovingSystemDefaults : ScriptableObject
{
    public float speed = 10f; 
    public float acceleration = 200f; 
    public float deceleration = 10f;
}
