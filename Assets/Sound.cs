using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField]
    public AudioSource Source;
    public void Init(AudioClip clip, float volume = 1, float pitch = 1)
    {
        Source.clip = clip;
        Source.volume = volume;
        Source.pitch = pitch;
        Source.Play();
    }
    private void Update()
    {
        if(Source.clip == GlobalDefinitions.audioClips[33] && Player.Instance.Wand.AttackRight < 50)
        {
            Source.Stop();
            DestroyImmediate(gameObject);
        }
        else if (!Source.isPlaying)
        {
            DestroyImmediate(gameObject);
        }
    }
}
