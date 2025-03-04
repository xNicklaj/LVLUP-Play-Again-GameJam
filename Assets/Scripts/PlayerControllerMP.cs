using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(NetworkObject))]
public class PlayerControllerMP : NetworkBehaviour
{
    public float MoveSpeed = 5f;
    public float RotationSpeed = 10f;

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private InputDevice _device;
    [SerializeField] private GameObject _playerRenderer;
    [SerializeField] private NetworkObject _networkObject;
    private InputAction _movement;
    private Vector2 directionInput;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _networkObject = GetComponent<NetworkObject>();
        
        _movement = _playerInput.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetControllerDevice(PlayerInput _playerInput, InputDevice device)
    {
        _playerInput = _playerInput;
        _device = device;
    }

    private void Update()
    {
        if (!IsOwner || _playerInput == null || _device == null) return;
        Vector2 axis = _movement.ReadValue<Vector2>();
        Vector3 newPos = this.transform.position;
        Debug.Log(axis);
        newPos.x += axis.x * MoveSpeed * Time.deltaTime;
        newPos.y += axis.y * MoveSpeed * Time.deltaTime;
        this.transform.position = newPos;
    }

    private void FixedUpdate()
    {
        if (!IsOwner || _playerInput == null || _device == null) return;
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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner) _playerInput.enabled = true;
    }

    public void OnDirection(InputValue value)
    {
        directionInput = value.Get<Vector2>();
    }
}