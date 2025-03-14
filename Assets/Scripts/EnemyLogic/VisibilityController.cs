using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class VisibilityController : NetworkBehaviour
{
    [Header("TestVisibility")]

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer) return;
        _spriteRenderer.maskInteraction = NetworkManager.Singleton.LocalClientId == 0 ? SpriteMaskInteraction.None : SpriteMaskInteraction.VisibleInsideMask;
    }
}