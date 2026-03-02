using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 15f;
    public Rigidbody2D bulletRb;

    void Start()
    {
        Destroy(gameObject, 3f); // Удаляем пулю через 3 сек, если никуда не попала
    }

    private void FixedUpdate()
    {
        bulletRb.linearVelocity = transform.right * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Проверка на обычного врага
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage(transform.right);
            Destroy(gameObject);
        }
        
        // 2. ПРОВЕРКА НА БОССА
        if (other.CompareTag("Boss"))
        {
            BossHealth boss = other.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(1); // Отнимаем 1 хп
            }
            Destroy(gameObject); // Пуля исчезает после попадания
        }

        // 3. Если попали в землю (Tilemap)
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}