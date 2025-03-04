using UnityEngine;
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
    }

    private void BackButtonClicked(ClickEvent evt)
    {
        _joinContainer.RemoveFromClassList(".active");
        _baseContainer.AddToClassList(".disabled");
    }

    private void JoinGameButtonClicked(ClickEvent evt)
    {
        _baseContainer.RemoveFromClassList(".active");
        _joinContainer.AddToClassList(".disabled");
    }

    private void HostGameButtonClicked(ClickEvent evt)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
