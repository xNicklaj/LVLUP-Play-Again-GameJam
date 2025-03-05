using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ShootingSystem : MonoBehaviour
{
    #region References
    public GameObject bulletPrefab;
    public VisionSystem vision;
    private Transform shooter;
    #endregion

    public struct ShootingModifiers
    {
        public bool homing;
        public int shots;
        public int axis;
        public float timeslotDurationMult; // minimum should be 1/10 of a second
    }

    public struct SlotsConfig
    {
        public float slotDuration;
        public int shotsPerSlot;
        public int pauseSlotsNumber;
    }

    #region ShootingVariables
    private bool _isShooting = true;
    public ShootingModifiers modifiers = new ShootingModifiers
    {
        axis = 3,
        homing = true,
        shots = 1 // TODO: unimplemented for now
    };
    public SlotsConfig timeslotsConfig = new SlotsConfig
    {
        slotDuration = 1.0f,
        pauseSlotsNumber = 2,
        shotsPerSlot = 4
    };
    public Color bulletColor = Color.white;
    private float nextSlot;
    private float nextSubslot;
    private int pauseCounter = 0;
    private bool amIenemy = true;

    public bool IsShooting { get => _isShooting; set => _isShooting = value; }
    #endregion

    private void Start()
    {
        amIenemy = gameObject.tag != "Player";
        nextSlot = Time.time;
        shooter = gameObject.transform;
        vision = gameObject.GetComponent<VisionSystem>();
        Debug.Assert(bulletPrefab != null, "Bullet prefab not assigned");

        Debug.Assert(vision != null, "vision component is null");
    }

    private void Update()
    {
        if (Time.time >= nextSubslot)
        {
            Debug.Log("Subslot started");

            if (pauseCounter >= timeslotsConfig.shotsPerSlot)
            {
                // it's a pause slot
                Debug.Log("Pause subslot..." + pauseCounter.ToString());
                pauseCounter++;
                pauseCounter %= timeslotsConfig.shotsPerSlot + timeslotsConfig.pauseSlotsNumber * timeslotsConfig.shotsPerSlot + 1;

                nextSubslot = Time.time + timeslotsConfig.slotDuration / timeslotsConfig.shotsPerSlot;
                return;
            }

            if (Time.time >= nextSlot)
            {
                Debug.Log("Slot started");
                // we changed slot.
                nextSlot = Time.time + timeslotsConfig.slotDuration;
            }


            Debug.Log("Fire subslot" + pauseCounter.ToString());
            pauseCounter++;
            if (amIenemy)
            {
                GameObject target = vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer });
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
                IsShooting = false; // how does a player shoot?

                if (IsShooting)
                {
                    Vector2 targetDirection = gameObject.GetComponent<PlayerInput>().actions.FindAction("Direction").ReadValue<Vector2>();
                    ExecuteSubslot(targetDirection, modifiers);
                }
            }

            nextSubslot = Time.time + timeslotsConfig.slotDuration / timeslotsConfig.shotsPerSlot;
        }

    }

    public void ExecuteSubslot(Vector2 targetDirection, ShootingModifiers modifiers)
    {
        if (!IsShooting)
        {
            return;
        }

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
        GameObject bulletInstance = Instantiate(bulletPrefab, shooter.position, shooter.rotation);

        Bullet bullet = bulletInstance.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.direction = targetDirection;
            bullet.isFromEnemy = amIenemy;
            bullet.color = bulletColor;
            bullet.isHoming = modifiers.homing;
        }
    }
}