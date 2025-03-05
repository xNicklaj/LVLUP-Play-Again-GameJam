using UnityEngine;

[CreateAssetMenu(fileName = "VisionSystemDefaults", menuName = "Scriptable Objects/VisionSystemDefaults")]
public class VisionSystemDefaults : ScriptableObject
{
    public float sightRadius = 7f;       // How far the enemy can see
    public float sightAngle = 360f;        // Enemy's vision cone in degrees
    public LayerMask playerLayer;         // Layer where player is
    public LayerMask obstacleLayer;       // Layers that block vision (walls, etc.)
    public bool debugEverything = false;  // Visualize rays
    public int numberOfRays = 3;
    public float angleBetweenRays = 15f;

}

