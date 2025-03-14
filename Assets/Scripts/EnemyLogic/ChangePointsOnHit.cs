using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ChangePointsOnHit : MonoBehaviour
{
    public ChangePointsOnHitSettings Settings;
    public UnityEvent Death;

    private void Awake()
    {
        Death = new UnityEvent();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.name.Contains("Bullet") || collision.gameObject.GetComponent<Bullet>() == null)
            return;
        if (collision.gameObject.GetComponent<Bullet>().damageMult.Value <= 0) return;
        if (Settings.AmIPlayer && !collision.gameObject.GetComponent<Bullet>().isFromEnemy.Value) return;
        Debug.Log($"I am {gameObject.name}, and I was hit by enemy bullet, changing by {Settings.Amount}");
        PointManager.Instance.AddScoreServerRpc(Settings.Amount);
        if (!Settings.DestroyOnHit) return;
        DestroyThisObjectServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void DestroyThisObjectServerRpc()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        Death.Invoke();
        if (GetComponent<NetworkObject>() != null && GetComponent<NetworkObject>().IsSpawned) GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
}
