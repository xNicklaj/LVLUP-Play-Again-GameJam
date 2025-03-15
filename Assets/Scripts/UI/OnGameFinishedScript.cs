using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum GameState
{
    GameLost,
    GameFinished
}

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(AudioSource))]
public class OnGameFinishedScript : NetworkBehaviour
{
    private VisualElement _root;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _audioSource;

    public AudioClip WinSound;
    public AudioClip LoseSound;

    private VisualElement _HighScoreElement;
    private Label _csLabel;
    private Label _hsLabel;

    private float _ogMusicVolume;
    private float _ogSFXVolume;

    public GameState state = GameState.GameFinished;

    void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _audioSource = GetComponent<AudioSource>();
        _root.visible = false;

        _HighScoreElement = _root.Q<VisualElement>("HighScoreElement");
        _csLabel = _root.Q<Label>("CSLabel");
        _hsLabel = _root.Q<Label>("HSLabel");
        _HighScoreElement.visible = false;

        if (state == GameState.GameLost)
        {
            GameManager_v2.Instance.OnGameLost.AddListener(Show);
            _audioSource.clip = LoseSound;
        }
        else
        {
            GameManager_v2.Instance.OnGameFinish.AddListener(Show);
            _audioSource.clip = WinSound;
        }

        _audioMixer.GetFloat("MusicVolume", out _ogMusicVolume);
        _audioMixer.GetFloat("SFXVolume", out _ogSFXVolume);
    }

    private void Show()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        ShowClientRpc(
            PointManager.Instance.HighScore.Value.ToString(),
            PointManager.Instance.CurrentScore.Value.ToString()
        );
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShowClientRpc(string highScore, string currentScore)
    {
        Debug.Log("ShowClientRpc received!");
        _audioSource.Play();

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"SERVER: Showing game over screen - HighScore: {highScore}, CurrentScore: {currentScore}");
        }
        else
        {
            Debug.Log($"CLIENT: Showing game over screen - HighScore: {highScore}, CurrentScore: {currentScore}");
        }

        _audioMixer.SetFloat("MusicVolume", -90);
        _audioMixer.SetFloat("SFXVolume", -90);
        _hsLabel.text = highScore;
        _csLabel.text = currentScore;
        _root.visible = true;

        if (int.Parse(currentScore) > int.Parse(highScore))
        {
            PlayerPrefs.SetInt("HighScore", int.Parse(currentScore));
            _HighScoreElement.visible = true;
        }

        StartCoroutine(DisconnectAndReset());
    }

    private IEnumerator DisconnectAndReset()
    {
        yield return new WaitForSeconds(5);  // delay before main menu

        bool wasClient = NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost;
        bool wasHost = NetworkManager.Singleton.IsHost;

        NetworkManager.Singleton.Shutdown(); // Properly shut down the network
        yield return new WaitForSeconds(1);  // Small delay for proper cleanup

        _audioMixer.SetFloat("MusicVolume", _ogMusicVolume);
        _audioMixer.SetFloat("SFXVolume", _ogSFXVolume);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        yield return new WaitForSeconds(1);  // Wait for scene load

        // Restart network based on previous state
        if (wasHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else if (wasClient)
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.LogWarning("sa solo quello che non era.");
        }
    }
}
