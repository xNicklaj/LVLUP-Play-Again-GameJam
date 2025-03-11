using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class FrogAttackModifier : ModifierBase
{
    public GameObject FrogPrefab;
    public float Radius = 5;

    protected override void ApplyModifier()
    {
        DespawnEnemiesServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void DespawnEnemiesServerRpc()
    {
        LayerMask[] masks = new LayerMask[] { LayerMask.GetMask("LitEnemy"), LayerMask.GetMask("UnlitEnemy") };
        List<Collider2D> colliders = VisionSystem.FindNearPosition(transform.position, Radius, masks);
        foreach (Collider2D collider in colliders)
        {
            collider.TryGetComponent<NetworkObject>(out NetworkObject no);
            if (no == null) continue;
            if (no.IsSpawned) no.Despawn();
            Vector3 pos = collider.transform.position;
            Destroy(collider.gameObject);

            SpawnFrogRpc(pos);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnFrogRpc(Vector3 pos)
    {
        if (FrogPrefab == null) return;
        GameObject frog = Instantiate(FrogPrefab, pos, Quaternion.identity);
    }

    protected override void DisposeModifier()
    {
        
    }

}
