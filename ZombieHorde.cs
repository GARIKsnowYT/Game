using UnityEngine;
using UnityEngine.AI;

public class ZombieHorde : MonoBehaviour
{
    public string leaderTag = "Leader"; // ��� ������
    public float leaderDetectionRange = 10f; // ������ ����������� ������
    public float playerDetectionRange = 5f; // ������ ����������� ������

    private Transform leader; // �����
    private Transform player; // �����

    private void Start()
    {
        // ������� ������ �� ����
        GameObject leaderObject = GameObject.FindGameObjectWithTag(leaderTag);
        if (leaderObject != null)
        {
            leader = leaderObject.transform;
        }
        else
        {
            Debug.LogError("Leader not found!");
        }

        // ������� ������
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // ���������� ���� ������ � �����
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // �������� ������ �� ��������� NavMeshAgent
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // ���������, ����� �� �����
                if (IsPlayerVisible(enemy))
                {
                    // ���� � ������
                    agent.SetDestination(player.position);
                }
                else
                {
                    // ��������� ���������� �� ������
                    if (Vector3.Distance(enemy.transform.position, leader.position) <= leaderDetectionRange)
                    {
                        // ���� � ������
                        agent.SetDestination(leader.position);
                    }
                }
            }
        }
    }

    // ��������, ����� �� ����� ��� ����������� �����
    bool IsPlayerVisible(GameObject enemy)
    {
        // ���������, ��������� �� ����� � �������� ������� ����������� �����
        return Vector3.Distance(enemy.transform.position, player.position) <= playerDetectionRange;
    }
}
