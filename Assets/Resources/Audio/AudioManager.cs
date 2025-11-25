using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup Master;
    public AudioMixerGroup Music;
    public AudioMixerGroup SFX;
    [SerializeField]
    private AudioSource MusicSource;
    [SerializeField]
    private AudioSource SecondaryMusicSource;
    [SerializeField]
    private GameObject AudioObject;
    private static AudioManager m_Instance;
    public static AudioManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = FindObjectOfType<AudioManager>();
            return m_Instance;
        }
        private set
        {
            m_Instance = value;
        }
    }
    public static void PlaySound(SoundClip soundID, Vector2 position, float volume = 1, float pitch = 1, int variation = -1)
    {
        PlaySound(soundID.GetVariation(variation), position, volume, pitch);
    }
    public static void PlaySound(AudioClip soundID, Vector2 position, float volume = 1, float pitch = 1)
    {
        if (PlayerData.SFXVolume <= 0)
            return;
        Sound sound = Instantiate(Instance.AudioObject, position, Quaternion.identity).GetComponent<Sound>();
        sound.Init(soundID, volume * PlayerData.SFXVolume, pitch);
    }
    private void Start()
    {
        m_Instance = this;
    }

    private static AudioClip CurrentTheme = null;
    public static AudioClip MenuTheme = null;
    public static AudioClip MeadowTheme = null;
    public static AudioClip LeonardTheme = null;
    private static float MusicTransition = 0f;
    private static int MusicPriority = 0;
    private void Update()
    {
        MenuTheme = MenuTheme != null ? MenuTheme : Resources.Load<AudioClip>("Audio/Music/BubbleBathBonanza");
        MeadowTheme = MeadowTheme != null ? MeadowTheme : Resources.Load<AudioClip>("Audio/Music/Meadow");
        LeonardTheme = LeonardTheme != null ? LeonardTheme : Resources.Load<AudioClip>("Audio/Music/Leonard");


        if (MusicSource.clip != CurrentTheme)
        {
            MusicTransition += Time.deltaTime * 0.5f;
            if(MusicTransition >= 1)
            {
                (SecondaryMusicSource, MusicSource) = (MusicSource, SecondaryMusicSource);
                MusicTransition = 0;
            }
        }
        else
        {
            MusicTransition -= Time.deltaTime * 0.5f;
            if (MusicTransition <= 0)
                MusicTransition = 0;
        }
        if(MusicTransition != 0)
        {
            SecondaryMusicSource.enabled = true;
            if(SecondaryMusicSource.clip == null)
            {
                SecondaryMusicSource.clip = CurrentTheme;
                SecondaryMusicSource.time = 0;
                SecondaryMusicSource.Play();
            }
        }
        else
        {
            SecondaryMusicSource.enabled = false;
            if (SecondaryMusicSource.clip != null)
            {
                SecondaryMusicSource.clip = null;
                SecondaryMusicSource.time = 0;
            }
        }
        MusicSource.volume = PlayerData.MusicVolume * (1 - MusicTransition);
        SecondaryMusicSource.volume = PlayerData.MusicVolume * MusicTransition;
        //MusicSource.clip = null;
        //if (!MusicSource.isPlaying)
        //{
        //    //print("Playing: " + MusicSource.clip.name);
        //    MusicSource.Play();
        //}
        CurrentTheme = SceneManager.GetActiveScene().buildIndex == 0 ? MenuTheme : MeadowTheme;
        MusicPriority = 0;
    }
    public static void SetMusic(AudioClip Music, int Priority = 1)
    {
        if(MusicPriority < Priority)
        {
            MusicPriority = Priority;
            CurrentTheme = Music;
        }
    }
}
