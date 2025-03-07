using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

class GameManager_v2 : Singleton<GameManager_v2>
{
    public UnityEvent OnGameStart = new UnityEvent();
    public UnityEvent OnGameFinish = new UnityEvent();
    public UnityEvent OnGameLost = new UnityEvent();
    public UnityEvent OnUISelected = new UnityEvent();
    public UnityEvent OnPointsUpdated = new UnityEvent();
    public bool IsHost = false;
    [SerializeField] public bool GameStarted { get; private set; }  = false;

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

    private void HandlePointsUpdated()
    {
        if (GameStarted && PointManager.Instance.CurrentScore.Value == 0)
            OnGameLost.Invoke();
    }

    private void HandleGameLost()
    {
        GameStarted = false;
    }
}