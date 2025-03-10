using Unity.Netcode;
using UnityEngine;

public class HealthManager : NetworkSingleton<HealthManager>
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc()
    {
        health.Value -= 1;
        GameManager_v2.Instance.ChangeHeartsClientRpc(health.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CallGameLostClientRpc()
    {
        GameManager_v2.Instance.OnGameLost.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner && !IsHost) return;
        if (GameManager_v2.Instance.GameStarted == false) return;
        if (collision.gameObject.name.Contains("Bullet"))
        {
            TakeDamageServerRpc();
        }
    }
}
