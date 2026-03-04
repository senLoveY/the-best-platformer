using UnityEngine;
using System.Collections;

public class KingSlimeAI : MonoBehaviour
{
    [Header("Настройки Прыжка")]
    public float jumpForce = 15f;
    public float forwardForce = 5f;
    public float attackInterval = 4f;

    [Header("Настройки Урона")]
    public float impactRadius = 4f; 
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
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
{
    BossHealth health = GetComponent<BossHealth>();
    if (health != null && health.isDead) return;
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
    
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
            float direction = (player.position.x > transform.position.x) ? 1 : -1;
            
            anim.SetTrigger("attack");
            rb.AddForce(new Vector2(direction * forwardForce, jumpForce), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(1f);
        canJump = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGrounded && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateImpact();
        }
    }

    void CreateImpact()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, impactRadius);
        
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Player"))
            {
                PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    Vector2 knockbackDir = (obj.transform.position - transform.position).normalized;
                    playerHealth.TakeDamageWithKnockback(impactDamage, knockbackDir * knockbackPower);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}