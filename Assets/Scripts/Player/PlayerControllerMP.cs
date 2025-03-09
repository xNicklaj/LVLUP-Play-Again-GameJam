using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.InputSystem.Users;
using Unity.VisualScripting;
using UnityEngine.InputSystem.UI;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ClientNetworkAnimator))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerControllerMP : NetworkBehaviour
{
    public float MoveSpeedMult = 1.0f;
    public readonly float MoveSpeed = 5f;
    public float RotationSpeed = 10f;

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private InputDevice _device;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private NetworkObject _networkObject;
    [SerializeField] private Animator _animator;

    private InputAction _movement;
    private InputAction _look;
    private Vector2 directionInput;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _networkObject = GetComponent<NetworkObject>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _movement = _playerInput.actions.FindAction("Move");
        _look = _playerInput.actions.FindAction("Direction");
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
        Vector2 lookAxis = _look.ReadValue<Vector2>();
        _rigidbody.MovePosition(_rigidbody.position + (MoveSpeed * MoveSpeedMult * Time.deltaTime * axis));
        _animator.SetFloat("Horizontal", lookAxis.x);
        _animator.SetFloat("Vertical", lookAxis.y);
        _animator.SetFloat("Speed", axis.magnitude);

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
        if (!IsOwner) return;
        NetworkObject no = this.GetComponent<NetworkObject>();
        print(no);
        if(no && no.IsSpawned) no.Despawn();
        base.OnDestroy();
    }

    public void OnDirection(InputValue value)
    {
        directionInput = value.Get<Vector2>();
    }
}