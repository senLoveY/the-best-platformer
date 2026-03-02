using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Скорости")]
    public float playerSpeed = 8f;
    public float sprintSpeed = 14f;
    public float jumpForce = 12f; 

    [Header("Компоненты")]
    public Rigidbody2D playerRb;
    public Transform groundCheck; 
    public Animator anim;
    public AudioSource sprintAudioSource; // Сюда перетащим новый Audio Source

    [Header("Настройки Прыжка")]
    public float checkRadius = 0.2f; 
    public LayerMask groundLayer; 

    private float movementX;
    private bool isGrounded; 

    [HideInInspector] public bool isRight = true;

    private void Update()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Звук прыжка
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.jumpSound);
            
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        HandleSprintSound(); // Вызываем наш новый метод
        UpdateAnimations(); // Обновляем анимации
        HandleFlip();       // Поворачиваем персонажа
    }

    private void FixedUpdate()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : playerSpeed;
        playerRb.linearVelocity = new Vector2(movementX * currentSpeed, playerRb.linearVelocity.y);
    }
    
    // --- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ---
    
    void HandleSprintSound()
    {
        // Если мы бежим с ускорением по земле
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && Mathf.Abs(movementX) > 0.05f)
        {
            // И если звук еще не играет - включаем его
            if (!sprintAudioSource.isPlaying)
            {
                sprintAudioSource.Play();
            }
        }
        else
        {
            // В любом другом случае - выключаем
            if (sprintAudioSource.isPlaying)
            {
                sprintAudioSource.Stop();
            }
        }
    }

    void UpdateAnimations()
    {
        if (Mathf.Abs(movementX) > 0.05f)
        {
            anim.SetBool("isRunning", true);
            anim.speed = Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1f;
        }
        else
        {
            anim.SetBool("isRunning", false);
            anim.speed = 1f;
        }
        anim.SetBool("isGrounded", isGrounded);
    }

    void HandleFlip()
    {
        if (movementX > 0) 
        {
            transform.localScale = new Vector3(2f, 2f, 1f);
            isRight = true;
        }
        else if (movementX < 0) {
            transform.localScale = new Vector3(-2f, 2f, 1f);
            isRight = false;
        }
    }
}