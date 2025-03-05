using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ShieldSystem : MonoBehaviour
{
    [SerializeField] private float shieldRange = 2f;  // Distanza dello scudo dal giocatore
    [SerializeField] private float shieldWidth = 3f;  // Larghezza dello scudo
    [SerializeField] private float shieldHeight = 1.5f; // Altezza dello scudo
    [SerializeField] private float shieldAngle = 0f; // Angolazione dello scudo
    [SerializeField] private Color shieldColor = Color.green;


    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh shieldMesh;
    private bool isFront = false;
    
    private Vector3 sideOffset = new Vector3(0, -0.5f, 0);
    private Vector3 frontOffset;
    
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        shieldMesh = new Mesh();
        meshFilter.mesh = shieldMesh;
        
        meshRenderer.sortingOrder = 10; //TODO: see if used
        meshRenderer.material.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, shieldColor.a);
        
        UpdateShieldMesh();
    }

    private void Update()
    {
        UpdateShieldMesh();
    }
    
    public void OnClassAction(InputValue value)
    {
        ToggleShieldPosition();
    }
    
    private void ToggleShieldPosition()
    {
        isFront = !isFront; // Cambia posizione tra lato e fronte

        if (isFront)
        {
            shieldAngle += 90f; // Ruota di 90°
        }
        else
        {
            shieldAngle -= 90f; // Torna nella posizione originale
        }
        UpdateShieldMesh();
    }
    
    public void SetShieldRange(float range)
    {
        shieldRange = range;
        UpdateShieldMesh();
    }

    public void SetShieldSize(float width, float height)
    {
        shieldWidth = width;
        shieldHeight = height;
        UpdateShieldMesh();
    }

    public void SetShieldAngle(float angle)
    {
        shieldAngle = angle;
        UpdateShieldMesh();
    }

    private void UpdateShieldMesh()
    {
        frontOffset = new Vector3(shieldRange + 0.5f, 0, 0);
        Vector3 chosenOffset = isFront ? frontOffset : sideOffset;

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6] { 0, 2, 1, 2, 0, 3 };
        
        float rad = Mathf.Deg2Rad * shieldAngle;
        //Vector3 shieldOffset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0)*shieldRange;
        
        vertices[0] = chosenOffset + RotatePoint(new Vector3(-shieldWidth / 2, -shieldHeight / 2, 0), rad);
        vertices[1] = chosenOffset + RotatePoint(new Vector3(shieldWidth / 2, -shieldHeight / 2, 0), rad);
        vertices[2] = chosenOffset + RotatePoint(new Vector3(shieldWidth / 2, shieldHeight / 2, 0), rad);
        vertices[3] = chosenOffset + RotatePoint(new Vector3(-shieldWidth / 2, shieldHeight / 2, 0), rad);

        shieldMesh.Clear();
        shieldMesh.vertices = vertices;
        shieldMesh.triangles = triangles;
        shieldMesh.RecalculateNormals();
    }
    private Vector3 RotatePoint(Vector3 point, float angleRad)
    {
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        return new Vector3(
            point.x * cos - point.y * sin,
            point.x * sin + point.y * cos,
            0
        );
    }
}