using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Combo System")]
    private int currentCombo = 0;
    public int GetCurrentCombo() { return currentCombo; }

    [Header("Events")]
    public UnityEvent onTakeDamage;
    public UnityEvent onDie;
    public UnityEvent<int> onComboChanged;
    public UnityEvent<int> onHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateCombo(0);
        
        onHealthChanged.Invoke(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        
        onHealthChanged?.Invoke(currentHealth);
        //Debug.Log("Player took damage! HP left: " + currentHealth);
        
        UpdateCombo(0);
        onTakeDamage?.Invoke();
        
        GameJuice.Instance.TriggerHitstop(0.1f, 0f);
        GameJuice.Instance.ShakeCamera(0.2f, 0.4f);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AddCombo(int amount = 1)
    {
        currentCombo += amount;
        UpdateCombo(currentCombo);
    }

    private void UpdateCombo(int newCombo)
    {
        currentCombo = newCombo;
        onComboChanged?.Invoke(currentCombo);
    }

    private void Die()
    {
        //Debug.Log("Player Dead! Game Over.");
        onDie?.Invoke();
    }
}