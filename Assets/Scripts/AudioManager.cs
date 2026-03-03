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
    public AudioClip playerDeathSound;
    public AudioClip playerHurtSound;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        
        sfxSource.PlayOneShot(clip);
    }

    public void ChangeMusic(AudioClip newMusic)
    {
        musicSource.Stop(); 
        musicSource.clip = newMusic; 
        musicSource.loop = false; 
        musicSource.Play(); 
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource.clip == backgroundMusic && musicSource.isPlaying) return;
        
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }


}