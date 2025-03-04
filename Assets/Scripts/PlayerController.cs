using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 lookInput;
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

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // movement
        rb.linearVelocity = moveInput * MoveSpeed;
        
        // rotation
        if (Gamepad.current != null && lookInput.sqrMagnitude > 0.01f) // controller
        {
            float targetAngle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
        else if (Mouse.current != null && lookInput.sqrMagnitude > 0.01f) // mouse
        {
            Vector3 diff = Camera.main.ScreenToWorldPoint(lookInput)- transform.position;
            diff.Normalize();
            if (diff.sqrMagnitude > 0.01f)
            {
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
            }
        }
    }
}