using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(AudioSource))]
public class MusicHandler : NetworkSingleton<MusicHandler>
{
    public AudioClip MenuTheme;
    public AudioClip GameTheme;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager_v2.Instance.OnMainMenuStarted.AddListener(() =>
        {
            GetComponent<AudioSource>().clip = MenuTheme;
            GetComponent<AudioSource>().Play();
        });
        GameManager_v2.Instance.OnGameStart.AddListener(() =>
        {
            PlayGameMusicClientRpc();
        });
        GameManager_v2.Instance.OnMainMenuExit.AddListener(() =>
        {
            GetComponent<AudioSource>().Stop();
        });
        GameManager_v2.Instance.OnGameFinish.AddListener(() =>
        {
            GetComponent<AudioSource>().Stop();
        });
        GameManager_v2.Instance.OnGameLost.AddListener(() =>
        {
            GetComponent<AudioSource>().Stop();
        });
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayGameMusicClientRpc()
    {
        GetComponent<AudioSource>().clip = GameTheme;
        GetComponent<AudioSource>().time = 1;
        GetComponent<AudioSource>().Play();
    }

}