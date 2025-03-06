using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ShootingSystem : NetworkBehaviour
{
    [System.Serializable]
    public struct ShootingModifiers
    {
        public bool homing;
        public int axis;
        public float timeslotDurationMult; // minimum should be 1/10 of a second
        public float rangeMult;
        public float damageMult;
    }

    [System.Serializable]
    public struct SlotsConfig
    {
        public float slotDuration;
        public int shotsPerSlot;
        public int pauseSlotsNumber;
    }

    #region References
    public ShootingSystemDefaults defaults;
    public GameObject bulletPrefab;
    [HideInInspector] public VisionSystem vision;
    private Transform shooter;
    #endregion

    #region ShootingVariables
    private bool _isShooting = true;
    public ShootingModifiers modifiers;
    public SlotsConfig timeslotsConfig;
    public Color bulletColor = Color.white;
    private float nextSlot;
    private float nextSubslot;
    private int pauseCounter = 0;
    private bool amIenemy;

    public bool IsShooting { get => _isShooting; private set => _isShooting = value; }
    
    #endregion

    private void Start()
    {
        if (!IsOwner) return;

        amIenemy = !gameObject.CompareTag("Player");
        nextSlot = Time.time;
        shooter = gameObject.transform;
        if (amIenemy)
        {
            vision = gameObject.GetComponent<VisionSystem>();
            vision.debug = true; // TODO: remove this i guess
            Debug.Assert(vision != null, "vision component is null");
        }

        Debug.Assert(bulletPrefab != null, "Bullet prefab not assigned");
    }

    private void Update()
    {   
        if(!IsOwner) return;

        /* TODO: this was just to see the raycasts. The movement script will be the one calling repeatedly the VisionComponent. */
        if (amIenemy)
            vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer });



        if (Time.time >= nextSubslot)
        {
            //Debug.Log(amIenemy);
            // Debug.Log("Subslot started");

            if (pauseCounter >= timeslotsConfig.shotsPerSlot)
            {
                // it's a pause slot
                // Debug.Log("Pause subslot..." + pauseCounter.ToString());
                pauseCounter++;
                pauseCounter %= timeslotsConfig.shotsPerSlot + timeslotsConfig.pauseSlotsNumber * timeslotsConfig.shotsPerSlot + 1;

                nextSubslot = Time.time + timeslotsConfig.slotDuration * modifiers.timeslotDurationMult / timeslotsConfig.shotsPerSlot;
                return;
            }

            if (Time.time >= nextSlot)
            {
                // Debug.Log("Slot started");
                // we changed slot.
                nextSlot = Time.time + (timeslotsConfig.slotDuration * modifiers.timeslotDurationMult);
            }


            // Debug.Log("Fire subslot" + pauseCounter.ToString());
            pauseCounter++;


            if (amIenemy)
            {
                GameObject target = vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer }); // should flash a raycast if debug on
                IsShooting = target != null; // if sees target, shoots target.
                // Debug.Log("i see you, i'm shooting!");
                if (IsShooting)
                {
                    Vector2 targetDirection = (target.transform.position - transform.position).normalized;
                    ExecuteSubslot(targetDirection, modifiers);
                }
            }
            else
            {
                IsShooting = true; // TODO: how does a player shoot?

                if (IsShooting)
                {
                    Vector2 targetDirection = gameObject.GetComponent<PlayerInput>().actions.FindAction("Direction").ReadValue<Vector2>().normalized;
                    //Debug.Log(targetDirection);
                    ExecuteSubslot(targetDirection, modifiers);
                }
            }

            nextSubslot = Time.time + timeslotsConfig.slotDuration * modifiers.timeslotDurationMult / timeslotsConfig.shotsPerSlot;
        }

    }

    public void ExecuteSubslot(Vector2 targetDirection, ShootingModifiers modifiers)
    {
        if (!IsShooting)
            return;

        // we changed subslot and this is not pause, we should fire.  ---------------
        // Debug.Log("I SHOULD FIRE NOW!");

        for (int i = 0; i < modifiers.axis; i++)
        {
            // Calculate evenly distributed angle
            float angle = 360.0f / modifiers.axis * i;
            Vector2 direction;
            direction = Quaternion.Euler(0, 0, angle) * targetDirection;
            // Debug.Log("Fire!");
            FireBullet(direction, modifiers);
        }

    }

    private void FireBullet(Vector2 targetDirection, ShootingModifiers modifiers)
    {
        //Debug.Log(amIenemy);
        GameObject bulletInstance = Instantiate(bulletPrefab, shooter.position, shooter.rotation);
        bulletInstance.GetComponent<NetworkObject>().Spawn();

        Bullet bullet = bulletInstance.GetComponent<Bullet>();
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