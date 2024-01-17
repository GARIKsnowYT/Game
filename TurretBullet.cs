/**************************************************************************************************/
/** © 2017 NULLcode Studio. License: https://creativecommons.org/publicdomain/zero/1.0/deed.ru
/** Разработано в рамках проекта: http://null-code.ru/
/** ****** Внимание! Проекту нужна Ваша помощь! ******
/** WebMoney: R209469863836, Z126797238132, E274925448496, U157628274347
/** Яндекс.Деньги: 410011769316504
/**************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    [SerializeField] private float bulletSpeed = 50;
    private LayerMask layer;

    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)  // Проверка, является ли коллайдер не триггером
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Mathf.RoundToInt(damage)); // Применяем урон к здоровью игрока
            }

            Destroy(gameObject);
        }
    }

    public void SetBullet(Vector3 direction)
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.velocity = direction * bulletSpeed;
        transform.forward = direction;
    }
}
