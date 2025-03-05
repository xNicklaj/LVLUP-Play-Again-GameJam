using System;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuHandler : MonoBehaviour
{
    private VisualElement _baseContainer;
    private VisualElement _joinContainer;
    private Button _hostGameButton;
    private Button _joinGameButton;
    private Button _joinButton;
    private Button _backButton;
    private Button _quitButton;
    private TextField _ipField;

    [SerializeField] private Scene playScene;

    void Awake()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _baseContainer = root.Q<VisualElement>("BaseContainer");
        _joinContainer = root.Q<VisualElement>("JoinContainer");
        
        _hostGameButton = root.Q<Button>("HostGameButton");
        _joinGameButton = root.Q<Button>("JoinGameButton");
        _joinButton = root.Q<Button>("JoinButton");
        _backButton = root.Q<Button>("BackButton");
        _quitButton = root.Q<Button>("QuitButton");
        _ipField = root.Q<TextField>("IPField");

        _hostGameButton.RegisterCallback<ClickEvent>(HostGameButtonClicked);
        _joinGameButton.RegisterCallback<ClickEvent>(JoinGameButtonClicked);
        _backButton.RegisterCallback<ClickEvent>(BackButtonClicked);
        _joinButton.RegisterCallback<ClickEvent>(JoinButtonClicked);
        _quitButton.RegisterCallback<ClickEvent>(QuitButtonClicked);
        _ipField.RegisterCallback<ChangeEvent<string>>(IPFieldFocusOut);

    }

    private void IPFieldFocusOut(ChangeEvent<string> evt)
    {
        if (!ValidateIPv4(evt.newValue)) return;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = evt.newValue;
    }

    private void QuitButtonClicked(ClickEvent evt)
    {
        Application.Quit();
    }

    private void JoinButtonClicked(ClickEvent evt)
    {
        NetworkManager.Singleton.StartClient();
    }

    private void BackButtonClicked(ClickEvent evt)
    {
        _baseContainer.RemoveFromClassList("disabled");
        _joinContainer.RemoveFromClassList("active");
        _joinContainer.AddToClassList("disabled");
    }

    private void JoinGameButtonClicked(ClickEvent evt)
    {
        _baseContainer.AddToClassList("disabled");
        _joinContainer.AddToClassList("active");
        _joinContainer.RemoveFromClassList("disabled");
    }

    private void HostGameButtonClicked(ClickEvent evt)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(playScene.name, LoadSceneMode.Single);
        NetworkManager.Singleton.StartServer();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
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

