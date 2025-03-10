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
    private PlayerInput _playerInput;
    private InputAction _look;   
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;
    private Vector3 _startPosition;
  
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
        // Recuperiamo l'azione "Direction" dal PlayerInput (nel parent, ad es.)
        _playerInput = GetComponentInParent<PlayerInput>();
        if (_playerInput)
        {
            _look = _playerInput.actions.FindAction("Direction");
        }

        // Memorizziamo la posizione iniziale
        _startPosition = transform.localPosition;

        // All'avvio, possiamo disabilitare lo scudo se vogliamo
        // spriteRenderer.enabled = false;
        // if (polygonCollider) polygonCollider.enabled = false;

        UpdateShieldTransform();
        //spriteRenderer.enabled = false;
        //if (polygonCollider) polygonCollider.enabled = false;
        //UpdateShieldMesh();
        Debug.Log("AWAKE");
    }

    private void Update()
    {
        UpdateShieldTransform();
    }

    // ===========================
    // Metodo Principale di Update
    // ===========================
    private void UpdateShieldTransform()
    {
        // 1) Leggiamo la direzione dal PlayerInput
        Vector2 dir = Vector2.zero;
        if (_look != null)
        {
            dir = _look.ReadValue<Vector2>();
        }

        // 2) Se il vettore è != (0,0), calcoliamo l'angolo
        if (dir.sqrMagnitude > 0.001f)
        {
            float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            // Se vuoi aggiungere un offset di angolo rispetto a direzione:
            angleZ += shieldAngle; // Esempio: ruotare di +90° se vuoi scudo davanti al giocatore

            // 3) Posizioniamo lo scudo a shieldRange di distanza, nella direzione "dir"
            float rad = angleZ * Mathf.Deg2Rad;
            float posX = Mathf.Sin(rad) * shieldRange;
            float posY = -Mathf.Cos(rad) * shieldRange;

            // Aggiorniamo la posizione e rotazione locali
            transform.localPosition = new Vector3(posX, posY, _startPosition.z);
            //transform.localEulerAngles = new Vector3(0, 0, angleZ);
        }
        else
        {
            // Se non c'è input di direzione, rimettiamo lo scudo in _startPosition
            // o dove preferisci
            transform.localPosition = _startPosition;
            //transform.localEulerAngles = new Vector3(0, 0, shieldAngle);
        }

        // NB: al momento ignoriamo shieldWidth e shieldHeight (nessuna scala)
        // Se un domani vuoi scalare lo sprite:
        // transform.localScale = new Vector3(shieldWidth, shieldHeight, 1f);
    }

    // =====================
    // Gestione di ClassAction
    // =====================
    public void OnClassAction(InputValue value)
    {
        bool isPressed = value.isPressed;

        if (isPressed)
        {
            Debug.Log("ciao1");
            // enable shield
            spriteRenderer.enabled = true;
            if (polygonCollider) polygonCollider.enabled = true;
        }
        else
        {
            Debug.Log("ciao2");
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
        UpdateShieldTransform();
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
        UpdateShieldTransform();
    }
    
    /// <summary>
    /// Change the shield rotation
    /// </summary>
    /// <param name="angle"></param>
    public void SetShieldAngle(float angle)
    {
        shieldAngle = angle;
        UpdateShieldTransform();
    }

    private void UpdateShieldMesh()
    {
        frontOffset = new Vector3(shieldRange + 0.5f, 0, 0);
        //Vector3 offset = isFront ? frontOffset : sideOffset;
        
        transform.localPosition = frontOffset; //changed from offset
        transform.localEulerAngles = new Vector3(0, 0, shieldAngle);
        //transform.localScale = new Vector3(shieldWidth, shieldHeight, 1f);
        
    }

}