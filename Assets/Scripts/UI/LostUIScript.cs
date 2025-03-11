using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LostUIScript : MonoBehaviour
{
    private VisualElement _root;
    [SerializeField] private AudioMixer _audioMixer;

    private VisualElement _HighScoreElement;
    private Label _csLabel;
    private Label _hsLabel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _root.visible = false;

        _HighScoreElement = _root.Q<VisualElement>("HighScoreElement");
        _csLabel = _root.Q<Label>("CSLabel");
        _hsLabel = _root.Q<Label>("HSLabel");
        _HighScoreElement.visible = false;

        GameManager_v2.Instance.OnGameLost.AddListener(() =>
        {
            _audioMixer.SetFloat("MusicVolume", -90);
            _audioMixer.SetFloat("SFXVolume", -90);
            _root.visible = true;
            _hsLabel.text = PointManager.Instance.HighScore.Value.ToString();
            _csLabel.text = PointManager.Instance.CurrentScore.Value.ToString();

            if(PointManager.Instance.CurrentScore.Value > PointManager.Instance.HighScore.Value)
            {
                PlayerPrefs.SetInt("HighScore", PointManager.Instance.CurrentScore.Value);
                _HighScoreElement.visible = true;
            }

            StartCoroutine(DisconnectAndReset());
        });
    }

    private IEnumerator DisconnectAndReset()
    {
        if(!NetworkManager.Singleton.IsServer) yield break;
        yield return new WaitForSeconds(5);
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        // TODO: needs more fixing
    }




}
