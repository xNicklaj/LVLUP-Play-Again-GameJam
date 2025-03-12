using UnityEngine;

public class MinigunModifier : ModifierBase
{
    public float fireRateMultiplier = 2.2f;
    private float _originalFireRate;

    private ShootingSystem _shootingSystem;


    protected override void ApplyModifier()
    {
        _shootingSystem = ModifierTarget.GetComponent<ShootingSystem>();
        _originalFireRate = _shootingSystem.modifiers.timeslotDurationMult;
        _shootingSystem.modifiers.timeslotDurationMult = 1 / fireRateMultiplier;
    }

    protected override void DisposeModifier()
    {
        _shootingSystem.modifiers.timeslotDurationMult = _originalFireRate;
    }
}
