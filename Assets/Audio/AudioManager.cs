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
    private void Update()
    {
        MusicSource.volume = PlayerData.MusicVolume;
        //MusicSource.clip = null;
        //if (!MusicSource.isPlaying)
        //{
        //    //print("Playing: " + MusicSource.clip.name);
        //    MusicSource.Play();
        //}
    }
}
