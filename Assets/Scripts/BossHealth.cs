using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public int health = 10;
    public GameObject finishPoint;
    public Color hurtColor = Color.red;

    private SpriteRenderer sprite;
    private Color originalColor;
    private Animator anim;
    private Rigidbody2D rb;
    public bool isDead = false; // Сделали публичным, чтобы ИИ видел это

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = sprite.color;
        
        if (finishPoint != null) finishPoint.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        StopAllCoroutines();
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            StartCoroutine(DeathRoutine());
        }
    }

    IEnumerator FlashRed()
    {
        sprite.color = hurtColor;
        yield return new WaitForSeconds(0.15f);
        sprite.color = originalColor;
    }

    IEnumerator DeathRoutine()
    {
        isDead = true;
        Debug.Log("БОСС НАЧИНАЕТ УМИРАТЬ...");

        // 1. Включаем анимацию смерти
        anim.SetTrigger("die");

        // 2. Отключаем физику и столкновения, чтобы игрок не бился об "труп"
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; // Чтобы босс замер на месте

        // 3. Выключаем скрипт ИИ, чтобы босс не прыгал во время смерти
        KingSlimeAI ai = GetComponent<KingSlimeAI>();
        if (ai != null) ai.enabled = false;

        // 4. Ждем, пока проиграется анимация (например, 2 секунды)
        // Можешь настроить это время под длину своей анимации
        yield return new WaitForSeconds(1f);

        // 5. Показываем финиш
        if (finishPoint != null) finishPoint.SetActive(true);

        // 6. Удаляем босса
        Destroy(gameObject);
    }
}