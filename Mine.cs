using UnityEngine;

public class Mine : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionTimer = 3f;
    public int damage = 50;
    public AudioClip activationSound; // ���� ���������
    public GameObject explosionEffectPrefab; // ������ ����������� �������
    public Animator mineAnimator; // ������ �� ��������� Animator

    private bool activated = false;

    private void Start()
    {
        // �������� ��������� Animator ��� ������ (��������������, ��� �� ��������� �� ��� �� �������)
        mineAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            ActivateMine(other.gameObject);
        }
    }

    private void ActivateMine(GameObject player)
    {
        activated = true;

        // ������������ ����� ���������
        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }

        Invoke("Explode", explosionTimer);
    }

    private void Explode()
    {
        // ���������� ������ ������
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, 2f);
        }

        // ������� ��� ������� � ������� ������
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            // ���� ������ ��������������� � ������� (��������, ����� ��������� ��������), ������� ����
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // ����������� ���� ����� ������
        Destroy(gameObject);
    }
}
