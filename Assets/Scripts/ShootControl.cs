using UnityEngine;

public class ShootControl : MonoBehaviour
{
    public float fireRate = 0.2f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    private float timeUntilFire; 
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= timeUntilFire)
        {
            Shoot();
            timeUntilFire = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        
        float angle = playerController.transform.localScale.x > 0 ? 0f : 180f;
        AudioManager.instance.PlaySFX(AudioManager.instance.shootSound);

        Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(new Vector3(0f, 0f, angle)));
    }
}