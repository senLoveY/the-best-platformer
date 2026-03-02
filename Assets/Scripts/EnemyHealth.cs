using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health = 2; 
    public float knockbackForce = 6f; // Чуть увеличил для эффекта
    public Color hurtColor = Color.red; 
    
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Color originalColor;
    private EnemyPatrol patrolScript;
    public bool isDead = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        patrolScript = GetComponent<EnemyPatrol>();
        originalColor = sprite.color;
    }

    public void TakeDamage(Vector2 knockbackDirection)
    {
        if (isDead) return; // Если уже умирает, ничего не делаем

        health--;

        // 1. Включаем красный цвет
        sprite.color = hurtColor;

        // 2. Делаем отброс
        if (patrolScript != null) patrolScript.enabled = false;
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (health <= 0)
        {
            // Если жизни кончились, запускаем процесс смерти
            StartCoroutine(DeathRoutine());
        }
        else
        {
            // Если еще жив, возвращаем цвет и патруль через время
            StartCoroutine(RecoverRoutine());
        }
    }

    // Процесс восстановления после обычного удара
    IEnumerator RecoverRoutine()
    {
        yield return new WaitForSeconds(0.15f); // Время покраснения
        sprite.color = originalColor;
        yield return new WaitForSeconds(0.1f); // Еще чуть паузы перед ходьбой
        if (patrolScript != null) patrolScript.enabled = true;
    }

    // Добавь это внутрь класса EnemyHealth
    public void InstantDie()
    {
        if (isDead) return;
        health = 0; // Сбрасываем здоровье
        StartCoroutine(DeathRoutine()); // Запускаем ту же красивую смерть с покраснением и вылетом
    }

    // Процесс смерти
    IEnumerator DeathRoutine()
    {
        isDead = true;
        
        // Враг уже красный и уже летит от AddForce в методе TakeDamage
        
        // Ждем, пока он летит в красном цвете (например, 0.3 секунды)
        yield return new WaitForSeconds(0.3f);

        // Можно добавить эффект затухания (плавно убираем прозрачность)
        float timer = 0;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            Color c = sprite.color;
            c.a = Mathf.Lerp(1, 0, timer / 0.2f); // Плавное исчезновение
            sprite.color = c;
            yield return null;
        }

        Destroy(gameObject);

        
    }
}