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
    public float invincibilityTime = 1.5f; 
    private bool isInvincible = false;
    public bool isDead = false;

    [Header("Физика и Прыжки")]
    public Rigidbody2D rb;
    public float bounceForce = 12f; 

    [Header("Анимации и Визуал")]
    public Animator anim;
    private SpriteRenderer sprite;

    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

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
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.playerHurtSound);

            StartCoroutine(BecomeInvincible());
        }
    }

    public void TakeDamageWithKnockback(int damage, Vector2 knockbackForce)
    {
        if (isDead) return;

        TakeDamage(damage);
        
        if (rb != null && !isDead)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
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
        if (isDead) return;

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            EnemyHealth enemy = other.gameObject.GetComponent<EnemyHealth>();
            BossHealth boss = other.gameObject.GetComponent<BossHealth>();

            if (rb.linearVelocity.y < -0.1f && transform.position.y > other.transform.position.y)
            {
                if (enemy != null && !enemy.isDead)
                {
                    enemy.InstantDie(); 
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce); 
                }
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

    public void Die()
    {
        if (isDead) return;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        isDead = true;
        Debug.Log("Игрок погиб...");

        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.playerDeathSound);

        if (anim != null)
            anim.SetTrigger("die");

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        ShootControl sc = GetComponent<ShootControl>();
        if (sc != null) sc.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(1.5f);

        if (LevelManager.instance != null)
            LevelManager.instance.Respawn();

        Destroy(gameObject);
    }
}