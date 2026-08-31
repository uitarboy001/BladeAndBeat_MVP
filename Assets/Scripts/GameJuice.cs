using System.Collections;
using UnityEngine;

public class GameJuice : MonoBehaviour
{
    public static GameJuice Instance { get; private set; }

    [Header("Camera Shake Settings")]
    private Vector3 originalCamPos;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;

    [Header("Screen Flash")]
    public SpriteRenderer flashPanel;
    
    [Header("Clash Visuals")]
    public SpriteRenderer blueFilterPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        originalCamPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalCamPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.unscaledDeltaTime;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalCamPos;
        }
    }

    // Shake Camera
    public void ShakeCamera(float duration = 0.1f, float magnitude = 0.2f)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    // Hitstop
    public void TriggerHitstop(float duration = 0.08f, float timeScale = 0.05f)
    {
        StartCoroutine(HitstopRoutine(duration, timeScale));
    }

    private IEnumerator HitstopRoutine(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
    }

    // Flash Screen (Perfect Parry)
    public void FlashScreen()
    {
        if (flashPanel != null) StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashPanel.color = new Color(1, 1, 1, 0.4f);
        float fade = 0.4f;
        while (fade > 0)
        {
            fade -= Time.unscaledDeltaTime * 2f;
            flashPanel.color = new Color(1, 1, 1, fade);
            yield return null;
        }
    }
    
    public void ToggleBlueFilter(bool isActive)
    {
        if (blueFilterPanel != null)
        {
            blueFilterPanel.gameObject.SetActive(isActive);
        }
    }

    // Clash Mode
    public void ShakeCameraUnscaled(float duration, float magnitude)
    {
        StartCoroutine(ShakeUnscaledRoutine(duration, magnitude));
    }

    private System.Collections.IEnumerator ShakeUnscaledRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = originalCamPos + Random.insideUnitSphere * magnitude;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localPosition = originalCamPos;
    }
}