using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(NetworkTransform))]
public abstract class ModifierBase : NetworkBehaviour
{
    public ModifierData ModifierData;

    protected GameObject ModifierTarget;

    private SpriteRenderer _spriteRenderer;
    private float _despawnTimer = 0;
    private bool _isDespawning = false;

    protected abstract void ApplyModifier();
    protected abstract void DisposeModifier();

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<BoxCollider2D>().isTrigger = true;
        if(_spriteRenderer.sprite == null) _spriteRenderer.sprite = ModifierData.Sprite;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ModifierTarget = GameObject.Find(ModifierData.TargetName + "(Clone)");
    }

    private void Update()
    {
        if (_isDespawning) HandleDespawn();
        HandleBobbing();
    }

    private void HandleBobbing(float bobbingSpeed = 3f, float bobbingHeight = .005f)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + math.sin(Time.time * bobbingSpeed) * bobbingHeight, 0);
    }

    private void HandleDespawn()
    {
        _despawnTimer += Time.deltaTime;

        if (_despawnTimer >= ModifierData.DespawnTimeoutDuration && !IsHost) return;
        if (_despawnTimer >= ModifierData.DespawnTimeoutDuration && IsHost)
            DespawnAndDestroyServerRpc();

        float frequency = ModifierData.DespawnFrequencyCurve.Evaluate(_despawnTimer / ModifierData.DespawnTimeoutDuration) * 9 + 1;

        NativeArray<float> jobInput = new NativeArray<float>(2, Allocator.Persistent);
        NativeArray<float> jobOutput = new NativeArray<float>(1, Allocator.Persistent);
        jobInput[0] = frequency;
        jobInput[1] = _despawnTimer;
        EvaluateOpacityJob job = new EvaluateOpacityJob
        {
            In = jobInput,
            Out = jobOutput
        };
        job.Schedule().Complete();
        jobInput.Dispose();

        SetOpacity(jobOutput[0]);

        jobOutput.Dispose();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if((ModifierData.TargetObject == null && collider.CompareTag("Player")) || collider.gameObject == ModifierTarget || collider.gameObject.name.Replace("(Clone)", "") == ModifierData.TargetObject.name)
        {
            ModifierTarget = collider.gameObject;
            OnPickupClientRpc(RpcTarget.Single(ModifierTarget.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
            AddToModifierListClientRpc();
            DisableRendererClientRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void DespawnAndDestroyServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DisableRendererClientRpc()
    {
        _spriteRenderer.enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddToModifierListClientRpc()
    {
        // TODO
    }

    [ContextMenu("Start Despawning")]
    [Rpc(SendTo.ClientsAndHost)]
    private void StartDespawnClientRpc()
    {
        _isDespawning = true;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void OnPickupClientRpc(RpcParams rpcParams)
    {
        if(ModifierData.TargetObject != null)
        {
            if (ModifierTarget == null) ModifierTarget = GameObject.Find(ModifierData.TargetName + "(Clone)");
            if (ModifierTarget == null)
            {
                Debug.LogError("No ModifierTarget with name " + ModifierData.TargetName + "(Clone)" + " found. The modifier will be discarded");
            }
        }
        ApplyModifier();
        StartCoroutine(WaitForDuration());
    }

    private IEnumerator WaitForDuration()
    {
        yield return new WaitForSeconds(ModifierData.Duration);
        DisposeModifier();
        DespawnAndDestroyServerRpc();
    }

    private void SetOpacity(float value)
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, value);
    }

    [BurstCompile]
    struct EvaluateOpacityJob : IJob
    {
        [ReadOnly]
        public NativeArray<float> In;
        [WriteOnly]
        public NativeArray<float> Out;

        public void Execute()
        {
            Out[0] = math.sin(In[0] * In[1]) >= 0 ? 1 : 0;
            return;
        }
    }
}

