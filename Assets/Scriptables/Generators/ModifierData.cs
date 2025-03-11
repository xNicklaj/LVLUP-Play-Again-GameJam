using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier", menuName = "Modifier/New Modifier Data")]
public class ModifierData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public GameObject TargetObject;
    public AnimationCurve DespawnFrequencyCurve;
    public float DespawnTimeoutDuration = 10f;
    public float TimeBeforeDespawn = 10f;
    public float Duration = 30;
    public AudioClip AppliedClip;
    public AudioClip DisposedClip;

    [HideInInspector] public string TargetName { get; private set; }

    private void OnEnable()
    {
        if(TargetObject != null) TargetName = TargetObject.name;
    }
}
