using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float reflectedSpeedMultiplier = 2.5f;
    [SerializeField] private float lifeTime = 5f;
    
    private Vector2 _direction = Vector2.left;
    private bool _isReflected = false;
    public bool IsReflected => _isReflected;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    
    void Update()
    {
        transform.Translate(_direction * (speed * Time.deltaTime), Space.World);
    }

    public void Reflect()
    {
        _isReflected = true;
        _direction = Vector2.right;
        speed *= reflectedSpeedMultiplier;
        
        transform.rotation = Quaternion.Euler(0, 180, 0);
        gameObject.layer = LayerMask.NameToLayer("ReflectedProjectile");
    }

    public void DestroyByBlock()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isReflected && collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (_isReflected && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyArcher>()?.Die();
            Destroy(gameObject);
        }
    }
}