using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public float targetHeight = 10f;
    public float heightAmplitude = 2f;
    public float heightFrequency = 1f;
    public float detectionRange = 10f;
    public float shootingCooldown = 1f;
    public Transform gunTransform;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public string patrolPointTag = "point";

    private Rigidbody rb;
    private float originalY;
    private Transform player;
    private bool isPatrolling = true;
    private bool isAttacking = false;
    private float lastShotTime;
    private Transform currentPatrolPoint;
    private int currentPatrolIndex = 0;
    private GameObject[] patrolPoints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalY = transform.position.y;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        patrolPoints = GameObject.FindGameObjectsWithTag(patrolPointTag);
        FindNextPatrolPoint();
    }

    void Update()
    {
        if (isPatrolling)
        {
            PatrollingBehavior();
        }
        else
        {
            AttackPlayerBehavior();
        }
    }

    void PatrollingBehavior()
    {
        if (currentPatrolPoint == null)
        {
            FindNextPatrolPoint();
            return;
        }

        Vector3 direction = (currentPatrolPoint.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        Vector3 newPosition = transform.position;
        newPosition.y = originalY;
        transform.position = newPosition;

        float heightOffset = Mathf.Sin(Time.time * heightFrequency) * heightAmplitude;
        newPosition.y = originalY + heightOffset;
        transform.position = newPosition;

        rb.velocity = transform.forward * moveSpeed;

        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1f)
        {
            FindNextPatrolPoint();
        }

        if (CanSeePlayer())
        {
            isPatrolling = false;
        }
    }

    void AttackPlayerBehavior()
    {
        if (!isAttacking)
        {
            originalY = transform.position.y;
            isAttacking = true;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange && Time.time - lastShotTime > shootingCooldown)
        {
            Shoot();
            lastShotTime = Time.time;
        }

        if (!CanSeePlayer() && Time.time - lastShotTime > 2f)
        {
            isPatrolling = true;
            isAttacking = false;
        }

        Vector3 newPosition = transform.position;
        newPosition.y = targetHeight;
        transform.position = newPosition;
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.position - gunTransform.position;
        RaycastHit hit;
        if (Physics.Raycast(gunTransform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunTransform.position, gunTransform.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        Vector3 shootDirection = (player.position - gunTransform.position).normalized;
        bulletRb.velocity = shootDirection * bulletSpeed;
        Destroy(bullet, 2f);
    }

    void FindNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("Отсутствуют целевые точки с тегом 'point'");
            return;
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        currentPatrolPoint = patrolPoints[currentPatrolIndex].transform;
    }
}
