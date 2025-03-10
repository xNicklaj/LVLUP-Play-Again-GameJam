using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DustParticles : MonoBehaviour
{
    public AudioClip Sound;

    private AudioSource _source;


    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if(_source.clip == null && Sound != null) _source.clip = Sound;
        PlaySound();
        StartCoroutine(DespawnAtEndOfClip(_source.clip.length + .4f));
    }
    private void PlaySound()
    {
        _source.Play();
    }

    private IEnumerator DespawnAtEndOfClip(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
