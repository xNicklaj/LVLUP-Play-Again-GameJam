using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class InGameUISelector : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset HostDocument;
    [SerializeField] private VisualTreeAsset ClientDocument;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            this.GetComponent<UIDocument>().visualTreeAsset = HostDocument;
        }
        else
        {
            this.GetComponent<UIDocument>().visualTreeAsset = ClientDocument;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
