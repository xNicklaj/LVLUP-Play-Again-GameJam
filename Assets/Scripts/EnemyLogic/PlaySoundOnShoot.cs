using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnShoot : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ShootingSystem _shootingSystem;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _shootingSystem = GetComponent<ShootingSystem>();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayShootingClipClientRpc()
    {
        _audioSource.Play();
    }

}
