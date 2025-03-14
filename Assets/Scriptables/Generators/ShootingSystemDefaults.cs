using UnityEngine;

[CreateAssetMenu(fileName = "ShootingSystemDefaults", menuName = "Scriptable Objects/ShootingSystemDefaults")]

public class ShootingSystemDefaults : ScriptableObject
{
    public ShootingSystem.ShootingModifiers modifiers = new ShootingSystem.ShootingModifiers
    {
        axis = 1,
        homing = false,
        timeslotDurationMult = 1.0f,
        rangeMult = 1.0f,
        damageMult = 1.0f,
        bulletSpeedMult = 1.0f,
    };

    public ShootingSystem.SlotsConfig timeslotsConfig = new ShootingSystem.SlotsConfig
    {
        slotDuration = 1.0f,
        pauseSlotsNumber = 0,
        shotsPerSlot = 1
    };
}
