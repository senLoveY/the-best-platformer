using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health = 2; 
    public float knockbackForce = 6f; 
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
        if (isDead) return; 

        health--;

        sprite.color = hurtColor;

        if (patrolScript != null) patrolScript.enabled = false;
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (health <= 0)
        {
            StartCoroutine(DeathRoutine());
        }
        else
        {
            StartCoroutine(RecoverRoutine());
        }
    }

    IEnumerator RecoverRoutine()
    {
        yield return new WaitForSeconds(0.15f); 
        sprite.color = originalColor;
        yield return new WaitForSeconds(0.1f); 
        if (patrolScript != null) patrolScript.enabled = true;
    }

    public void InstantDie()
    {
        if (isDead) return;
        health = 0; 
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        isDead = true;
        yield return new WaitForSeconds(0.3f);
        float timer = 0;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            Color c = sprite.color;
            c.a = Mathf.Lerp(1, 0, timer / 0.2f); 
            sprite.color = c;
            yield return null;
        }

        Destroy(gameObject);

        
    }
}