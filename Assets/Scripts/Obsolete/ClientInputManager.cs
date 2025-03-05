
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[Obsolete("Do not use unless you know what you're doing.")]
class ClientInputManager : Singleton<ClientInputManager>
{
    private Dictionary<InputDevice, bool> _deviceSemaphoreArray = new Dictionary<InputDevice, bool>();

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        //if(NetworkManager.Singleton.IsClient) FillDevices(clientId);
    }

    private void FillDevices(ulong clientId)
    {
        ReadOnlyArray<InputDevice> devices = InputSystem.devices;
        foreach (var device in devices)
        {
            _deviceSemaphoreArray.Add(device, false);
        }
    }

    public InputDevice GetFirstAvailableDevice(ulong clientId)
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            if (_deviceSemaphoreArray[device] == true) continue;

            // If device is available
            _deviceSemaphoreArray[device] = true;
            return device;
        }
        return null;
    }

}