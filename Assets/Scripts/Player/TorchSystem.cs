using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class TorchSystem : MonoBehaviour
{
    [Range(0,100)][SerializeField] private float torchRange = 5f;
    [Range(5,90)][SerializeField] private float torchAngle = 45f;
    [SerializeField] private Color torchColor = Color.yellow;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh torchMesh;
    private PolygonCollider2D polygonCollider;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;

        torchMesh = new Mesh();
        meshFilter.mesh = torchMesh;
        
        meshRenderer.sortingOrder = 10; //TODO: see if used
        meshRenderer.material.color = new Color(torchColor.r, torchColor.g, torchColor.b, torchColor.a);
        
        UpdateTorchMesh();
    }

    private void Update()
    {
            UpdateTorchMesh();
            UpdateTorchCollider();
    }
    
    /// <summary>
    /// Change the torch color and transparency.
    /// <para>Note: Not tested yet</para>
    /// </summary>
    /// <param name="color"></param>
    public void SetTorchColor(Color color)
    {
        torchColor = color;
        meshRenderer.material.color = new Color(color.r, color.g, color.b, color.a);
    }

    /// <summary>
    /// Change the torch range
    /// </summary>
    /// <param name="range"></param>
    public void SetTorchRange(float range)
    {
        torchRange = range;
        UpdateTorchMesh();
    }
    
    /// <summary>
    /// Change the torch angle
    /// </summary>
    /// <param name="angle"></param>
    public void SetTorchAngle(float angle)
    {
        torchAngle = angle;
        UpdateTorchMesh();
    }

    private void UpdateTorchMesh()
    {
        Vector3[] vertices = new Vector3[10];
        int[] triangles = new int[(vertices.Length - 2) * 3];

        vertices[0] = Vector3.zero;

        float halfAngle = torchAngle * 0.5f;
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / (vertices.Length - 2));
            float rad = Mathf.Deg2Rad * angle;

            // 90 degree rotation
            float x = Mathf.Sin(rad) * torchRange;
            float y = Mathf.Cos(rad) * torchRange;
            vertices[i + 1] = new Vector3(y, -x, 0);
        }

        for (int i = 0; i < vertices.Length - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        torchMesh.Clear();
        torchMesh.vertices = vertices;
        torchMesh.triangles = triangles;
        torchMesh.RecalculateNormals();
    }
    
    private void UpdateTorchCollider()
    {
        Vector2[] colliderPoints = new Vector2[10];

        Vector3[] meshVertices = torchMesh.vertices;
        for (int i = 0; i < meshVertices.Length; i++)
        {
            colliderPoints[i] = new Vector2(meshVertices[i].x, meshVertices[i].y);
        }

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, colliderPoints);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Oggetto entrato nel cono di luce: " + other.name);
        var obj = other.GetComponent<VisibilityController>();
        if (obj)
        {
            obj.SetLayer(true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log("Oggetto uscito dal cono di luce: " + other.name);
        var obj = other.GetComponent<VisibilityController>();
        if (obj)
        {
            obj.SetLayer(false);
        }
    }
    
}
