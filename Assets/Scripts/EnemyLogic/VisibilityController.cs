using System;
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

    [SerializeField] private GameObject child;

    private void Awake()
    {
        SpriteRenderer rendererParent = GetComponent<SpriteRenderer>();
        SpriteRenderer rendererChild = null;
        try
        {
            rendererChild = child.GetComponent<SpriteRenderer>();
        }
        catch (Exception e)
        {
            child = transform.GetChild(0).gameObject;
        }
        finally
        {
            rendererChild = child.GetComponent<SpriteRenderer>();
        }

        if (rendererParent && rendererChild)
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                rendererChild.enabled = false;
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
            child.gameObject.layer = LayerMask.NameToLayer(litLayerName);

        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer(unlitLayerName);
            child.gameObject.gameObject.layer = LayerMask.NameToLayer(unlitLayerName);
        }
    }
}