using UnityEngine;

public class PlayerParryController : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private float attackAnimDuration = 0.15f;
    
    private Coroutine attackAnimCoroutine;
    
    [Header("Parry Settings")]
    [SerializeField] private float parryCooldown = 0.2f;
    [SerializeField] private Transform parryPoint;
    [SerializeField] private float perfectRadius = 1.2f;
    [SerializeField] private float goodRadius = 2.2f;
    [SerializeField] private LayerMask projectileLayer;
    [SerializeField] private LayerMask giantProjectileLayer;
    
    [SerializeField] private int requiredMashes = 8;
    [SerializeField] private float clashTimeLimit = 2.5f;

    private bool isClashing = false;
    private int currentMashCount = 0;
    private float clashTimer = 0f;
    private float _lastParryTime;
    
    private GiantProjectile currentGiantProjectile;

    void Update()
    {
        if (isClashing)
        {
            HandleClashPhase();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            PlayAttackAnimation();
            TryParry();
        }
    }

    // Visuals
    private void PlayAttackAnimation()
    {
        if (attackAnimCoroutine != null)
        {
            StopCoroutine(attackAnimCoroutine);
        }
        attackAnimCoroutine = StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        if (playerRenderer != null) playerRenderer.sprite = attackSprite;
        
        yield return new WaitForSecondsRealtime(attackAnimDuration);
        
        if (playerRenderer != null) playerRenderer.sprite = idleSprite;
    }
    
    
    private void TryParry()
    {
        if (Time.time < _lastParryTime + parryCooldown) return;
        _lastParryTime = Time.time;

        // Giant bullet
        Collider2D giantCollider = Physics2D.OverlapCircle(parryPoint.position, goodRadius * 2f, giantProjectileLayer);
        if (giantCollider != null)
        {
            currentGiantProjectile = giantCollider.GetComponent<GiantProjectile>();
            if (currentGiantProjectile != null && !currentGiantProjectile.IsReflected)
            {
                StartClash();
                return;
            }
        }
        
        // Normal bullet
        Collider2D arrowCollider = Physics2D.OverlapCircle(parryPoint.position, goodRadius, projectileLayer);

        if (arrowCollider != null)
        {
            float distance = Vector2.Distance(parryPoint.position, arrowCollider.transform.position);
            Arrow projectile = arrowCollider.GetComponent<Arrow>();
            
            if (projectile != null && !projectile.IsReflected)
            {
                if (distance <= perfectRadius)
                {
                    // Perfect Parry
                    projectile.Reflect();
                    
                    int currentCombo = GetComponent<PlayerHealth>().GetCurrentCombo();
                    float comboPitch = 1f + Mathf.Min(currentCombo * 0.02f, 0.5f);
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.parryClip, 1f, comboPitch);
                    
                    GameJuice.Instance.TriggerHitstop(0.08f, 0.02f);
                    GameJuice.Instance.ShakeCamera(0.15f, 0.3f);
                    GameJuice.Instance.FlashScreen();
                    
                    GetComponent<PlayerHealth>().AddCombo(1);
                }
                else
                {
                    // Good Parry
                    projectile.DestroyByBlock();
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.parryClip, 0.7f, 0.8f);
                    GameJuice.Instance.ShakeCamera(0.1f, 0.1f);
                }
            }
        }
        else
        {
            // Parry (Miss timing)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.missClip);
        }
    }
    
    // CLASH
    private void StartClash()
    {
        isClashing = true;
        currentMashCount = 0;
        clashTimer = clashTimeLimit;

        if (attackAnimCoroutine != null) StopCoroutine(attackAnimCoroutine);
        if (playerRenderer != null) playerRenderer.sprite = attackSprite;
        
        Time.timeScale = 0f; 
        
        GameJuice.Instance.ToggleBlueFilter(true);
        GameJuice.Instance.ShakeCameraUnscaled(clashTimeLimit, 0.1f);
    }

    private void HandleClashPhase()
    {
        clashTimer -= Time.unscaledDeltaTime;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            currentMashCount++;
            
            float hitPitch = 1f + ((float)currentMashCount / requiredMashes) * 0.8f;
            AudioManager.Instance.PlayClashHit(hitPitch);
            
            GameJuice.Instance.ShakeCameraUnscaled(0.1f, 0.2f);
            
            currentGiantProjectile.PushBackVisual(currentMashCount, requiredMashes);

            if (currentMashCount >= requiredMashes)
            {
                WinClash();
            }
        }

        if (clashTimer <= 0f && isClashing)
        {
            LoseClash();
        }
    }

    private void WinClash()
    {
        isClashing = false;
        Time.timeScale = 1f;
        
        if (playerRenderer != null) playerRenderer.sprite = idleSprite;
        
        GameJuice.Instance.ToggleBlueFilter(false);
        GameJuice.Instance.FlashScreen();
        GameJuice.Instance.ShakeCamera(0.3f, 0.5f);

        currentGiantProjectile.ReflectSuperSpeed();
        GetComponent<PlayerHealth>().AddCombo(5);
        
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clashWinClip);
    }

    private void LoseClash()
    {
        isClashing = false;
        Time.timeScale = 1f;
        
        if (playerRenderer != null) playerRenderer.sprite = idleSprite;
        
        GameJuice.Instance.ToggleBlueFilter(false);
        GetComponent<PlayerHealth>().TakeDamage(2);
        currentGiantProjectile.DestroyProjectile();
    }


    private System.Collections.IEnumerator HitstopRoutine(float duration)
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (parryPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(parryPoint.position, perfectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(parryPoint.position, goodRadius);
    }
}