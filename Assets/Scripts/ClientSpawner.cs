using UnityEngine;
using Unity.Netcode;

public class ClientSpawner : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
