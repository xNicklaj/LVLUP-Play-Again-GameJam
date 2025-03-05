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

        
    }

    private void FixedUpdate()
    {
        float targetAngle = Mathf.Atan2(directionInput.y, directionInput.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}