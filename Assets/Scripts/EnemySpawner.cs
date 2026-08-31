using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Difficulty Curve")]
    [SerializeField] private float initialSpawnInterval = 3f;
    [SerializeField] private float minSpawnInterval = 0.8f;
    [SerializeField] private float intervalDecreaseRate = 0.1f;

    private float currentSpawnInterval;
    private bool isSpawning = true;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Not Set Spawn Points in Inspector!");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (isSpawning)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(currentSpawnInterval);

            // Increase the difficulty
            if (currentSpawnInterval > minSpawnInterval)
            {
                currentSpawnInterval -= intervalDecreaseRate;
                
                currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval); 
            }
        }
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedSpawnPoint = spawnPoints[randomIndex];

        GameObject newEnemy = Instantiate(enemyPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);

        // (Flip)
        // if (selectedSpawnPoint.position.x > 0)
        // {
        //     Vector3 scale = newEnemy.transform.localScale;
        //     scale.x = -Mathf.Abs(scale.x);
        //     newEnemy.transform.localScale = scale;
        // }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
}