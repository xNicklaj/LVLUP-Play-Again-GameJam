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

    public GameState state = GameState.GameFinished;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            GameManager_v2.Instance.OnGameLost.AddListener(() =>
            {
                Show();
            });
        }else
        {
            GameManager_v2.Instance.OnGameFinish.AddListener(() =>
            {
                Show();
            });
        }
        
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
        if(!NetworkManager.Singleton.IsServer) yield break;
        yield return new WaitForSeconds(5);
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        // TODO: needs more fixing
    }




}
