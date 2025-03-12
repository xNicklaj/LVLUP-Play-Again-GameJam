using System;
using System.Collections.Generic;
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
    private bool isFront = false; //todo: not used?
    private PlayerInput _playerInput;
    private InputAction _look;   
    private SpriteRenderer spriteRenderer;
    private Collider2D polygonCollider;
    private Animator _animator;
    private Vector3 _startPosition;
  
    private Vector3 sideOffset = new Vector3(0, -0.5f, 0);
    private Vector3 frontOffset;
    
    [Header("Extra Shields")]
    public GameObject shieldPrefab;
    private List<GameObject> extraShields = new List<GameObject>();
    private bool moreShield = false;
    
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

    private void Start()
    {
        this.playerNetwork.UseMultipleShields.AddListener(UseMultipleShields);
    }

    private void UseMultipleShields(bool arg0)
    {
        Debug.Log(arg0);
        if (arg0)
            SpawnOtherShields();
        else
            DespawnOtherShields();
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

            if (moreShield)
            {
                UpdateOtherShield(transform.localPosition);
            }
        }
        else
        {
            transform.localPosition = _startPosition;
            spriteRenderer.enabled = false;
            polygonCollider.enabled = false;

            if (moreShield)
            {
                foreach (GameObject shield in extraShields)
                {
                    shield.transform.localPosition = _startPosition;
                    shield.GetComponent<Renderer>().enabled = false;
                    shield.GetComponent<Collider2D>().enabled = false;
                }
            }
        }
        //transform.localEulerAngles = new Vector3(0, 0, angleZ);
        // transform.localScale = new Vector3(shieldWidth, shieldHeight, 1f);
    }

    public void SpawnOtherShields()
    {
        if (moreShield) return;
        Transform parent = transform.parent;

        Vector3 pos = new Vector3(-parent.position.x, parent.position.y, parent.position.z);
        GameObject newShieldObj = Instantiate(shieldPrefab, pos, transform.rotation, parent);
        extraShields.Add(newShieldObj);
        pos = new Vector3(-parent.position.x, -parent.position.y, parent.position.z);
        newShieldObj = Instantiate(shieldPrefab, pos, transform.rotation, parent);
        extraShields.Add(newShieldObj);
        pos = new Vector3(parent.position.x, -parent.position.y, parent.position.z);
        newShieldObj = Instantiate(shieldPrefab, pos, transform.rotation, parent);
        extraShields.Add(newShieldObj);
        
        moreShield = true;
    }

    public void DespawnOtherShields()
    {
        if (!moreShield) return;
        foreach (GameObject shield in extraShields)
        {
            Destroy(shield);
        }
        extraShields = new List<GameObject>();
        moreShield = false;
    }
    private void UpdateOtherShield(Vector3 mainPosition)
    {
        float dist = mainPosition.magnitude;
        float angleRad = Mathf.Atan2(mainPosition.y, mainPosition.x);
        
        foreach (GameObject shield in extraShields)
        {
            shield.GetComponent<Renderer>().enabled = true;
            shield.GetComponent<Collider2D>().enabled = true;
            
            float offsetRad = 90f * Mathf.Deg2Rad;
            angleRad += offsetRad;

            float newX = dist * Mathf.Cos(angleRad);
            float newY = dist * Mathf.Sin(angleRad);
            
            shield.transform.localPosition = new Vector3(newX,newY,mainPosition.z);
            
            Animator anim = shield.GetComponent<Animator>();
            anim.SetFloat("Horizontal", newX);
            anim.SetFloat("Vertical", newY);
        }
        
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

    private void OnDestroy()
    {
        this.playerNetwork.UseMultipleShields.RemoveListener(UseMultipleShields);
    }
}