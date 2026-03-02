using UnityEngine;
using System.Collections;

public class KingSlimeAI : MonoBehaviour
{
    [Header("Настройки Прыжка")]
    public float jumpForce = 15f;
    public float forwardForce = 5f;
    public float attackInterval = 4f;

    [Header("Настройки Урона")]
    public float impactRadius = 4f; // Радиус удара об землю
    public int impactDamage = 1;
    public float knockbackPower = 10f;

    [Header("Проверка земли")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private bool isGrounded;
    private bool canJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // Находим игрока по тэгу
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
{
    BossHealth health = GetComponent<BossHealth>();
    if (health != null && health.isDead) return;
    // Проверяем землю
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
    
    // Передаем в аниматор значение земли, НО только если мы не в процессе прыжка
    // (Это предотвратит мгновенный возврат в Idle)
    if (rb.linearVelocity.y < 0.1f) 
    {
        anim.SetBool("isGrounded", isGrounded);
    }

    if (isGrounded && canJump)
    {
        StartCoroutine(JumpRoutine());
    }
}

    IEnumerator JumpRoutine()
    {
        canJump = false;
        yield return new WaitForSeconds(attackInterval);

        if (player != null)
        {
            Debug.Log("КОД: Включаю анимацию прыжка!");
            // Определяем направление к игроку
            float direction = (player.position.x > transform.position.x) ? 1 : -1;
            
            // Прыгаем вверх и вперед
            anim.SetTrigger("attack"); // Анимация подготовки/прыжка
            rb.AddForce(new Vector2(direction * forwardForce, jumpForce), ForceMode2D.Impulse);
        }

        // Ждем, пока босс взлетит, прежде чем он снова сможет прыгнуть
        yield return new WaitForSeconds(1f);
        canJump = true;
    }

    // Этот метод вызывается автоматически встроенной физикой Unity при столкновении
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если босс упал на землю (слой Ground)
        if (isGrounded && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateImpact();
        }
    }

    void CreateImpact()
    {
        Debug.Log("БОСС УДАРИЛ ПО ЗЕМЛЕ!");
        // Можно добавить эффект тряски камеры здесь
        
        // Ищем игрока в радиусе поражения
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, impactRadius);
        
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Player"))
            {
                // Наносим урон и отбрасываем
                PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Вычисляем направление отскока (от босса)
                    Vector2 knockbackDir = (obj.transform.position - transform.position).normalized;
                    playerHealth.TakeDamageWithKnockback(impactDamage, knockbackDir * knockbackPower);
                }
            }
        }
    }

    // Визуализация радиуса в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}