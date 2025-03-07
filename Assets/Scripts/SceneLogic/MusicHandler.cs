using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager_v2.Instance.OnGameStart.AddListener(() =>
        {
            GetComponent<AudioSource>().time = 1;
            GetComponent<AudioSource>().Play();
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
        
    }
}
