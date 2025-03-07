using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager_v2.Instance.OnGameStart.AddListener(() =>
        {
            GetComponent<AudioSource>().Play();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
