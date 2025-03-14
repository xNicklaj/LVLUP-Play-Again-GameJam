using UnityEngine;

public class TerminatorModifier : ModifierBase
{
    private float OGDuration;
    private float OGSpeed;

    protected override void ApplyModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.axis = 10;
        OGDuration = ModifierTarget.GetComponent<ShootingSystem>().modifiers.timeslotDurationMult;
        OGSpeed = ModifierTarget.GetComponent<ShootingSystem>().modifiers.bulletSpeedMult;
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.timeslotDurationMult = 0.6f;
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.bulletSpeedMult = 1.3f;
    }

    protected override void DisposeModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.axis = 1;
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.timeslotDurationMult = OGDuration;
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.bulletSpeedMult = OGSpeed;

    }
}
