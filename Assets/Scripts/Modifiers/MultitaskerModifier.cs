using UnityEngine;

public class MultitaskerModifier : ModifierBase
{
    protected override void ApplyModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.axis = 4;
    }

    protected override void DisposeModifier()
    {
        ModifierTarget.GetComponent<ShootingSystem>().modifiers.axis = 1;
    }
}
