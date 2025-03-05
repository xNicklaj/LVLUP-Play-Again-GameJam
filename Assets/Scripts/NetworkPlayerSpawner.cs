using UnityEngine;
using Unity.Netcode;
using System;

public class NetworkPlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _kingPrefab;
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
            if(clientId == 0)
            {
                SpawnClient(clientId, (int)PlayerType.King);
            }
            
            if(clientId == 1)
            {
                SpawnClient(clientId, (int)PlayerType.Attack);
                SpawnClient(clientId, (int)PlayerType.Shielder);
            }

            if (clientId >= 2) return;
        }
    }

    private void SpawnClient(ulong clientId,int prefabId) {
        GameObject newPlayer = null;

        switch (prefabId)
        {
            case (int)PlayerType.King:
                newPlayer = (GameObject)Instantiate(_kingPrefab);
                break;
            case (int)PlayerType.Attack:
                newPlayer = (GameObject)Instantiate(_attackPrefab);
                break;
            case (int)PlayerType.Shielder:
                newPlayer = (GameObject)Instantiate(_shieldPrefab);
                break;
        }

        if (newPlayer == null) return;
        newPlayer.SetActive(true);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}

public enum PlayerType{
    King,
    Attack,
    Shielder
}