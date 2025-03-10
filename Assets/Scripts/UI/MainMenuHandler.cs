using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class MainMenuHandler : MonoBehaviour
{
    private VisualElement _baseContainer;
    private VisualElement _joinContainer;
    private VisualElement _coinContainer;
    private Button _coinButton;
    private Button _hostGameButton;
    private Button _joinGameButton;
    private Button _joinButton;
    private Button _backButton;
    private Button _quitButton;
    private TextField _ipField;
    private Label _gameTitle;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private PlaySceneSettings _playSceneSettings;

    public AudioClip SelectSound;
    public AudioClip StartSound;
    public AudioClip FocusSound;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _gameTitle = root.Q<Label>("GameTitle");
        _coinContainer = root.Q<VisualElement>("CoinContainer");
        _baseContainer = root.Q<VisualElement>("BaseContainer");
        _joinContainer = root.Q<VisualElement>("JoinContainer");

        _coinButton = root.Q<Button>("CoinButton");
        _hostGameButton = root.Q<Button>("HostGameButton");
        _joinGameButton = root.Q<Button>("JoinGameButton");
        _joinButton = root.Q<Button>("JoinButton");
        _backButton = root.Q<Button>("BackButton");
        _quitButton = root.Q<Button>("QuitButton");
        _ipField = root.Q<TextField>("IPField");

        _hostGameButton.clicked += HostGameButtonClicked;
        _joinGameButton.clicked += JoinGameButtonClicked;
        _backButton.clicked += BackButtonClicked;
        _joinButton.clicked += JoinButtonClicked;
        _quitButton.clicked += QuitButtonClicked;
        _ipField.RegisterCallback<ChangeEvent<string>>(IPFieldFocusOut);
        _coinButton.clicked += () =>
        {
            _coinContainer.AddToClassList("disabled");
            _baseContainer.RemoveFromClassList("disabled");
            _hostGameButton.Focus();
            StopCoroutine(VisibilityLoop());
            
            _hostGameButton.AddToClassList("disabled");
            _joinGameButton.AddToClassList("disabled");
            _quitButton.AddToClassList("disabled");
            StartCoroutine(ShowMainMenu());
        };

        #region Sounds
        _joinGameButton.clicked += () => PlaySound(SelectSound);
        _backButton.clicked += () => PlaySound(SelectSound);
        _quitButton.clicked += () => PlaySound(SelectSound);

        _hostGameButton.RegisterCallback<FocusEvent>((evt) => PlaySound(FocusSound));
        _joinGameButton.RegisterCallback<FocusEvent>((evt) => PlaySound(FocusSound));
        _backButton.RegisterCallback<FocusEvent>((evt) => PlaySound(FocusSound));
        _joinButton.RegisterCallback<FocusEvent>((evt) => PlaySound(FocusSound));
        _quitButton.RegisterCallback<FocusEvent>((evt) => PlaySound(FocusSound));
        #endregion

        _coinButton.Focus();
        StartCoroutine(VisibilityLoop());
        StartCoroutine(GameTitleOpacityHandler());
    }

    private IEnumerator VisibilityLoop()
    {
        bool t = false;
        StyleColor white = new StyleColor(Color.white);
        StyleColor black = new StyleColor(Color.black);
        while (true)
        {
            yield return new WaitForSeconds(.8f);
            t = !t;
            _coinButton.style.color = t ? white : black;
        }
    }

    private IEnumerator ShowMainMenu()
    {
        float delay = .4f;
        _hostGameButton.RemoveFromClassList("disabled");
        yield return new WaitForSeconds(delay);
        _joinGameButton.RemoveFromClassList("disabled");
        yield return new WaitForSeconds(delay);
        _quitButton.RemoveFromClassList("disabled");
        yield return new WaitForSeconds(delay);
        _hostGameButton.Focus();
    }

    private IEnumerator GameTitleOpacityHandler()
    {
        float delay = .8f;
        StyleColor c1 = new StyleColor(new Color(1, 1, 1, 1));
        StyleColor c2 = new StyleColor(new Color(1, 1, 1, .7f));
        StyleColor c3 = new StyleColor(new Color(1, 1, 1, .45f));
        while (true)
        {
            _gameTitle.style.color = c1;
            yield return new WaitForSeconds(delay);
            _gameTitle.style.color = c2;
            yield return new WaitForSeconds(delay);
            _gameTitle.style.color = c3;
            yield return new WaitForSeconds(delay);
            _gameTitle.style.color = c2;
            yield return new WaitForSeconds(delay);
            _gameTitle.style.color = c1;
            yield return new WaitForSeconds(delay);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void IPFieldFocusOut(ChangeEvent<string> evt)
    {
        if (!ValidateIPv4(evt.newValue)) return;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = evt.newValue;
    }

    private void QuitButtonClicked()
    {
        Application.Quit();
    }

    private void BackButtonClicked()
    {
        _baseContainer.RemoveFromClassList("disabled");
        _joinContainer.RemoveFromClassList("active");
        _joinContainer.AddToClassList("disabled");
    }

    private void JoinGameButtonClicked()
    {
        _baseContainer.AddToClassList("disabled");
        _joinContainer.AddToClassList("active");
        _joinContainer.RemoveFromClassList("disabled");
        _joinButton.Focus();
    }

    private void HostGameButtonClicked()
    {
        _playerInputManager.splitScreen = false;
        PlaySound(StartSound);
        NetworkManager.Singleton.StartHost();
        GameManager_v2.Instance.IsSessionHost = true;
        StopCoroutine(GameTitleOpacityHandler());
        GetComponent<UIDocument>().rootVisualElement.visible = false;
    }

    private void JoinButtonClicked()
    {
        _playerInputManager.splitScreen = true;
        PlaySound(StartSound);
        NetworkManager.Singleton.StartClient();
        GameManager_v2.Instance.IsSessionHost = true;
        StopCoroutine(GameTitleOpacityHandler());
        GetComponent<UIDocument>().rootVisualElement.visible = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        _hostGameButton.Focus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool ValidateIPv4(string ipString)
    {
        if (String.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }
}

