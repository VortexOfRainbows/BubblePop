using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Still needs music!
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
    public static void PlaySound(AudioClip soundID, Vector2 position, float volume = 1, float pitch = 1)
    {
        Sound sound = Instantiate(Instance.AudioObject, position, Quaternion.identity).GetComponent<Sound>();
        sound.Init(soundID, volume, pitch);
    }

    private void Start()
    {
        m_Instance = this;
    }
    private void Update()
    {
        //MusicSource.clip = null;
        //if (!MusicSource.isPlaying)
        //{
        //    //print("Playing: " + MusicSource.clip.name);
        //    MusicSource.Play();
        //}
    }
}
