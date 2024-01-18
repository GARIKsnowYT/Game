using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Worq.AEAI.HealthAndDamage;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 5f;
    public float staminaDuration = 2f;
    public Slider staminaSlider;

    private Rigidbody rb;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isRunning = false;
    private float currentStamina;
    private float maxStamina = 100f;
    private AudioSource footstepAudioSource;
    public AudioClip footstepSound; // Переменная для звука ходьбы
    [SerializeField] private PlayerAttack playerAttack;
    // Добавленные переменные
    private bool isWalking;

    public Transform spawnPointPlayer;

    public CanvasController canvasController; // Присвойте CanvasController в инспекторе

    void Start()
    {
        
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCamera = GetComponentInChildren<Camera>();
        currentStamina = maxStamina;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Инициализация аудио
        footstepAudioSource = gameObject.GetComponent<AudioSource>();
        footstepAudioSource.spatialBlend = 1f; // Делаем звук пространственным
        footstepAudioSource.volume = 0.5f; // Устанавливаем громкость
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.loop = true;

        // Присваиваем аудиоклип для звука ходьбы
        footstepAudioSource.clip = footstepSound;
        
    }
    void PlayFootstepSound()
    {
        if (isWalking && isGrounded)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play();
            }
        }
        else
        {
            footstepAudioSource.Stop();
        }
    }

    void Update()
    {
        
        HandleMovement();
        HandleMouseLook();
        HandleJump();
        HandleCrouch();
        HandleSprint();
        UpdateStaminaUI();
        HandleCursorLock();
        PlayFootstepSound(); // Добавленный метод для звука ходьбы
    }

    void HandleMovement()
    {
        float moveSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
        float forwardMovement = Input.GetAxis("Vertical") * moveSpeed;
        float sideMovement = Input.GetAxis("Horizontal") * moveSpeed;

        Vector3 movement = transform.forward * forwardMovement + transform.right * sideMovement;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Проверка наличия ввода для ходьбы
        isWalking = Mathf.Abs(forwardMovement) > 0.1f || Mathf.Abs(sideMovement) > 0.1f;
        //Debug.Log(rb.velocity);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return null;
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            playerCamera.transform.localPosition = isCrouching ? new Vector3(0f, 0.5f, 0f) : new Vector3(0f, 0.75f, 0f);
        }
    }

    void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && currentStamina > 0)
        {
            isRunning = true;
            currentStamina -= Time.deltaTime;
        }
        else
        {
            isRunning = false;
            if (currentStamina < maxStamina)
            {
                currentStamina += Time.deltaTime / staminaDuration;
            }
        }
    }

    void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }

    void HandleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void Die()
    {
        canvasController.ShowDeathCanvas();
        ToggleController(false); // Выключаем скрипт при смерти
    }
    public Health Health;
    public void RespawnPlayer()
    {
        if (spawnPointPlayer != null)
        {
            transform.position = spawnPointPlayer.position;
            Health.ResetParameters(); // Сброс параметров здоровья, голода и жажды
            enabled = true;
        }
    }

    // Метод для включения/выключения контроллера
    public void ToggleController(bool isEnabled)
    {
        enabled = isEnabled;
    }
}