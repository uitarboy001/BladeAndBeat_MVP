using System.Collections;
using UnityEngine;

public class EnemyArcher : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite shootSprite;
    
    [SerializeField] private SpriteRenderer enemyRenderer;
    [SerializeField] private Color warningColor = Color.red;
    
    [Header("Shoot Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float prepareTime = 1.5f;
    
    [SerializeField] private GameObject giantProjectilePrefab;
    [SerializeField] private float heavyAttackChance = 0.2f;
    
    private Color originalColor;

    private bool isDead = false;

    void Start()
    {
        if (enemyRenderer != null)
            originalColor = enemyRenderer.color;

        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(prepareTime - 0.5f);

            bool isHeavy = Random.value <= heavyAttackChance;

            if (isHeavy)
            {
                for (int i = 0; i < 5; i++)
                {
                    enemyRenderer.color = Color.yellow;
                    yield return new WaitForSeconds(0.05f);
                    enemyRenderer.color = originalColor;
                    yield return new WaitForSeconds(0.05f);
                }
            
                enemyRenderer.sprite = shootSprite;
                Instantiate(giantProjectilePrefab, shootPoint.position, Quaternion.identity);
            }
            else
            {
                enemyRenderer.color = warningColor;
                yield return new WaitForSeconds(0.3f);
                enemyRenderer.color = originalColor;
            
                enemyRenderer.sprite = shootSprite;
                Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        
        StopAllCoroutines();
        Debug.Log("Enemy Killed");

        Destroy(gameObject);
    }
}