using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class VisibilityController : MonoBehaviour
{
    [Header("TestVisibility")]
    [SerializeField] private TestVisibilitybyLayer TestVisibility; //TODO: to change

    [Header("Layer Names")]
    [SerializeField] private string litLayerName = "LitEnemy";
    [SerializeField] private string unlitLayerName = "UnlitEnemy";


    private void Awake()
    {
        SpriteRenderer rendererParent = GetComponentInParent<SpriteRenderer>();
        SpriteRenderer rendererChild = GetComponentInChildren<SpriteRenderer>();
        NetworkBehaviour network = GetComponent<NetworkBehaviour>();
        
        if (rendererParent && rendererChild && network)
        {
            if (!network.IsHost)
            {
                rendererParent.enabled = false;
            }
        }
    }


    /// <summary>
    /// Change the layer of the object
    /// </summary>
    /// <param name="toLit">Se true, imposta il layer come Lit, altrimenti Unlit</param>
    public void SetLayer(bool toLit)
    {
        if (toLit)
        {
            gameObject.layer = LayerMask.NameToLayer(litLayerName);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer(unlitLayerName);
        }
    }
}