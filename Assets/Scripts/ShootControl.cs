using UnityEngine;

public class ShootControl : MonoBehaviour
{
    public float fireRate = 0.2f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    private float timeUntilFire; // Изменил на private, чтобы не путаться в инспекторе
    private PlayerController playerController;

    void Start()
    {
        // Ошибка: GetComponent пишется с большой буквы
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Логика стрельбы
        if (Input.GetMouseButtonDown(0) && Time.time >= timeUntilFire)
        {
            Shoot();
            timeUntilFire = Time.time + fireRate;
        }
    }

    // Ошибка: метод Shoot должен быть ВНЕ метода Update
    void Shoot()
    {
        
        float angle = playerController.transform.localScale.x > 0 ? 0f : 180f;
        AudioManager.instance.PlaySFX(AudioManager.instance.shootSound);

        // Создаем пулю
        Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(new Vector3(0f, 0f, angle)));
    }
}