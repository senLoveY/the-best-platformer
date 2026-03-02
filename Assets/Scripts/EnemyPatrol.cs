using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public Rigidbody2D rb; // Ошибка: было RigitBody
    public LayerMask groundLayers;
    public Transform groundCheck;
    
    private bool isFacingRight = true;

    private void FixedUpdate()
    {
        // 1. Двигаем врага вперед
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        // 2. Пускаем луч вниз, чтобы проверить, есть ли впереди земля
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayers);

        // 3. Если луч не коснулся земли (пустота впереди)
        if (hit.collider == null)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        
        // Разворачиваем объект (меняем Scale)
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        
        // Инвертируем скорость, чтобы он пошел в обратную сторону
        speed *= -1;
    }

    // Рисуем луч в редакторе, чтобы его было видно
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 1f);
    }
}