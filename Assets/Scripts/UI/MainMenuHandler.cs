using System;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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

    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private PlaySceneSettings _playSceneSettings;

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

        _hostGameButton.clicked += HostGameButtonClicked;
        _joinGameButton.clicked += JoinGameButtonClicked;
        _backButton.clicked += BackButtonClicked;
        _joinButton.clicked += JoinButtonClicked;
        _quitButton.clicked += QuitButtonClicked;
        _ipField.RegisterCallback<ChangeEvent<string>>(IPFieldFocusOut);
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

    private void JoinButtonClicked()
    {
        _playerInputManager.splitScreen = true;
        NetworkManager.Singleton.StartClient();
        GetComponent<UIDocument>().rootVisualElement.visible = false;
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
        NetworkManager.Singleton.StartHost();
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

