using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostScript : MonoBehaviour
{
    public float TimeVisible = 1f;
    public AnimationCurve VisibilityCurve;

    private float _timer = 0f;
    private SpriteRenderer _spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        float alpha = 1 - VisibilityCurve.Evaluate(_timer % TimeVisible);
        Color c = new Color(1, 1, 1, alpha);
        if (alpha < 0.05)
            Destroy(this);
        _spriteRenderer.color = c;
    }
}
