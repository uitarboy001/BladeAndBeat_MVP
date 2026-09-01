using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource; 
    [SerializeField] private AudioSource clashSource; 

    [Header("Background Music")]
    public AudioClip bgmClip;

    [Header("Sound Effects")]
    public AudioClip parryClip;
    public AudioClip missClip;
    public AudioClip clashStartClip;
    public AudioClip clashHitClip;
    public AudioClip clashWinClip;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayClashHit(float pitch = 1f)
    {
        if (clashHitClip != null && clashSource != null)
        {
            clashSource.pitch = pitch;
            clashSource.PlayOneShot(clashHitClip);
        }
    }
}