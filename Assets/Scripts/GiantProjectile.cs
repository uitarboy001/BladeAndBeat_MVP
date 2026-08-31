using UnityEngine;

public class GiantProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float superReflectSpeed = 30f;
    [SerializeField] private float lifeTime = 5f;
    
    private Vector2 _direction = Vector2.left;
    private bool _isReflected = false;
    private Vector3 originalScale;

    public bool IsReflected => _isReflected;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        
        originalScale = transform.localScale;
    }

    void Update()
    {
        transform.Translate(_direction * (speed * Time.deltaTime), Space.World);
    }

    public void PushBackVisual(int currentMash, int requiredMash)
    {
        float progress = (float)currentMash / requiredMash;
        transform.localScale = originalScale * (1f - (progress * 0.3f)); 
        
        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
    }

    public void ReflectSuperSpeed()
    {
        _isReflected = true;
        _direction = Vector2.right;
        speed = superReflectSpeed;
        
        transform.localScale = originalScale * 1.5f;
        GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
        gameObject.layer = LayerMask.NameToLayer("ReflectedProjectile");
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isReflected && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyArcher>()?.Die();
        }
    }
}