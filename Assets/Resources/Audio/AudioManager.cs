using UnityEngine;
using UnityEngine.Audio;

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

    public static AudioClip TargetTheme = null;
    public static AudioClip MeadowTheme = null;
    public static AudioClip LeonardTheme = null;
    private static float MusicTransition = 0f;
    private void Update()
    {
        MeadowTheme = MeadowTheme != null ? MeadowTheme : Resources.Load<AudioClip>("Audio/Music/Meadow");
        LeonardTheme = LeonardTheme != null ? LeonardTheme : Resources.Load<AudioClip>("Audio/Music/Leonard");

        if(MusicSource.clip != TargetTheme)
        {
            MusicTransition += Time.deltaTime * 0.4f;
            if(MusicTransition >= 1)
            {
                var temp = MusicSource;
                MusicSource = SecondaryMusicSource;
                SecondaryMusicSource = temp;
                MusicTransition = 0;
            }
        }
        if(MusicTransition != 0)
        {
            SecondaryMusicSource.enabled = true;
            if(SecondaryMusicSource.clip == null)
            {
                SecondaryMusicSource.clip = TargetTheme;
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
        TargetTheme = MeadowTheme;
    }

    private void SwapMusic()
    {

    }
}
