using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlaySceneLoader : NetworkBehaviour
{
    [SerializeField] private GameObject _kingPrefab;
    [SerializeField] private GameObject _attackPrefab;
    [SerializeField] private GameObject _shieldPrefab;
    [SerializeField] private SpawnPointsScriptable _spawnPointsScriptable;
    private NetworkVariable<Vector3> _sessionSpawnPoint = new NetworkVariable<Vector3>();

    private void Awake()
    {
        _sessionSpawnPoint.Value = _spawnPointsScriptable.list[Random.Range(0, _spawnPointsScriptable.list.Count)];
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        NetworkManager.Singleton.DisconnectClient(1);
        NetworkManager.Singleton.Shutdown();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnection;
    }

    private void OnClientConnection(ulong localClientId)
    {
        if (IsServer)
        {
            switch (localClientId)
            {
                case 0:
                    SpawnClientServerRpc(localClientId, (int)PlayerType.King);
                    break;
                case 1:
                    SpawnClientServerRpc(localClientId, (int)PlayerType.Attack);
                    SpawnClientServerRpc(localClientId, (int)PlayerType.Shielder);
                    StartCoroutine(StartGame());
                    break;
            }
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        GameManager_v2.Instance.OnGameStart.Invoke();
    }

    [Rpc(SendTo.Server)]
    private void SpawnClientServerRpc(ulong clientId, int prefabId)
    {
        GameObject newPlayer = null;

        switch (prefabId)
        {
            case (int)PlayerType.King:
                newPlayer = (GameObject)Instantiate(_kingPrefab, GetRandomPointAround(_sessionSpawnPoint.Value, _spawnPointsScriptable.spawnRadius), Quaternion.identity);
                break;
            case (int)PlayerType.Attack:
                newPlayer = (GameObject)Instantiate(_attackPrefab, GetRandomPointAround(_sessionSpawnPoint.Value, _spawnPointsScriptable.spawnRadius), Quaternion.identity);
                break;
            case (int)PlayerType.Shielder:
                newPlayer = (GameObject)Instantiate(_shieldPrefab, GetRandomPointAround(_sessionSpawnPoint.Value, _spawnPointsScriptable.spawnRadius), Quaternion.identity);
                break;
        }

        if (newPlayer == null) return;
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
    public static Vector3 GetRandomPointAround(Vector3 origin, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;

        float randomDistance = Random.Range(0f, radius);
        Vector3 offset = randomDirection * randomDistance;
        if (Random.Range(0, 1) == 0) return origin + offset;
        else return origin - offset;
    }
}
