using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InstrumentNetworkController : NetworkBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _look;

    public UnityEvent<bool> UseMultipleShields = new UnityEvent<bool>();


    public NetworkVariable<Vector2> rightStickAxis = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private void Start()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        if (_playerInput)
        {
            _look = _playerInput.actions.FindAction("Direction");
        }
        
    }

    void Update()
    {
        if (!IsOwner) return;
        
        rightStickAxis.Value = _look.ReadValue<Vector2>();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UseMultipleShieldsClientRpc(bool arg0)
    {
        UseMultipleShields.Invoke(arg0);
    }
}