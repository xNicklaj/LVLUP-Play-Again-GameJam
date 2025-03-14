using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D))]
public class TorchSystemSprite : MonoBehaviour
{
    [Range(0,100)][SerializeField] private float torchRange = 5f;
    [Range(5,90)][SerializeField] private float torchAngle = 45f;
    [SerializeField] private Color torchColor = Color.yellow;

    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;
    private PlayerInput _playerInput;
    private InputAction _look;
    private Vector3 _startPosition;
    
    [SerializeField] private InstrumentNetworkController playerNetwork;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;

        spriteRenderer.color = torchColor;
        _playerInput = GetComponentInParent<PlayerInput>();
        if (_playerInput)
        {
            _look = _playerInput.actions.FindAction("Direction");
        }
        
        _startPosition = transform.localPosition;

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
    
    private void UpdateTorchTransform()
    {
        
        Vector2 dir = Vector2.zero;
        if (_look != null)
        {
            //dir = _look.ReadValue<Vector2>();
            dir = playerNetwork.rightStickAxis.Value;
        }

        if (dir.sqrMagnitude > 0.001f)
        {
            float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0, 0, angleZ);
            
            // Caso A: 0..45 o 315..360
            if (angleZ <= 45f || angleZ >= 315f)
            {
                transform.localPosition = new Vector3(-0.02f, _startPosition.y, _startPosition.z);
            }
            // Caso B: 135..225
            else if (angleZ >= 135f && angleZ <= 225f)
            {
                transform.localPosition = new Vector3(0.03f, _startPosition.y, _startPosition.z);
            }
            // Caso C: tutti gli altri angoli
            else
            {
                transform.localPosition = new Vector3(-0.02f, _startPosition.y, _startPosition.z);
            }
        }        else
        {
            transform.localPosition = _startPosition;
        }
        
        
        
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
    
}
