using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DustParticles : NetworkBehaviour
{
    public AudioClip Sound;

    private AudioSource _source;


    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if(_source.clip == null && Sound != null) _source.clip = Sound;
    }

    public override void OnNetworkSpawn()
    {
        PlaySoundRpc();
        StartCoroutine(DespawnAtEndOfClip(_source.clip.length + .5f));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlaySoundRpc()
    {
        _source.Play();
    }

    private IEnumerator DespawnAtEndOfClip(float time)
    {
        if(!IsHost) yield break;
        yield return new WaitForSeconds(time);
        if(GetComponent<NetworkObject>().IsSpawned) GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
