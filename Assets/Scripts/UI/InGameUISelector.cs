using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class InGameUISelector : NetworkBehaviour
{
    [SerializeField] private VisualTreeAsset HostDocument;
    [SerializeField] private VisualTreeAsset ClientDocument;

    public override void OnNetworkSpawn()
    {
        NetworkManager.OnClientConnectedCallback += (ulong e) => { SelectUI(); };
    }

    void SelectUI()
    {
        if (!this.IsClient) return;
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
