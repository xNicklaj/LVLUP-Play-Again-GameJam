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
        ChangeHeartsClientRpc(health.Value);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner && !IsHost) return;
        if (GameManager_v2.Instance.GameStarted == false) return;
        if (!collision.gameObject.name.Contains("Bullet")) return;
        if (!(collision.gameObject.GetComponent<Bullet>().damageMult.Value > 0)) return;
        TakeDamageServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeHeartsClientRpc(int num)
    {
        GameManager_v2.Instance.HeartsChanged.Invoke(num);
        if (num <= 0)
            GameManager_v2.Instance.OnGameLost.Invoke();
    }
}
