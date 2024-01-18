using System.Collections;
using UnityEngine;

public class HandShake : MonoBehaviour
{
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 1f;
    public float shakeDistance = 0.1f;
    public float returnSpeed = 2f;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            ShakeHands();
        }
        else
        {
            // Плавно возвращаем руки в исходное положение
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * returnSpeed);
        }
    }

    private void ShakeHands()
    {
        // Вычисляем смещение по X и Y на основе синусоидальной функции
        float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        float offsetY = -Mathf.Abs(Mathf.Cos(Time.time * shakeSpeed)) * shakeDistance;

        // Применяем смещение к позиции рук
        Vector3 newPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);
        transform.localPosition = newPosition;
    }
}
