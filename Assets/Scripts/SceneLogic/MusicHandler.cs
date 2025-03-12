using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicHandler : Singleton<MusicHandler>
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
            GetComponent<AudioSource>().clip = GameTheme;
            GetComponent<AudioSource>().time = 1;
            GetComponent<AudioSource>().Play();
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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
            GetComponent<AudioSource>().mute = !GetComponent<AudioSource>().mute;
    }
}
