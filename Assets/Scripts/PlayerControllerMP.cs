using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.InputSystem.Users;
using Unity.VisualScripting;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
public class PlayerControllerMP : NetworkBehaviour
{
    public float MoveSpeed = 5f;
    public float RotationSpeed = 10f;

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private InputDevice _device;
    [SerializeField] private GameObject _playerRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private NetworkObject _networkObject;
    private InputAction _movement;
    private Vector2 directionInput;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _networkObject = GetComponent<NetworkObject>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _movement = _playerInput.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetControllerDevice(PlayerInput playerInput, InputDevice device)
    {
        _playerInput = playerInput;
        _device = device;
    }

    private void Update()
    {
        if (!IsOwner) return;
        Vector2 axis = _movement.ReadValue<Vector2>();
        Vector3 newPos = this.transform.position;
        //Debug.Log(axis);
        newPos.x += axis.x * MoveSpeed * Time.deltaTime;
        newPos.y += axis.y * MoveSpeed * Time.deltaTime;
        //this.transform.position = newPos;

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        
        _rigidbody.MovePosition(_rigidbody.position + (MoveSpeed * Time.fixedDeltaTime *  _movement.ReadValue<Vector2>()));

        // rotation
        float targetAngle = Mathf.Atan2(directionInput.y, directionInput.x) * Mathf.Rad2Deg;
        _playerRenderer.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            _playerInput.enabled = true;
            //_playerInput.uiInputModule = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputSystemUIInputModule>();
        }
    }

    public override void OnDestroy()
    {
        Debug.Log("Aiutooo");
        System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
        Debug.Log(t.ToString());
        if (!IsOwner) return;
        this.GetComponent<NetworkObject>().Despawn();
        base.OnDestroy();
    }

    public void OnDirection(InputValue value)
    {
        directionInput = value.Get<Vector2>();
    }
}