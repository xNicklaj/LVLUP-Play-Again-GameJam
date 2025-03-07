using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class VisibilityController : MonoBehaviour
{
    [Header("TestVisibility")]
    [SerializeField] private TestVisibilitybyLayer TestVisibility; //TODO: to change

    [Header("Layer Names")]
    [SerializeField] private string litLayerName = "LitEnemy";
    [SerializeField] private string unlitLayerName = "UnlitEnemy";

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisibility();
    }

    private void Update()
    {
        UpdateVisibility();
    }
    
    private void UpdateVisibility()
    {
        // for King Player
        if (TestVisibility && TestVisibility.Visibility)
        {
            spriteRenderer.enabled = true;
            return;
        }

        // Altrimenti, controlliamo se il layer è "Lit"
        int currentLayer = gameObject.layer;
        int litLayer = LayerMask.NameToLayer(litLayerName);

        // Se l'oggetto è nel layer "Lit", abilitiamo lo sprite, altrimenti no
        spriteRenderer.enabled = (currentLayer == litLayer);
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

        UpdateVisibility();
    }
}