using UnityEngine;

public class TimingRing : MonoBehaviour
{
    [Header("Ring Settings")]
    public Transform ringSprite;
    public float startScale = 3f;
    public float perfectScale = 1f; 
    public float timeToReachPerfect = 1f;

    [Header("Colors")]
    public SpriteRenderer ringRenderer;
    public Color normalColor = Color.white;
    public Color perfectColor = Color.cyan;

    private float timer = 0f;

    void Start()
    {
        ringSprite.localScale = Vector3.one * startScale;
        ringRenderer.color = normalColor;
    }

    void Update()
    {
        if (GetComponent<Arrow>().IsReflected) 
        {
            ringSprite.gameObject.SetActive(false);
            return;
        }

        timer += Time.deltaTime;
        float progress = timer / timeToReachPerfect;

        float currentScale = Mathf.Lerp(startScale, perfectScale, progress);
        ringSprite.localScale = Vector3.one * currentScale;

        if (currentScale <= perfectScale + 0.3f)
        {
            ringRenderer.color = perfectColor;
        }
    }
}