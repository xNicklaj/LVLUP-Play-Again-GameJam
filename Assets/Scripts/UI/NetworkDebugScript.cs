using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NetworkDebugScript : MonoBehaviour
{
    private Button _hostButton;
    private Button _clientButton;

    private void Awake()
    {
        _hostButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("HostButton");
        _clientButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("ClientButton");

        _hostButton.RegisterCallback<ClickEvent>(ev => NetworkManager.Singleton.StartHost());
        _clientButton.RegisterCallback<ClickEvent>(ev => NetworkManager.Singleton.StartClient());
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
