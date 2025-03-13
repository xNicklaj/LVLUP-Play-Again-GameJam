using Unity.Netcode;
using UnityEngine;

public class HandleExitPoint : NetworkBehaviour
{
    public int ScoreAmount = 3000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("PlayerRecon")) return;
        if (GameManager_v2.Instance.GameStarted == false) return;
        PointManager.Instance.AddScoreServerRpc(ScoreAmount);
        FinishGameClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void FinishGameClientRpc()
    {
        GameManager_v2.Instance.OnGameFinish.Invoke();
    }
}
