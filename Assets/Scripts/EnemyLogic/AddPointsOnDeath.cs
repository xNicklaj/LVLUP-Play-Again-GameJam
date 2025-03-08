using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class AddPointsOnDeath : MonoBehaviour
{
    public int Amount = 100;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.name.Contains("Bullet") || collision.gameObject.GetComponent<Bullet>() == null)
            return;
        PointManager.Instance.AddScore(Amount);
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
}
