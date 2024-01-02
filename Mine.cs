using UnityEngine;

public class Mine : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionTimer = 3f;
    public int damage = 50;
    public AudioClip activationSound; // Звук активации
    public GameObject explosionEffectPrefab; // Префаб визуального эффекта
    public Animator mineAnimator; // Ссылка на компонент Animator

    private bool activated = false;

    private void Start()
    {
        // Получаем компонент Animator при старте (предполагается, что он находится на том же объекте)
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

        // Проигрывание звука активации
        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }

        Invoke("Explode", explosionTimer);
    }

    private void Explode()
    {
        // Визуальный эффект взрыва
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, 2f);
        }

        // Находим все объекты в радиусе взрыва
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            // Если объект взаимодействует с взрывом (например, имеет компонент здоровья), наносим урон
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // Уничтожение мины после взрыва
        Destroy(gameObject);
    }
}
