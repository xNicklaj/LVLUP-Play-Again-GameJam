using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerControllerMP : NetworkBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _playerRenderer;
    private InputAction _movement;
    private Vector2 directionInput;
    private Rigidbody2D rb;
    public float MoveSpeed = 5f;
    public float RotationSpeed = 10f;
    
    private void Awake()
    {
        if(_playerInput == null)
            _playerInput = GetComponent<PlayerInput>();
        if(_playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on GameObject. Please add one.");
            Destroy(this.gameObject);
            return;
        }

        _movement = _playerInput.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        Vector2 axis = _movement.ReadValue<Vector2>();
        Vector3 newPos = this.transform.position;
        Debug.Log(axis);
        newPos.x += axis.x * MoveSpeed * Time.deltaTime;
        newPos.y += axis.y * MoveSpeed * Time.deltaTime;
        this.transform.position = newPos;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        // rotation
        if (Gamepad.current != null && directionInput.sqrMagnitude > 0.01f) // controller
        {
            float targetAngle = Mathf.Atan2(directionInput.y, directionInput.x) * Mathf.Rad2Deg;
            _playerRenderer.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
        else if (Mouse.current != null && directionInput.sqrMagnitude > 0.01f) // mouse
        {
            Vector3 diff = Camera.main.ScreenToWorldPoint(directionInput)- _playerRenderer.transform.position;
            diff.Normalize();
            if (diff.sqrMagnitude > 0.01f)
            {
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                _playerRenderer.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
            }
        }
    }

    public void OnDirection(InputValue value)
    {
        directionInput = value.Get<Vector2>();
    }
}