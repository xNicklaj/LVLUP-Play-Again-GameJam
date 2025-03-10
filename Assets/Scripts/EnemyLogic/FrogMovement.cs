using System.Collections;
using TMPro.EditorUtilities;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class FrogMovement : MonoBehaviour
{

    public float MoveSpeed = 3f;
    public short MaxHops = 4;
    public AnimationCurve ShrinkCurve;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private short _jumpCount = 0;
    [SerializeField] private bool _isShrinking = false;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private Animator _animator;

    private float _shrinkTimer = 0f;
    private Vector2 _direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _direction = new Vector2(Random.Range(0f, 1f) > .5f ? 1 : -1, 0);
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_isJumping) _rb.MovePosition((Vector2)(transform.position) + (MoveSpeed * _direction * Time.deltaTime));
        if (_isShrinking)
        {
            _shrinkTimer += Time.deltaTime;
            float shrinkFactor = ShrinkCurve.Evaluate(_shrinkTimer);
            _sprite.transform.localScale = new Vector3(1 - shrinkFactor, 1 - shrinkFactor, 1);
            if (_shrinkTimer >= 1)
            {
                Destroy(gameObject);
            }
        }
    }

    public void StartJump()
    {
        if (_jumpCount >= MaxHops) return;
        _isJumping = true;   
    }

    public void StopJump()
    {
        _isJumping = false;
        _jumpCount += 1;
        if (_jumpCount == MaxHops)
        {
            _animator.StopPlayback();
            _isShrinking = true;
        }
            
    }

    public void DisableGravity()
    {
        _rb.gravityScale = 0;
        _rb.linearVelocityY = 0;
    }
}
