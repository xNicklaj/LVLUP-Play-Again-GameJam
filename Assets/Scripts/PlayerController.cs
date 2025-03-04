using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 directionInput;
    private Rigidbody2D rb;
    public float MoveSpeed = 5f;
    public float RotationSpeed = 10f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnDirection(InputValue value)
    {
        directionInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // movement
        rb.linearVelocity = moveInput * MoveSpeed;
        
        // rotation
        if (Gamepad.current != null && directionInput.sqrMagnitude > 0.01f) // controller
        {
            float targetAngle = Mathf.Atan2(directionInput.y, directionInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
        else if (Mouse.current != null && directionInput.sqrMagnitude > 0.01f) // mouse
        {
            Vector3 diff = Camera.main.ScreenToWorldPoint(directionInput)- transform.position;
            diff.Normalize();
            if (diff.sqrMagnitude > 0.01f)
            {
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
            }
        }
    }
}