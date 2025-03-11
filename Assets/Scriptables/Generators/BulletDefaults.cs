using UnityEngine;

[CreateAssetMenu(fileName = "BulletDefaults", menuName = "Scriptable Objects/BulletDefaults")]
public class BulletDefaults : ScriptableObject
{
    public float maxTraveledDistance = 20f;
    public float timeBeforeDestroy = 0.0f;
    public float initialVelocity = 20f;
    public float damage = 1.0f;
    public AnimationCurve velocityOverTime; // Assume normalized (0 to 1)
    public AnimationCurve scaleOverTime; // Assume normalized (0 to 1)
    public float homingRadiusMult = 0.25f;
    public float homingTurnSpeed = 45f;
}
