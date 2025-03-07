using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSystem : MonoBehaviour
{
    #region CustomTypes
    [Flags]
    public enum SpawnerActivateMechanism
    {
        PLAYER_PROXIMITY = 1 << 0,
        EVENT = 1 << 1,
    }

    [Flags]
    public enum SpawnerDeactivateMechanism
    {
        AFTER_N_SPAWNS = 1 << 1,
        AFTER_N_CYCLES = 1 << 2,
        EVENT = 1 << 3
    }

    [Serializable]
    public struct SpawnerModifiers
    {
        public float timeslotDurationMult;
        public float rangeMult;
        public int maxEntitiesInAreaOverride;
    }

    [Serializable]
    public struct SlotsConfig
    {
        public float slotDuration;
        public int spawnsPerSlot;
        public int pauseSlotsNumber;
    }
    #endregion

    #region References
    private VisionSystem vision;
    public GameObject objectPrefab; // to be set in inspector
    public SpawnableIdentifier prefabId; // to be set in inspector
    public LayerMask layerToSpawnIn; // to be set in inspector
    #endregion

    #region SpawnerVariables
    // ---------------
    // TODO: just as in the shooting system, the default values for these fields are never used, 
    // the inspector starts with everything set to 0. How do I know if those zeros were put by the user
    // or by Unity?
    public SpawnerModifiers modifiers; // to be set in inspector 
    public SlotsConfig timeslotsConfig; // to be set in inspector
    public SpawnerActivateMechanism activateIf; // to be set in inspector
    public SpawnerDeactivateMechanism deactivateIf; // to be set in inspector

    public int spawnsBeforeDeactivation = 10; // to be set in inspector
    public int cyclesBeforeDeactivation = 10; // to be set in inspector 
    // ---------------

    public LayerMask mustTouchLayers;
    public LayerMask mustNotTouchLayers;

    public bool isActive = true;
    private float nextSlot;
    private float nextSubslot;
    private int slotCounter;
    private int cycleCounter;
    private int subslotCounter;
    private int spawnedCounter = 0;
    private bool isThisPause = false;
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        nextSlot = Time.time;
        vision = gameObject.GetComponent<VisionSystem>();
        Debug.Assert(vision != null, "vision component is null");
        Debug.Assert(objectPrefab != null, "objectPrefab component is null");
    }

    // Update is called once per frame
    void Update()
    {
        vision.sightRadiusMult = modifiers.rangeMult;

        if (vision.CountNearMe(new LayerMask[] { layerToSpawnIn }) >= modifiers.maxEntitiesInAreaOverride)
        {
            // temporarly disabled until someone is killed or gets farther
            isActive = false;
        }

        int subslotsInSlot = (int)(timeslotsConfig.slotDuration / (timeslotsConfig.slotDuration / timeslotsConfig.spawnsPerSlot));
        int totalSlotsInCycle = 1 + timeslotsConfig.pauseSlotsNumber;

        // update isActive and get positions

        Vector3 spawnPosition = vision.GetValidPointAroundMe(
            mustTouchLayers,
            mustNotTouchLayers
        );

        if (Time.time >= nextSubslot)
        {
            // Debug.Log("Subslot started");

            if (isThisPause)
            {
                subslotCounter++;
                subslotCounter %= subslotsInSlot;

                // is slot finished?
                if (subslotCounter == 0)
                {
                    // yes, slot is finished. Update nextSlot
                    nextSlot = Time.time + (timeslotsConfig.slotDuration * modifiers.timeslotDurationMult);

                    // is cycle finished?
                    slotCounter++;
                    slotCounter %= totalSlotsInCycle;
                    if (slotCounter == 0)
                    {
                        // cycle is finished. 
                        cycleCounter++;
                        if ((deactivateIf & SpawnerDeactivateMechanism.AFTER_N_CYCLES) == deactivateIf && cycleCounter >= cyclesBeforeDeactivation)
                        {
                            // TODO: destroy or not?
                            isActive = false;
                            // Destroy(gameObject);
                        }

                        isThisPause = false;
                    }
                }

                // anyway, update nextSubslot
                nextSubslot = Time.time + timeslotsConfig.slotDuration * modifiers.timeslotDurationMult / timeslotsConfig.spawnsPerSlot;
                return;
            }

            // ------ non-pause slot -------

            if (Time.time >= nextSlot)
            {
                slotCounter++;
                slotCounter %= totalSlotsInCycle;
                if (slotCounter == 0)
                {
                    // cycle is finished. 
                    cycleCounter++;
                    if ((deactivateIf & SpawnerDeactivateMechanism.AFTER_N_CYCLES) == deactivateIf && cycleCounter >= cyclesBeforeDeactivation)
                    {
                        // TODO: destroy or not?
                        isActive = false;
                        // Destroy(gameObject);
                    }

                    isThisPause = false;
                }

                nextSlot = Time.time + (timeslotsConfig.slotDuration * modifiers.timeslotDurationMult);
            }

            // anyway, check if we finished the slot (if it was a slot of 1 subslot only it might be even right away)
            subslotCounter++;
            subslotCounter %= subslotsInSlot;
            if (subslotCounter == 0 && timeslotsConfig.pauseSlotsNumber > 0)
            {
                isThisPause = true; // the next subslot will be pause (if pause slots exist)
            }

            // anyway, update nextSubslot
            nextSubslot = Time.time + timeslotsConfig.slotDuration * modifiers.timeslotDurationMult / timeslotsConfig.spawnsPerSlot;


            if ((activateIf & SpawnerActivateMechanism.PLAYER_PROXIMITY) == activateIf) // if proximity is the trigger
            {
                /* 
                 * TODO: check if player is nearby, also through walls though, should it be like this?
                 * Moreover, should the trigger be one-shot (once the player got in the area, we're active until deactivation) 
                 * or continously checking this?
                 */
                isActive = isActive && vision.GetClosestInCircle(new LayerMask[] { vision.defaults.playerLayer }) != null;
            }

            if (!isActive) return;

            ExecuteSubslot(spawnPosition, objectPrefab, prefabId);
            spawnedCounter++;
            if ((deactivateIf & SpawnerDeactivateMechanism.AFTER_N_SPAWNS) == deactivateIf && spawnedCounter >= spawnsBeforeDeactivation)
            {
                // TODO: destroy or not?
                isActive = false;
                // Destroy(gameObject);
            }
        }
    }

    private void ExecuteSubslot(Vector3 spawnPosition, GameObject objectPrefab, SpawnableIdentifier prefabId)
    {
        GameObject objInstance = Instantiate(objectPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));

        if (objInstance.TryGetComponent<GameObject>(out var obj))
        {
            switch (prefabId)
            {
                case SpawnableIdentifier.ENEMY:
                    // set stuff internal to the prefab
                    obj.layer = LayerMask.GetMask("Enemies");
                    break;

                case SpawnableIdentifier.POWERUP:
                    // set stuff internal to the prefab
                    // obj.layer = LayerMask.GetMask("Powerups");
                    break;

                default:
                    break;
            }
        }

    }

    public enum SpawnableIdentifier
    {
        ENEMY,
        POWERUP
    }
}
