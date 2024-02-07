using UnityEngine;
using UnityEngine.AI;

public class ZombieHorde : MonoBehaviour
{
    public string leaderTag = "Leader"; // Тег лидера
    public float leaderDetectionRange = 10f; // Радиус обнаружения лидера
    public float playerDetectionRange = 5f; // Радиус обнаружения игрока

    private Transform leader; // Лидер
    private Transform player; // Игрок

    private void Start()
    {
        // Находим лидера по тегу
        GameObject leaderObject = GameObject.FindGameObjectWithTag(leaderTag);
        if (leaderObject != null)
        {
            leader = leaderObject.transform;
        }
        else
        {
            Debug.LogError("Leader not found!");
        }

        // Находим игрока
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Перебираем всех врагов в сцене
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // Получаем ссылку на компонент NavMeshAgent
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // Проверяем, виден ли игрок
                if (IsPlayerVisible(enemy))
                {
                    // Идем к игроку
                    agent.SetDestination(player.position);
                }
                else
                {
                    // Проверяем расстояние до лидера
                    if (Vector3.Distance(enemy.transform.position, leader.position) <= leaderDetectionRange)
                    {
                        // Идем к лидеру
                        agent.SetDestination(leader.position);
                    }
                }
            }
        }
    }

    // Проверка, виден ли игрок для конкретного врага
    bool IsPlayerVisible(GameObject enemy)
    {
        // Проверяем, находится ли игрок в пределах радиуса обнаружения врага
        return Vector3.Distance(enemy.transform.position, player.position) <= playerDetectionRange;
    }
}
