using UnityEngine;
using Unity.Netcode;
using System;

public class NetworkPlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _kingPrefab;
    [SerializeField] private GameObject _clientPrefab;
    [SerializeField] private GameObject _attackPrefab;
    [SerializeField] private GameObject _shieldPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnClientConnected(ulong clientId)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            SpawnClient(clientId, NetworkManager.Singleton.ConnectedClientsList.Count - 1);
        }
    }

    private void SpawnClient(ulong clientId,int prefabId) {
        GameObject newPlayer = null;

        switch (clientId)
        {
            case 0:
                newPlayer = (GameObject)Instantiate(_kingPrefab);
                break;
            case 1:
                newPlayer = (GameObject)Instantiate(_clientPrefab);
                break;
        }

        if (newPlayer == null) return;
        newPlayer.SetActive(true);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnSinglePlayerServerRpc(ulong clientId, int prefabId)
    {
        GameObject newPlayer = null;
        switch (clientId)
        {
            case 0:
                newPlayer = (GameObject)Instantiate(_attackPrefab);
                break;
            case 1:
                newPlayer = (GameObject)Instantiate(_shieldPrefab);
                break;
        }

        if (newPlayer == null) return;
        newPlayer.SetActive(true);
        newPlayer.GetComponent<NetworkObject>().Spawn(true);
        newPlayer.GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }
}

public enum PlayerType{
    King,
    Attack,
    Shielder
}