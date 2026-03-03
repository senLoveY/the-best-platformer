using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Параметры здоровья")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("Интерфейс (UI)")]
    public Slider healthSlider;

    [Header("Настройки урона")]
    public float invincibilityTime = 1.5f; // Время неуязвимости после удара
    private bool isInvincible = false;
    public bool isDead = false;

    [Header("Физика и Прыжки")]
    public Rigidbody2D rb;
    public float bounceForce = 12f; // Сила подскока при убийстве врага прыжком

    [Header("Анимации и Визуал")]
    public Animator anim;
    private SpriteRenderer sprite;

    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        
        // Авто-поиск компонентов, если забыли привязать в инспекторе
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

        // Инициализация полоски здоровья
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Метод получения обычного урона
    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        
        // Обновляем слайдер
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Звук получения урона
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.playerHurtSound);

            StartCoroutine(BecomeInvincible());
        }
    }

    // Метод получения урона с отбрасыванием (например, от Босса)
    public void TakeDamageWithKnockback(int damage, Vector2 knockbackForce)
    {
        if (isDead) return;

        TakeDamage(damage);
        
        if (rb != null && !isDead)
        {
            rb.linearVelocity = Vector2.zero; // Сбрасываем текущую скорость
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
    }

    // Корутина мигания при неуязвимости
    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        // Цикл мигания (прозрачный-белый)
        for (float i = 0; i < invincibilityTime; i += 0.2f)
        {
            sprite.color = new Color(1, 1, 1, 0.2f); // Полупрозрачный
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white; // Обычный
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isDead) return;

        // Взаимодействие с обычными врагами или боссом
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            EnemyHealth enemy = other.gameObject.GetComponent<EnemyHealth>();
            BossHealth boss = other.gameObject.GetComponent<BossHealth>();

            // Механика Марио: Прыжок сверху
            // Условие: летим вниз И наши ноги выше центра врага
            if (rb.linearVelocity.y < -0.1f && transform.position.y > other.transform.position.y)
            {
                if (enemy != null && !enemy.isDead)
                {
                    enemy.InstantDie(); // Убиваем обычного врага
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce); // Подскакиваем
                }
                // Примечание: Если хочешь наносить урон Боссу прыжком, добавь проверку для boss здесь
            }
            else
            {
                // Если коснулись не сверху — получаем урон
                TakeDamage(1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // Если упали в зону смерти (яму)
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Die();
        }
    }

    // Запуск процесса смерти
    public void Die()
    {
        if (isDead) return;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        isDead = true;
        Debug.Log("Игрок погиб...");

        // 1. Звук смерти
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.playerDeathSound);

        // 2. Анимация смерти (триггер 'die')
        if (anim != null)
            anim.SetTrigger("die");

        // 3. Выключаем управление, чтобы нельзя было двигаться мертвым
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        ShootControl sc = GetComponent<ShootControl>();
        if (sc != null) sc.enabled = false;

        // 4. Замораживаем физику, чтобы персонаж замер в воздухе/на земле
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // 5. Ждем, пока проиграется анимация (настрой время под свой клип)
        yield return new WaitForSeconds(1.5f);

        // 6. Респаун через LevelManager
        if (LevelManager.instance != null)
            LevelManager.instance.Respawn();

        // 7. Удаляем старый объект игрока
        Destroy(gameObject);
    }
}