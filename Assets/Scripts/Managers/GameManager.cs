

using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInputManager))]
class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] private GameObject _kingPrefab;
    [SerializeField] private GameObject _attackPrefab;
    [SerializeField] private GameObject _shieldPrefab;
    private PlayerInputManager _playerInputManager;
    public string PlayScene = "PlayScene";
    public bool IsHostClient = false;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _playerInputManager = GetComponent<PlayerInputManager>();
    }

    public void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void SpawnClient(ulong clientId, int prefabId)
    {
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

    private void OnServerStarted()
    {
        NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
        NetworkManager.Singleton.SceneManager.LoadScene(PlayScene, LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if(sceneName != SceneManager.GetSceneByBuildIndex(1).name) return;
        if (NetworkManager.Singleton.IsServer)
        {
            if (clientId == 0)
            {
                SpawnClient(clientId, (int)PlayerType.King);
            }

            if (clientId == 1)
            {
                SpawnClient(clientId, (int)PlayerType.Attack);
                SpawnClient(clientId, (int)PlayerType.Shielder);
            }

            if (clientId >= 2) return;
        }

        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            _playerInputManager.splitScreen = false;
        }
        else
        {
            // Client code
            _playerInputManager.splitScreen = true;
        }
    }
}
public enum PlayerType
{
    King,
    Attack,
    Shielder
}