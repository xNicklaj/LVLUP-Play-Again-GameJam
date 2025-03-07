using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LostUIScript : MonoBehaviour
{
    private VisualElement _root;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _root.visible = false;    
        _root = GetComponent<UIDocument>().rootVisualElement;
        GameManager_v2.Instance.OnGameLost.AddListener(() =>
        {
            _root.visible = true;
        });
    }
    
    
    
    
}
