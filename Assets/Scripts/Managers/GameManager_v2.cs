using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

class GameManager_v2 : Singleton<GameManager_v2>
{
    public UnityEvent OnGameStart = new UnityEvent();
    public UnityEvent OnGameFinish = new UnityEvent();
    public UnityEvent OnGameLost = new UnityEvent();
    public UnityEvent OnUISelected = new UnityEvent();
    public UnityEvent<int> HeartsChanged = new UnityEvent<int>();
    public UnityEvent<int> OnPointsUpdated = new UnityEvent<int>();
    public bool IsSessionHost = false;
    [SerializeField] public bool GameStarted { get; private set; } = false;

    private void Awake()
    {
        OnGameStart.AddListener(HandleOnGameStart);
        OnPointsUpdated.AddListener(HandlePointsUpdated);
        OnGameLost.AddListener(HandleGameLost);
    }

    private void HandleOnGameStart()
    {
        GameStarted = true;
        Debug.Log("Game Started");
    }

    private void HandlePointsUpdated(int value)
    {
        if (value == 0)
            OnGameLost.Invoke();
    }

    private void HandleGameLost()
    {
        GameStarted = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeHeartsClientRpc(int num)
    {
        HeartsChanged.Invoke(num);
        if (num <= 0)
            OnGameLost.Invoke();
    }
}