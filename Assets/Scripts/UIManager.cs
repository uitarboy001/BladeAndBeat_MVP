using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI comboText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    
    [Header("Combo Juice Settings")]
    public Color comboFlashColor = Color.yellow;
    public float comboPopScale = 1.5f;
    
    private Color originalComboColor;
    private Vector3 originalComboScale;
    private Coroutine comboCoroutine;

    void Start()
    {
        if (comboText != null) comboText.text = "";
        
        originalComboColor = comboText.color;
        originalComboScale = comboText.transform.localScale;
    }
    
    public void UpdateHP(int currentHealth)
    {
        hpText.text = currentHealth.ToString();
    }

    public void UpdateCombo(int currentCombo)
    {
        if (currentCombo > 0)
        {
            comboText.text = "Combo: " + currentCombo.ToString();
            
            if (comboCoroutine != null) StopCoroutine(comboCoroutine);
            comboCoroutine = StartCoroutine(ComboPulseRoutine());
        }
        else
        {
            comboText.text = ""; 
        }
    }
    
    private IEnumerator ComboPulseRoutine()
    {
        comboText.color = comboFlashColor;
        comboText.transform.localScale = originalComboScale * comboPopScale;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float progress = elapsed / duration;

            comboText.transform.localScale = Vector3.Lerp(originalComboScale * comboPopScale, originalComboScale, progress);
            comboText.color = Color.Lerp(comboFlashColor, originalComboColor, progress);

            yield return null;
        }

        comboText.transform.localScale = originalComboScale;
        comboText.color = originalComboColor;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}