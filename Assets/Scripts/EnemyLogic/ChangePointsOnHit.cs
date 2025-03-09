using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class ChangePointsOnHit : MonoBehaviour
{
    public int Amount = 100;
    public bool DestroyOnHit = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.name.Contains("Bullet") || collision.gameObject.GetComponent<Bullet>() == null)
            return;
        
        PointManager.Instance.AddScoreServerRpc(Amount);
        if (!DestroyOnHit) return;
        DestroyThisObjectServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void DestroyThisObjectServerRpc()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        if (GetComponent<NetworkObject>().IsSpawned) GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
}
