using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Параметры здоровья")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Настройки урона")]
    public float invincibilityTime = 1f; 
    private bool isInvincible = false;
    private bool isDead = false; // Чтобы смерть не срабатывала несколько раз

    [Header("Компоненты")]
    public Rigidbody2D rb;
    public Animator anim; // Ссылка на аниматор
    public float bounceForce = 12f;

    private SpriteRenderer sprite;

    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        
        // Автоматически находим компоненты, если забыли привязать
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

        // Настраиваем слайдер
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return; 

        currentHealth -= damage;
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvincible());
        }
    }

    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        for (float i = 0; i < invincibilityTime; i += 0.2f)
        {
            sprite.color = new Color(1, 1, 1, 0.2f); 
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isDead) return; // Если мертв, столкновения не считаем

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            // Пытаемся получить скрипт обычного врага или босса
            EnemyHealth enemy = other.gameObject.GetComponent<EnemyHealth>();
            BossHealth boss = other.gameObject.GetComponent<BossHealth>();

            // Проверка на убийство прыжком (если это не босс, или босс тоже умирает от прыжка)
            if (rb.linearVelocity.y < -0.1f && transform.position.y > other.transform.position.y)
            {
                if (enemy != null && !enemy.isDead)
                {
                    enemy.InstantDie();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
                }
                // Если ты решишь, что Босса тоже можно бить прыжком, можно добавить логику и сюда
            }
            else
            {
                TakeDamage(1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.gameObject.CompareTag("DeathZone"))
        {
            Die(); 
        }
    }

    // --- НОВАЯ ФУНКЦИЯ СМЕРТИ С АНИМАЦИЕЙ ---
    void Die()
    {
        if (isDead) return;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        isDead = true;
        
        // 1. Запускаем анимацию смерти
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        // 2. Отключаем управление и стрельбу
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        ShootControl sc = GetComponent<ShootControl>();
        if (sc != null) sc.enabled = false;

        // 3. Замораживаем физику, чтобы игрок не падал скворь пол во время анимации
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // 4. Ждем завершения анимации (подбери время под свою анимацию)
        yield return new WaitForSeconds(1.5f);

        // 5. Респаун
        if (LevelManager.instance != null)
        {
            LevelManager.instance.Respawn();
        }

        // 6. Удаляем старый объект
        Destroy(gameObject);
    }

    public void TakeDamageWithKnockback(int damage, Vector2 knockbackForce)
    {
        if (isDead) return;

        TakeDamage(damage);
        
        if (rb != null && !isDead) // Отбрасываем только если еще жив
        {
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
    }
}