using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

class GameManager_v2 : Singleton<GameManager_v2>
{
    public UnityEvent OnGameStart = new UnityEvent();
    public UnityEvent OnGameFinish = new UnityEvent();
    public UnityEvent OnGameLost = new UnityEvent();
    public UnityEvent OnUISelected = new UnityEvent();
    public UnityEvent OnMainMenuStarted = new UnityEvent();
    public UnityEvent OnMainMenuExit = new UnityEvent();
    public UnityEvent<int> HeartsChanged = new UnityEvent<int>();
    public UnityEvent<int> OnPointsUpdated = new UnityEvent<int>();
    public UnityEvent OnBossEnemySpawn = new UnityEvent();
    public UnityEvent OnBossEnemyKill = new UnityEvent();
    public bool IsSessionHost = false;
    [SerializeField] public bool GameStarted { get; private set; } = false;

    private void Awake()
    {
        OnGameStart.AddListener(HandleOnGameStart);
        OnPointsUpdated.AddListener(HandlePointsUpdated);
        OnGameLost.AddListener(HandleGameLost);
        OnGameFinish.AddListener(HandleGameFinish);
        
        //OnBossEnemySpawn.AddListener((() => Debug.Log("Spawnato boss")));
        //OnBossEnemyKill.AddListener((() => Debug.Log("Killato boss")));
    }

    private void HandleGameFinish()
    {
        PointManager.Instance.StopCountingServerRpc();
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
        PointManager.Instance.StopCountingServerRpc();
    }
}