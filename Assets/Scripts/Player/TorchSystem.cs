using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D))]
public class TorchSystemSprite : MonoBehaviour
{
    [Range(0,100)][SerializeField] private float torchRange = 5f;
    [Range(5,90)][SerializeField] private float torchAngle = 45f;
    [SerializeField] private Color torchColor = Color.yellow;

    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;

        spriteRenderer.color = torchColor;

        UpdateTorchTransform();
    }

    private void Update()
    {
        UpdateTorchTransform();
            //UpdateTorchCollider();
    }
    
    /// <summary>
    /// Change the torch color and transparency.
    /// <para>Note: Not tested yet</para>
    /// </summary>
    /// <param name="color"></param>
    public void SetTorchColor(Color color)
    {
        torchColor = color;
        //meshRenderer.material.color = new Color(color.r, color.g, color.b, color.a);
    }

    /// <summary>
    /// Change the torch range
    /// </summary>
    /// <param name="range"></param>
    public void SetTorchRange(float range)
    {
        torchRange = range;
        UpdateTorchTransform();
    }
    
    /// <summary>
    /// Change the torch angle
    /// </summary>
    /// <param name="angle"></param>
    public void SetTorchAngle(float angle)
    {
        torchAngle = angle;
        UpdateTorchTransform();
    }

    private void UpdateTorchMesh()
    {
        transform.localEulerAngles = new Vector3(0,0, 0);

        // Esempio di scaling:
        // Se la sprite è disegnata come un cono di 90° a grandezza 1x1
        // - X scale = torchAngle / 90f
        // - Y scale = torchRange / (grandezza disegnata)
        float angleScale = torchAngle / 90f;
        float rangeScale = torchRange / 3f; 
        transform.localScale = new Vector3(rangeScale, angleScale, 1);
    }
    
    private void UpdateTorchCollider()
    {

        Vector3[] vertices3D = new Vector3[10];
        Vector2[] colliderPoints = new Vector2[10];

        vertices3D[0] = Vector3.zero;
        float halfAngle = torchAngle * 0.5f;
        for (int i = 0; i < vertices3D.Length - 1; i++)
        {
            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / (vertices3D.Length - 2));
            float rad = Mathf.Deg2Rad * angle;
            float x = Mathf.Sin(rad) * torchRange;
            float y = Mathf.Cos(rad) * torchRange;
            vertices3D[i + 1] = new Vector3(y, -x, 0);
        }

        for (int i = 0; i < vertices3D.Length; i++)
            colliderPoints[i] = new Vector2(vertices3D[i].x, vertices3D[i].y);

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, colliderPoints);
    }
    private void UpdateTorchTransform()
    {
        // Esempio: se la sprite è disegnata come un cono di 90° con dimensione "base"
        // e vuoi un "range" in asse y e "angle" in asse x
        // regola la formula in base a come hai creato la sprite

        // Ruotiamo l'oggetto se vuoi un orientamento particolare
        // transform.localEulerAngles = new Vector3(0,0,0); // Se serve

        // Calcoliamo fattori di scala
        float angleScale = torchAngle / 90f;   // Se la sprite rappresenta 90° a full width = 1
        float rangeScale = torchRange / 5f;    // Se la sprite base copre un range di 5 unità a scale=1

        transform.localScale = new Vector3(rangeScale, angleScale, 1);
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
