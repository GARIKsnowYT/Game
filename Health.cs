using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public int maxHealth = 100;
    public int health;

    [Header("Thirst")]
    [SerializeField] public int maxThirst = 100;
    public float currentThirst;
    [SerializeField] private float thirstRate = 0.5f;
    [SerializeField] private Text thirstText;
    public Slider thirstSlider;

    [Header("Hunger")]
    [SerializeField] public int maxHunger = 100;
    public float currentHunger;
    [SerializeField] private float hungerRate = 0.5f;
    [SerializeField] private Text hungerText;
    public Slider hungerSlider;

    [Header("UI")]
    public Slider healthSlider;
    public Text healthText;

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public int MaxThirst
    {
        get { return maxThirst; }
    }

    public int MaxHunger
    {
        get { return maxHunger; }
    }

    private void Start()
    {
        health = maxHealth;
        currentThirst = maxThirst;
        currentHunger = maxHunger;

        healthSlider.maxValue = maxHealth;
        thirstSlider.maxValue = maxThirst;
        hungerSlider.maxValue = maxHunger;

        UpdateThirstText();
        UpdateHungerText();
        healthText.text = "Health: " + health.ToString();

        healthSlider.value = health;
        healthText.text = "Health: " + health.ToString();

        InvokeRepeating("LoseThirstAndHunger", 1f, 1f);
        InvokeRepeating("LoseHealth", 1f, 1f);
    }

    private void UpdateThirstText()
    {
        thirstText.text = "Thirst: " + Mathf.FloorToInt(currentThirst).ToString();
    }

    private void UpdateHungerText()
    {
        hungerText.text = "Hunger: " + Mathf.FloorToInt(currentHunger).ToString();
    }

    private void Update()
    {
        if (currentThirst <= 0 || currentHunger <= 0)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log("Вошли тейк дамаг");
        health -= damageAmount;
        health = Mathf.Max(health, 0);

        healthSlider.value = health;
        healthText.text = "Health: " + health.ToString();

        if (health <= 0)
        {
            // Оповещаем контроллер конваса о смерти
            FindObjectOfType<CanvasController>().ShowDeathCanvas();

            // Выключаем контроллер игрока
            FindObjectOfType<FirstPersonController>().enabled = false;
        }
    }

    private void LoseThirstAndHunger()
    {
        currentThirst -= thirstRate;
        currentHunger -= hungerRate;

        currentThirst = Mathf.Max(currentThirst, 0);
        currentHunger = Mathf.Max(currentHunger, 0);

        thirstSlider.value = Mathf.FloorToInt(currentThirst);
        UpdateThirstText();
        hungerSlider.value = Mathf.FloorToInt(currentHunger);
        UpdateHungerText();
    }

    private void LoseHealth()
    {
        if (currentThirst <= 0 || currentHunger <= 0)
        {
            TakeDamage(1);
        }
    }

    public void CollectItem(ItemType itemType, int amount)
    {
        switch (itemType)
        {
            case ItemType.Health:
                RestoreHealth(amount);
                break;
            case ItemType.Thirst:
                RestoreThirst(amount);
                break;
            case ItemType.Hunger:
                RestoreHunger(amount);
                break;
        }
    }

    public void RestoreHealth(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        healthSlider.value = health;
        healthText.text = "Health: " + health.ToString();
    }

    public void RestoreThirst(int amount)
    {
        currentThirst += amount;
        currentThirst = Mathf.Min(currentThirst, maxThirst);
        thirstSlider.value = Mathf.FloorToInt(currentThirst);
        UpdateThirstText();
    }
    public void ResetParameters()
    {
        health = maxHealth;
        currentThirst = maxThirst;
        currentHunger = maxHunger;

        healthSlider.value = health;
        healthText.text = "Health: " + health.ToString();

        thirstSlider.value = Mathf.FloorToInt(currentThirst);
        UpdateThirstText();
        hungerSlider.value = Mathf.FloorToInt(currentHunger);
        UpdateHungerText();
    }
    public void RestoreHunger(int amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Min(currentHunger, maxHunger);
        hungerSlider.value = Mathf.FloorToInt(currentHunger);
        UpdateHungerText();
    }
}