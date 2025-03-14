using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(VisionSystem))]
[RequireComponent(typeof(ChanceSystem))]
public class SpawnerSystem : NetworkBehaviour
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
    public ChanceSystem random;
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
    public int minimumRadialDistanceFromPlayers = 2; // to be set in inspector 

    public bool isBossRoom = false;
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

    private void Start()
    {
        isActive = false;
        GameManager_v2.Instance.OnGameStart.AddListener(() => isActive = true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (!IsHost) return;
        nextSlot = Time.time;
        random = GetComponent<ChanceSystem>();
        vision = gameObject.GetComponent<VisionSystem>();
        Debug.Assert(objectPrefab != null, "objectPrefab component is null");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(deactivateIf);
        if (!IsHost) return;
        vision.sightRadiusMult = modifiers.rangeMult;

        if (vision.CountNearMe(new LayerMask[] { layerToSpawnIn }) >= modifiers.maxEntitiesInAreaOverride)
        {
            // temporarly disabled until someone is killed or gets farther
            isActive = false;
        }

        int subslotsInSlot = (int)(timeslotsConfig.slotDuration / (timeslotsConfig.slotDuration / timeslotsConfig.spawnsPerSlot));
        int totalSlotsInCycle = 1 + timeslotsConfig.pauseSlotsNumber;

        isActive = true;

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
                        if (deactivateIf != 0 && (deactivateIf & SpawnerDeactivateMechanism.AFTER_N_CYCLES) == deactivateIf && cycleCounter >= cyclesBeforeDeactivation)
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
                    if (deactivateIf != 0 && (deactivateIf & SpawnerDeactivateMechanism.AFTER_N_CYCLES) == deactivateIf && cycleCounter >= cyclesBeforeDeactivation)
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


            if (activateIf != 0 && (activateIf & SpawnerActivateMechanism.PLAYER_PROXIMITY) == activateIf) // if proximity is the trigger
            {
                /* 
                 * TODO: check if player is nearby, also through walls though, should it be like this?
                 * Moreover, should the trigger be one-shot (once the player got in the area, we're active until deactivation) 
                 * or continously checking this?
                 */
                isActive = isActive && vision.GetClosestInCircle(new LayerMask[] { vision.defaults.playerLayer }) != null;
            }

            if (!isActive) return;

            if (deactivateIf != 0 && (deactivateIf & SpawnerDeactivateMechanism.AFTER_N_SPAWNS) == deactivateIf && spawnedCounter >= spawnsBeforeDeactivation)
            {
                // TODO: destroy or not?
                isActive = false;
                // Destroy(gameObject);
            }

            // get spawn point for this subslot
            // Debug.Log(prefabId);
            Vector2 spawnPosition = transform.position;

            if (prefabId == SpawnableIdentifier.POWERUP)
                spawnPosition = vision.GetValidPointAroundMe(mustTouchLayers, mustNotTouchLayers);
            else if (prefabId == SpawnableIdentifier.ENEMY)
                spawnPosition = TryGetGoodSpawnPoint();

            ExecuteSubslot(spawnPosition, objectPrefab, prefabId);
            spawnedCounter++;
        }
    }

    private void ExecuteSubslot(Vector3 spawnPosition, GameObject objectPrefab, SpawnableIdentifier prefabId)
    {
        if (!IsOwner) return;

        if (!random.Happens("Spawn"))
            return;

        if (!isActive)
            return;

        GameObject objInstance = Instantiate(objectPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));
        objInstance.GetComponent<NetworkObject>().Spawn();
        
        switch (prefabId)
        {
            case SpawnableIdentifier.ENEMY:
                // set stuff internal to the prefab
                objInstance.layer = layerToSpawnIn;
                if(isBossRoom){
                    objInstance.GetComponent<EnemyNavigation>().notifyOnDeath = true;
                    GameManager_v2.Instance.OnBossEnemySpawn.Invoke();
                }
                //NotifyBossBattle();
                break;

            case SpawnableIdentifier.POWERUP:
                // set stuff internal to the prefab
                // obj.layer = LayerMask.GetMask("Powerups");
                break;

            default:
                break;
        }
    }

    private Vector2 TryGetGoodSpawnPoint()
    {
        /* 
         * Valid spawn point := spawn point which is touching the right layers and not touching the wrong layers (eg. on ground, not on obstacles)
         * Good spawn point := spawn point which is valid and is far enough from any player in the area to not be a jumpscare
         *
         * If no good point is found, but a valid point is found, returns the valid point
         * If no valid point is found either, returns the origin (the spawner's transform.position itself)
         */

        Vector2 spawnPoint = transform.position; // initially set to origin

        bool isItGood = false;
        for (int i = 0; i < 3; i++) // 3 max attempts to find a good spawn point; else, still returns a valid point, but not ideal
        {
            spawnPoint = vision.GetValidPointAroundMe(mustTouchLayers, mustNotTouchLayers); // does N attempts in finding a valid random point for the spawn (default 3)
            if (spawnPoint.Equals(transform.position))
            {
                // no valid point was found. We don't care if origin is far enough from players,
                // it's already not a valid point, so we cannot hope for anything better. Let's break here and return the origin.
                break;
            }

            List<Collider2D> collidersOfNearbyPlayers = VisionSystem.FindNearPosition(spawnPoint, minimumRadialDistanceFromPlayers, new LayerMask[] { vision.defaults.playerLayer });


            // good spawn point is a valid spawn point which is distant enough from any player in the area (and is not origin)
            isItGood = true;
            foreach (var coll in collidersOfNearbyPlayers)
            {
                Vector2 vectorSpawnPointToPlayer = (Vector2)coll.gameObject.transform.position - spawnPoint;
                if (vectorSpawnPointToPlayer.magnitude >= minimumRadialDistanceFromPlayers)
                {
                    isItGood = false;
                    break;
                }
            }

            if (isItGood)
                break; // we found a Good Spawn Point, no need to continue in the loop
        }

        if (!isItGood)
            Debug.LogWarning("Good spawn points were not found for this subslot. Returning a not ideal position.");

        return spawnPoint;
    }

    public enum SpawnableIdentifier
    {
        ENEMY,
        POWERUP
    }
}
