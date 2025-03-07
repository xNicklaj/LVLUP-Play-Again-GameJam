using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class InGameUISelector : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset HostDocument;
    [SerializeField] private VisualTreeAsset ClientDocument;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong e) => { SelectUI(); };
    }

    void SelectUI()
    {
        if (!NetworkManager.Singleton.IsClient) return;
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            this.GetComponent<UIDocument>().visualTreeAsset = HostDocument;
        }
        else
        {
            this.GetComponent<UIDocument>().visualTreeAsset = ClientDocument;
        }
        GameManager_v2.Instance.OnUISelected.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
