using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
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
public class OnGameFinishedScript : MonoBehaviour
{
    private VisualElement _root;
    [SerializeField] private AudioMixer _audioMixer;

    private VisualElement _HighScoreElement;
    private Label _csLabel;
    private Label _hsLabel;

    private float _ogMusicVolume;
    private float _ogSFXVolume;

    public GameState state = GameState.GameFinished;

    void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _root.visible = false;

        _HighScoreElement = _root.Q<VisualElement>("HighScoreElement");
        _csLabel = _root.Q<Label>("CSLabel");
        _hsLabel = _root.Q<Label>("HSLabel");
        _HighScoreElement.visible = false;

        if (state == GameState.GameLost)
        {
            GameManager_v2.Instance.OnGameLost.AddListener(Show);
        }
        else
        {
            GameManager_v2.Instance.OnGameFinish.AddListener(Show);
        }

        _audioMixer.GetFloat("MusicVolume", out _ogMusicVolume);
        _audioMixer.GetFloat("SFXVolume", out _ogSFXVolume);
    }

    private void Show()
    {
        _audioMixer.SetFloat("MusicVolume", -90);
        _audioMixer.SetFloat("SFXVolume", -90);
        _root.visible = true;
        _hsLabel.text = PointManager.Instance.HighScore.Value.ToString();
        _csLabel.text = PointManager.Instance.CurrentScore.Value.ToString();

        if (PointManager.Instance.CurrentScore.Value > PointManager.Instance.HighScore.Value)
        {
            PlayerPrefs.SetInt("HighScore", PointManager.Instance.CurrentScore.Value);
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
        else {
            Debug.LogWarning("sa solo quello che non era.");
        }
    }
}
