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
    private Collider2D polygonCollider;
    private Animator _animator;
    private Vector3 _startPosition;
  
    private Vector3 sideOffset = new Vector3(0, -0.5f, 0);
    private Vector3 frontOffset;
    
    [SerializeField] private InstrumentNetworkController playerNetwork;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        spriteRenderer.color = shieldColor;
        frontOffset = new Vector3(shieldRange + 0.5f, 0, 0);

        polygonCollider = GetComponent<Collider2D>();
        if (polygonCollider)
        {
            polygonCollider.isTrigger = true;
        }
        _playerInput = GetComponentInParent<PlayerInput>();
        if (_playerInput)
        {
            _look = _playerInput.actions.FindAction("Direction");
        }

        _startPosition = transform.localPosition;

        UpdateShieldTransform();
    }

    private void Update()
    {
        UpdateShieldTransform();
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat("Horizontal", playerNetwork.rightStickAxis.Value.x);
        _animator.SetFloat("Vertical", playerNetwork.rightStickAxis.Value.y);
    }
    
    private void UpdateShieldTransform()
    {
        Vector2 dir = Vector2.zero;
        if (_look != null)
        {
            //dir = _look.ReadValue<Vector2>();
            dir = playerNetwork.rightStickAxis.Value;
        }

        if (dir.sqrMagnitude > 0.001f)
        {
            spriteRenderer.enabled = true;
            polygonCollider.enabled = true;
            
            float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            angleZ += shieldAngle;

            float rad = angleZ * Mathf.Deg2Rad;
            float posX = Mathf.Sin(rad) * shieldRange;
            float posY = -Mathf.Cos(rad) * shieldRange;

            transform.localPosition = new Vector3(posX, posY, _startPosition.z);
        }
        else
        {
            transform.localPosition = _startPosition;
            spriteRenderer.enabled = false;
            polygonCollider.enabled = false;
        }
        //transform.localEulerAngles = new Vector3(0, 0, angleZ);
        // transform.localScale = new Vector3(shieldWidth, shieldHeight, 1f);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Shield hit");
        if (!collision.gameObject.name.Contains("Bullet")) return;
        if (!collision.gameObject.GetComponent<Bullet>().isFromEnemy) return;
        collision.gameObject.GetComponent<Bullet>().damageMult.Value = 0;
    }
}