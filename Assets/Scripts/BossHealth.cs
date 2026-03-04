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
    public bool isDead = false; 

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
        anim.SetTrigger("die");
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; 
        KingSlimeAI ai = GetComponent<KingSlimeAI>();
        if (ai != null) ai.enabled = false;
        yield return new WaitForSeconds(1f);

        if (finishPoint != null) finishPoint.SetActive(true);
        Destroy(gameObject);
    }
}