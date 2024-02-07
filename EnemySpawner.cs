using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Префаб врага
    public string playerTag = "Player"; // Тег игрока
    public float spawnRadius1 = 5f; // Радиус для проверки наличия врага
    public float spawnRadius2 = 10f; // Радиус для проверки наличия игрока
    public float spawnDelay = 3f; // Задержка перед спавном следующего врага

    private void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (!IsPlayerNearby() && !IsEnemyNearby())
            {
                SpawnEnemy();
            }
        }
    }

    bool IsPlayerNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, spawnRadius2);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag(playerTag))
            {
                return true;
            }
        }
        return false;
    }

    bool IsEnemyNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, spawnRadius1);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        // Рисуем границы радиусов для отображения в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius1);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius2);
    }
}
