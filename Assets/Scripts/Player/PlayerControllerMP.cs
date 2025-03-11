using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.InputSystem.Users;
using Unity.VisualScripting;
using UnityEngine.InputSystem.UI;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ClientNetworkAnimator))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerControllerMP : NetworkBehaviour
{
    public float MoveSpeed = 5f;
    public float MoveSpeedInAction = 1f;
    public float RotationSpeed = 10f;
    public bool isInAction = false;

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private InputDevice _device;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private NetworkObject _networkObject;
    [SerializeField] private Animator _animator;

    private InputAction _movement;
    private InputAction _look;
    private Vector2 directionInput;
    private Rigidbody2D rb;
    private float currSpeed;


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

        checkInAction();
        currSpeed = isInAction ? MoveSpeedInAction : MoveSpeed;

        Vector2 axis = _movement.ReadValue<Vector2>();
        Vector2 lookAxis = _look.ReadValue<Vector2>();

        _rigidbody.MovePosition(_rigidbody.position + (currSpeed * Time.deltaTime * axis));

        if (lookAxis.magnitude > 0.1)
            UpdateAnimator(lookAxis.x, lookAxis.y, axis.magnitude);
        else
            UpdateAnimator(axis.x, axis.y, axis.magnitude);

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
        if (no && no.IsSpawned) no.Despawn();
        base.OnDestroy();
    }

    public void OnDirection(InputValue value)
    {
        directionInput = value.Get<Vector2>();
    }

    private void UpdateAnimator(float horizontal, float vertical, float speed)
    {
        _animator.SetFloat("Horizontal", horizontal);
        _animator.SetFloat("Vertical", vertical);
        _animator.SetFloat("Speed", speed);
    }

    private void checkInAction()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        Vector2 targetDirection = input.actions.FindAction("Direction").ReadValue<Vector2>().normalized;
        isInAction = targetDirection.magnitude > 0;
    }
}