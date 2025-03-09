using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class ShieldSystem : MonoBehaviour
{
    [SerializeField] private float shieldRange = 2f;
    [SerializeField] private float shieldWidth = 3f;
    [SerializeField] private float shieldHeight = 1.5f;
    [SerializeField] private float shieldAngle = 90f;
    [SerializeField] private Color shieldColor = Color.green;


    /*private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh shieldMesh;*/
    private bool isFront = false;
    
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;
    
    private Vector3 sideOffset = new Vector3(0, -0.5f, 0);
    private Vector3 frontOffset;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = shieldColor;
        frontOffset = new Vector3(shieldRange + 0.5f, 0, 0);

        polygonCollider = GetComponent<PolygonCollider2D>();
        if (polygonCollider)
            polygonCollider.isTrigger = true;

        spriteRenderer.enabled = false;
        if (polygonCollider) polygonCollider.enabled = false;
        UpdateShieldMesh();
    }

    private void Update()
    {
        UpdateShieldMesh();
    }
    
    public void OnClassAction(InputValue value)
    {
        bool isPressed = value.isPressed;

        if (isPressed)
        {
            // enable shield
            spriteRenderer.enabled = true;
            if (polygonCollider) polygonCollider.enabled = true;
        }
        else
        {
            // disable shield
            spriteRenderer.enabled = false;
            if (polygonCollider) polygonCollider.enabled = false;
        }
        
    }
    
    /// <summary>
    /// Change the shield distance from the player
    /// </summary>
    /// <param name="range"></param>
    public void SetShieldRange(float range)
    {
        shieldRange = range;
        UpdateShieldMesh();
    }
    
    /// <summary>
    /// Change the shield size
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetShieldSize(float width, float height)
    {
        shieldWidth = width;
        shieldHeight = height;
        UpdateShieldMesh();
    }
    
    /// <summary>
    /// Change the shield rotation
    /// </summary>
    /// <param name="angle"></param>
    public void SetShieldAngle(float angle)
    {
        shieldAngle = angle;
        UpdateShieldMesh();
    }

    private void UpdateShieldMesh()
    {
        frontOffset = new Vector3(shieldRange + 0.5f, 0, 0);
        //Vector3 offset = isFront ? frontOffset : sideOffset;
        
        transform.localPosition = frontOffset; //changed from offset
        transform.localEulerAngles = new Vector3(0, 0, shieldAngle);
        transform.localScale = new Vector3(shieldWidth, shieldHeight, 1f);
        
    }

}