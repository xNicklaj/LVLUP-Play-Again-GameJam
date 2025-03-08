using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.Events;

[RequireComponent(typeof(PlaySoundOnShoot))]
[RequireComponent(typeof(ChanceSystem))]
public class ShootingSystem : NetworkBehaviour
{

    #region CustomTypes
    [System.Serializable]
    public struct ShootingModifiers : INetworkSerializable
    {
        public bool homing;
        public int axis;
        public float timeslotDurationMult; // minimum should be 1/10 of a second
        public float rangeMult;
        public float damageMult;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue<bool>(ref homing);
            serializer.SerializeValue<int>(ref axis);
            serializer.SerializeValue<float>(ref timeslotDurationMult);
            serializer.SerializeValue<float>(ref rangeMult);
            serializer.SerializeValue<float>(ref damageMult);
        }
    }

    [System.Serializable]
    public struct SlotsConfig
    {
        public float slotDuration;
        public int shotsPerSlot;
        public int pauseSlotsNumber;
    }
    #endregion

    #region References
    public ChanceSystem random;
    public ShootingSystemDefaults defaults;
    PlayerInput input;
    public GameObject bulletPrefab;
    [HideInInspector] public VisionSystem vision;
    private EnemyNavigation nav;
    private Transform shooter;
    private PlaySoundOnShoot _playSoundOnShoot;
    private PlayerControllerMP pContr;
    #endregion

    #region ShootingVariables
    private bool _isShooting = true;

    // ---------------
    // TODO: just as in the spawner system, the default values for these structs are never used, 
    // the inspector starts with everything set to 0. How do I know if those zeros were put by the user
    // or by Unity?
    public ShootingModifiers modifiers; // to be set in the inspector
    public SlotsConfig timeslotsConfig; // to be set in the inspector
    // ---------------

    public Color bulletColor = Color.white;
    public float speedWhileShootingMult = 0.20f;
    private float nextSlot;
    private float nextSubslot;
    private int slotCounter;
    private int subslotCounter;
    private bool isThisPause = false;
    public bool amIenemy;

    public bool IsShooting { get => _isShooting; private set => _isShooting = value; }

    #endregion

    public void Awake()
    {
        _playSoundOnShoot = GetComponent<PlaySoundOnShoot>();
        random = GetComponent<ChanceSystem>();
        if (amIenemy)
            nav = GetComponent<EnemyNavigation>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        nextSlot = Time.time;
        shooter = gameObject.transform;
        if (amIenemy)
        {
            vision = gameObject.GetComponent<VisionSystem>();
            Debug.Assert(vision != null, "vision component is null");
        }
        input = gameObject.GetComponent<PlayerInput>();
        pContr = gameObject.GetComponent<PlayerControllerMP>();

        Debug.Assert(bulletPrefab != null, "Bullet prefab not assigned");
    }

    private void Update()
    {
        if (!IsOwner) return;
        int subslotsInSlot = (int)(timeslotsConfig.slotDuration / (timeslotsConfig.slotDuration / timeslotsConfig.shotsPerSlot));
        int totalSlotsInCycle = 1 + timeslotsConfig.pauseSlotsNumber;

        // update IsShooting and get shooting direction 
        Vector2 targetDirection = new Vector2(0, 0);
        if (amIenemy)
        {
            if (nav.isMoving)
                IsShooting = false; // never shoots while moving
            else
            {
                GameObject target = vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer }); // should flash a raycast if debug on
                IsShooting = target != null; // if sees target, shoots target.
                if (target == null) return;
                targetDirection = (target.transform.position - transform.position).normalized;
            }
        }
        else
        {
            targetDirection = input.actions.FindAction("Direction").ReadValue<Vector2>().normalized;
            IsShooting = targetDirection.magnitude > 0;
        }

        /* TODO: this was just to see the raycasts. The movement script will be the one calling repeatedly the VisionComponent. */
        // if (amIenemy)
        //     vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer });

        if (Time.time >= nextSubslot)
        {
            // Debug.Log(amIenemy);
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
                        // yes, cycle is finished. Next subslot will be fire
                        isThisPause = false;
                    }
                }

                // anyway, update nextSubslot
                nextSubslot = Time.time + timeslotsConfig.slotDuration * modifiers.timeslotDurationMult / timeslotsConfig.shotsPerSlot;
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
                    // okay, but nothing happens when the cycle finishes, ahah
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

            //Debug.Log("I am here " + IsShooting);

            // anyway, update nextSubslot
            nextSubslot = Time.time + timeslotsConfig.slotDuration * modifiers.timeslotDurationMult / timeslotsConfig.shotsPerSlot;

            if (!IsShooting) {
                pContr.MoveSpeedMult = 1.0f;
                return;
            };

            if (amIenemy)
            {
                ExecuteSubslot(targetDirection, modifiers);
            }
            else
            {
                if (targetDirection.magnitude > 0)
                {
                    pContr.MoveSpeedMult = speedWhileShootingMult;
                    ExecuteSubslot(targetDirection, modifiers);
                }
            }
        }
    }

    public void ExecuteSubslot(Vector2 targetDirection, ShootingModifiers modifiers)
    {
        // we changed subslot and this is not pause, we should fire.  ---------------
        // Debug.Log("I SHOULD FIRE NOW!");

        for (int i = 0; i < modifiers.axis; i++)
        {
            // Calculate evenly distributed angle
            float angle = 360.0f / modifiers.axis * i;
            Vector2 direction;
            direction = Quaternion.Euler(0, 0, angle) * targetDirection;
            if (random.Happens("Shooting"))
                FireBulletServerRpc(direction, shooter.position, shooter.rotation, modifiers);
        }

    }

    [Rpc(SendTo.Server)]
    private void FireBulletServerRpc(Vector2 targetDirection, Vector2 originPosition, Quaternion originRotation, ShootingModifiers modifiers)
    {
        FireBullet(targetDirection, originPosition, originRotation, modifiers);
    }

    private void FireBullet(Vector2 targetDirection, Vector2 originPosition, Quaternion originRotation, ShootingModifiers modifiers)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, originPosition, originRotation);
        bulletInstance.GetComponent<NetworkObject>().Spawn();
        Bullet bullet = bulletInstance.GetComponent<Bullet>();
        _playSoundOnShoot.PlayShootingClipClientRpc();

        if (bullet != null)
        {
            bullet.direction = targetDirection;
            bullet.color = bulletColor;
            bullet.isHoming = modifiers.homing;
            bullet.maxFlyTimeMult = modifiers.rangeMult;
            bullet.damageMult = modifiers.damageMult;
            bullet.isFromEnemy = amIenemy;
        }
    }
}