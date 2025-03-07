using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerSystemDefaults", menuName = "Scriptable Objects/SpawnerSystemDefaults")]
public class SpawnerSystemDefaults : ScriptableObject
{
    public int maxEntitiesInArea = 10;
    public float range = 15f;
    public SpawnerSystem.SpawnerActivateMechanism activateIf = SpawnerSystem.SpawnerActivateMechanism.PLAYER_PROXIMITY;
    public SpawnerSystem.SpawnerDeactivateMechanism deactivateIf = SpawnerSystem.SpawnerDeactivateMechanism.AFTER_N_SPAWNS;
    public int spawnsBeforeDeactivation = 10;
    public int cyclesBeforeDeactivation = 10;
    public LayerMask mustTouchLayers = LayerMask.GetMask("Ground");
    public LayerMask mustNotTouchLayers = LayerMask.GetMask("Obstacles");

    public SpawnerSystem.SpawnerModifiers modifiers = new SpawnerSystem.SpawnerModifiers
    {
        timeslotDurationMult = 1.0f, // minimum should be 1/10 of a second
        rangeMult = 1.0f,
        maxEntitiesInAreaOverride = 10
    };

    public SpawnerSystem.SlotsConfig timeslotsConfig = new SpawnerSystem.SlotsConfig
    {
        slotDuration = 1.0f,
        pauseSlotsNumber = 0,
        spawnsPerSlot = 1,
    };
}
