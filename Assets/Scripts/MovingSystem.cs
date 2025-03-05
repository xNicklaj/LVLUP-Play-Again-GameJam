using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class MovingSystem : MonoBehaviour
{
    #region References
    private Camera mainCamera;
    private PlayerInput playerInput;
    public MovingSystemDefaults defaults; // to be set in the Inspector
    #endregion

    #region InputVariables
    // Input variables
    private Vector2 movementInput;
    private Vector2 lookDirection;
    #endregion

    #region MovementVariables
    public float speedMult = 1.0f;
    public float accelerationMult = 1.0f;
    public float decelerationMult = 1.0f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();

        Debug.Assert(playerInput != null, "PlayerInput component missing");
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        HandleRotation();
    }

    // FixedUpdate is called once per physics update
    void FixedUpdate()
    {
        HandleMovement();
    }

    // Handle player input
    void GetInput()
    {
        movementInput = playerInput.actions.FindAction("Move").ReadValue<Vector2>();
        lookDirection = playerInput.actions.FindAction("Direction").ReadValue<Vector2>();
    }

    // Handle player movement
    void HandleMovement()
    {
        Vector2 targetVelocity = movementInput * defaults.speed * speedMult;

        // Calculate acceleration based on whether we're accelerating or decelerating
        float accelerationRate = (movementInput.magnitude > 0.1f) ? defaults.acceleration * accelerationMult : defaults.deceleration * decelerationMult;

        Vector2 currentVelocity = new Vector2(0.0f, 0.0f);

        // Apply acceleration with smoothing
        currentVelocity.x = Mathf.MoveTowards(
            currentVelocity.x,
            targetVelocity.x,
            accelerationRate * Time.deltaTime
        );

        currentVelocity.y = Mathf.MoveTowards(
            currentVelocity.y,
            targetVelocity.y,
            accelerationRate * Time.deltaTime
        );

        transform.position += new Vector3(currentVelocity.x, currentVelocity.y, 0) * Time.deltaTime;
    }

    // Handle player rotation
    void HandleRotation()
    {

        // if (lookDirection.magnitude > 0.01f) {
        //     // Normalize to prevent faster diagonal movement
        //     lookDirection = lookDirection.normalized;

        //     // Calculate angle from look direction
        //     float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;

        //     // Apply rotation
        //     transform.rotation = Quaternion.Euler(0, 0, angle);
        // }

        Vector2 scale = transform.localScale;
        if (lookDirection.magnitude > 0.01f)
        {
            if ((lookDirection.x > 0 && scale.x < 0) || (lookDirection.x < 0 && scale.x > 0))
            {
                transform.localScale = new Vector2(-scale.x, scale.y); // flip the sprite
                return;
            }
        }
        // WARN: keep the two IFs separate so that looking has priority over movement
        // TODO: this does not work, add a debounce?...
        if (movementInput.magnitude > 0.01f)
        {
            if ((movementInput.x < 0 && scale.x > 0) || (movementInput.x > 0 && scale.x < 0))
            {
                transform.localScale = new Vector2(-scale.x, scale.y); // flip the sprite
                return;
            }
        }
    }
}