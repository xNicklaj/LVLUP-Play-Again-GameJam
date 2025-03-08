using UnityEngine;

public class SharpshooterModifier : ModifierBase
{
    protected override void ApplyModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.homing = true;
    }

    protected override void DisposeModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.homing = true;
    }
}
