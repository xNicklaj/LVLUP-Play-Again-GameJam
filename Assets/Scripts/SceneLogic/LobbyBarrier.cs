using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LobbyBarrier : NetworkBehaviour
{
    void Awake()
    {
         GameManager_v2.Instance.OnGameStart.AddListener(() => DestroyBarrierClientRpc());
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DestroyBarrierClientRpc()
    {
        GetComponent<TilemapRenderer>().enabled = false;
        GetComponent<TilemapCollider2D>().enabled = false;
    }
}
