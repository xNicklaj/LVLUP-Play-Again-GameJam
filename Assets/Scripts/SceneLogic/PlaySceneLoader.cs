using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlaySceneLoader : NetworkBehaviour
{
    [SerializeField] private GameObject _kingPrefab;
    [SerializeField] private GameObject _attackPrefab;
    [SerializeField] private GameObject _shieldPrefab;
    [SerializeField] private SpawnPointsScriptable _spawnPointsScriptable;
    private NetworkVariable<Vector3> _sessionSpawnPoint = new NetworkVariable<Vector3>();

    public float DelayBeforeStart = 3f;

    private void Awake()
    {
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _sessionSpawnPoint.Value = _spawnPointsScriptable.list[Random.Range(0, _spawnPointsScriptable.list.Count)];

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnection;

            // Subscribe the new instance
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnection;
        }
    }

    private void OnClientConnection(ulong clientId)
    {
        if (this == null || gameObject == null)
        {
            Debug.LogWarning("PlaySceneLoader was destroyed but OnClientConnection was still called. Ignoring.");
            return;
        }

        if (!IsServer) return;

        Debug.Log($"Client {clientId} connected. Spawning their player.");

        // Assign roles based on connection order
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 1)
        {
            SpawnPlayer(clientId, PlayerType.King);
        }
        else if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            SpawnPlayer(clientId, PlayerType.Attack);
            SpawnPlayer(clientId, PlayerType.Shielder);
        }

        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(DelayBeforeStart);
        GameManager_v2.Instance.OnGameStart.Invoke();
    }

    private void SpawnPlayer(ulong clientId, PlayerType type)
    {
        GameObject prefab = type switch
        {
            PlayerType.King => _kingPrefab,
            PlayerType.Attack => _attackPrefab,
            PlayerType.Shielder => _shieldPrefab,
            _ => null
        };

        if (prefab == null) return;

        Vector3 spawnPos = GetRandomPointAround(_sessionSpawnPoint.Value, _spawnPointsScriptable.spawnRadius);
        GameObject newPlayer = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Make sure it's a networked object and assign it to the correct player
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.SpawnAsPlayerObject(clientId, true);
        }
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
