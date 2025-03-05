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
        if (!IsOwner) return;
        base.OnNetworkSpawn();
        NetworkManager.Singleton.GetComponent<NetworkPlayerSpawner>().SpawnSinglePlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        NetworkManager.Singleton.GetComponent<NetworkPlayerSpawner>().SpawnSinglePlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
