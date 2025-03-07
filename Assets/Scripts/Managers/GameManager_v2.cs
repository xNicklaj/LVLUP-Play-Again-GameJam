using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

class GameManager_v2 : Singleton<GameManager_v2>
{
    public UnityEvent OnGameStart = new UnityEvent();
    public UnityEvent OnGameFinish = new UnityEvent();
    public bool IsHost = false;

    private void Awake()
    {
        OnGameStart.AddListener(HandleOnGameStart);
    }

    private void HandleOnGameStart()
    {
        Debug.Log("Game Started");
    }
}