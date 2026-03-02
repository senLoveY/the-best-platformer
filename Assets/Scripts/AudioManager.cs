using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Источники звука")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Звуковые клипы")]
    public AudioClip victoryMusic;
    public AudioClip jumpSound;
    public AudioClip shootSound;
    public AudioClip backgroundMusic;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Объект "выживет" при смене сцены
        }
        else
        {
            Destroy(gameObject); // Удаляем дубликат, если он создался на новой сцене
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        // PlayOneShot позволяет проигрывать звуки поверх друг друга не прерывая их
        sfxSource.PlayOneShot(clip);
    }

    public void ChangeMusic(AudioClip newMusic)
    {
        musicSource.Stop(); // Останавливаем старую
        musicSource.clip = newMusic; // Заменяем трек
        musicSource.loop = false; // Победная музыка не должна зацикливаться
        musicSource.Play(); // Включаем новую
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource.clip == backgroundMusic && musicSource.isPlaying) return;
        
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }


}