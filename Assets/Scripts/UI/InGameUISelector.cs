using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class InGameUISelector : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset HostDocument;
    [SerializeField] private VisualTreeAsset P2ClientDocument;
    [SerializeField] private VisualTreeAsset P3ClientDocument;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= (ulong e) => { SelectUI(); };
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong e) => { SelectUI(); };
    }

    void SelectUI()
    {
        if (this == null || gameObject == null)
        {
            Debug.LogWarning("SelectUI was destroyed but OnClientConnection was still called. Ignoring.");
            return;
        }

        if (!NetworkManager.Singleton.IsClient) return;

        switch(NetworkManager.Singleton.LocalClientId)
        {
            case 0:
                this.GetComponent<UIDocument>().visualTreeAsset = HostDocument;
                break;
            case 1:
                this.GetComponent<UIDocument>().visualTreeAsset = P2ClientDocument;
                break;
            case 2:
                this.GetComponent<UIDocument>().visualTreeAsset = P3ClientDocument;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
