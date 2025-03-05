using UnityEngine;

[CreateAssetMenu(fileName = "BulletDefaults", menuName = "Scriptable Objects/BulletDefaults")]
public class BulletDefaults : ScriptableObject
{
    public float maxFlyTime = 20f;
    public float timeBeforeDestroy = 2.0f;
    public float initialVelocity = 20f;
    public AnimationCurve velocityOverTime; // Assume normalized (0 to 1)
}
