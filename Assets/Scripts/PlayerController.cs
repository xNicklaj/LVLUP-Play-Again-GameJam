using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 directionInput;
    private CharacterController controller; // Nuovo CharacterController
    public PlayerStats playerStats; // Riferimento allo ScriptableObject

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnDirection(InputValue value)
    {
        directionInput = value.Get<Vector2>();
    }

    private void Update()
    {
        // movement
        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0) * (playerStats.MoveSpeed * Time.deltaTime);
        controller.Move(moveDirection);

        // rotation
        if (Gamepad.current != null && directionInput.sqrMagnitude > 0.01f) // controller
        {
            float targetAngle = Mathf.Atan2(directionInput.y, directionInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetAngle), playerStats.RotationSpeed * Time.deltaTime);
        }
        else if (Mouse.current != null && directionInput.sqrMagnitude > 0.01f) // mouse
        {
            Vector3 diff = Camera.main.ScreenToWorldPoint(directionInput) - transform.position;
            diff.Normalize();
            if (diff.sqrMagnitude > 0.01f)
            {
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot_z), playerStats.RotationSpeed * Time.deltaTime);
            }
        }
    }
}