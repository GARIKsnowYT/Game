using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Turret : MonoBehaviour
{
    [SerializeField] private TurretBullet bulletPrefab;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private float smooth = 1;
    [SerializeField] private float rayOffset = 1;
    [SerializeField] private float damage = 10;
    [SerializeField] private Transform[] bulletPoint;
    [SerializeField] private Transform turretRotation;
    [SerializeField] private Transform center;
    [SerializeField] private bool useLimits;
    [SerializeField][Range(0, 180)] private float limitY = 50;
    [SerializeField][Range(0, 180)] private float limitX = 30;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip detectSound;

    private SphereCollider turretTrigger;
    private Transform target;
    private Vector3 offset;
    private int index;
    private float curFireRate;
    private Quaternion defaultRot = Quaternion.identity;
    private bool isObjectInsideTrigger = false;

    private AudioSource audioSource;

    void Awake()
    {
        turretTrigger = GetComponent<SphereCollider>();
        turretTrigger.isTrigger = true;
        offset = turretTrigger.center;
        curFireRate = fireRate;
        turretTrigger.enabled = true;
        enabled = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isObjectInsideTrigger = true;
            target = other.transform;
            StartCoroutine(EnableColliderDelayed());
            PlaySound(detectSound);
            enabled = true;
        }
    }

    IEnumerator EnableColliderDelayed()
    {
        yield return new WaitForSeconds(2f);
        turretTrigger.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isObjectInsideTrigger = false;
        }
    }

    bool Search()
    {
        if (rayOffset < 0) rayOffset = 0;
        float dist = Vector3.Distance(transform.position + offset, target.position);
        Vector3 lookPos = target.position - turretRotation.position;
        Debug.DrawRay(turretRotation.position, center.forward * (turretTrigger.radius + rayOffset));
        Vector3 rotation = Quaternion.Lerp(turretRotation.rotation, Quaternion.LookRotation(lookPos), smooth * Time.deltaTime).eulerAngles;

        if (useLimits)
        {
            rotation = CalculateNegativeValues(rotation);
            rotation.y = Mathf.Clamp(rotation.y, -limitY, limitY);
            rotation.x = Mathf.Clamp(rotation.x, -limitX, limitX);
        }

        rotation.z = 0;
        turretRotation.eulerAngles = rotation;

        if (dist > turretTrigger.radius + rayOffset)
        {
            target = null;
            return false;
        }

        if (IsRaycastHit(center)) return true;

        return false;
    }

    bool IsRaycastHit(Transform point)
    {
        RaycastHit hit;
        Ray ray = new Ray(point.position, point.forward);
        if (Physics.Raycast(ray, out hit, turretTrigger.radius + rayOffset))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void Shot()
    {
        if (!Search()) return;

        curFireRate += Time.deltaTime;
        if (curFireRate > fireRate)
        {
            Transform point = GetPoint();
            curFireRate = 0;

            PlaySound(shootSound);

            if (bulletPrefab != null)
            {
                TurretBullet bullet = Instantiate(bulletPrefab, point.position, Quaternion.identity) as TurretBullet;
                bullet.SetBullet(point.forward);
            }
            else if (IsRaycastHit(point))
            {
                Health playerHealth = target.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(Mathf.RoundToInt(damage));
                }
            }
        }
    }

    Transform GetPoint()
    {
        if (index == bulletPoint.Length - 1) index = 0; else index++;
        return bulletPoint[index];
    }

    void LateUpdate()
    {
        if (isObjectInsideTrigger)
        {
            if (target != null)
            {
                Shot();
            }
            else
            {
                Choice();
            }
        }
    }

    void Choice()
    {
        curFireRate = fireRate;

        target = FindTarget();

        turretRotation.rotation = Quaternion.Lerp(turretRotation.rotation, defaultRot, smooth * Time.deltaTime);

        if (Quaternion.Angle(turretRotation.rotation, defaultRot) == 0)
        {
            turretRotation.rotation = defaultRot;
            turretTrigger.enabled = true;
            enabled = false;
        }
    }

    private float lastDetectionTime = 0f;
    private float detectionCooldown = 10f;

    Transform FindTarget()
    {
        if (Time.time - lastDetectionTime < detectionCooldown)
        {
            return null; // Прошло менее 10 секунд с момента последнего обнаружения игрока
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position + offset, turretTrigger.radius);

        Collider currentCollider = null;
        float dist = Mathf.Infinity;

        foreach (Collider coll in colliders)
        {
            float currentDist = Vector3.Distance(transform.position + offset, coll.transform.position);

            if (currentDist < dist && coll.CompareTag("Player"))
            {
                currentCollider = coll;
                dist = currentDist;
            }
        }

        if (currentCollider != null)
        {
            lastDetectionTime = Time.time; // Запоминаем время последнего обнаружения игрока
        }

        return currentCollider?.transform;
    }
    void RespawnCollider()
    {
        turretTrigger.enabled = true;
    }

    Vector3 CalculateNegativeValues(Vector3 eulerAngles)
    {
        eulerAngles.y = (eulerAngles.y > 180) ? eulerAngles.y - 360 : eulerAngles.y;
        eulerAngles.x = (eulerAngles.x > 180) ? eulerAngles.x - 360 : eulerAngles.x;
        eulerAngles.z = (eulerAngles.z > 180) ? eulerAngles.z - 360 : eulerAngles.z;
        return eulerAngles;
    }

    void PlaySound(AudioClip sound)
    {
        if (sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }
}