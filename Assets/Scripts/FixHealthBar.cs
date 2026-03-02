using UnityEngine;

public class FixHealthBar : MonoBehaviour
{
   private Vector3 initialScale;

    void Start()
    {
        // Запоминаем начальный масштаб (например, 0.01, 0.01, 1)
        initialScale = transform.localScale;
    }

    // Используем LateUpdate, чтобы он срабатывал ПОСЛЕ того, 
    // как скрипт игрока развернет персонажа
    void LateUpdate()
    {
        // Если родитель (Игрок) развернут влево (отрицательный масштаб)
        if (transform.parent.localScale.x < 0)
        {
            // Разворачиваем Канвас в обратную сторону относительно родителя
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        }
        else
        {
            // Оставляем как было
            transform.localScale = initialScale;
        }
    }
}
