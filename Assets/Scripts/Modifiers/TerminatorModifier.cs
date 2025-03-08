using UnityEngine;

public class TerminatorModifier : ModifierBase
{
    private float OGDuration;

    protected override void ApplyModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.axis = 10;
        OGDuration = ModifierTarget.GetComponent<ShootingSystem>().modifiers.timeslotDurationMult;
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.timeslotDurationMult = 0.6f;
    }

    protected override void DisposeModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.axis = 1;
        OGDuration = ModifierTarget.GetComponent<ShootingSystem>().modifiers.timeslotDurationMult = OGDuration;
    }
}
