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
        Source.outputAudioMixerGroup = AudioManager.Instance.SFX;
        Source.Play();
    }
    private void Update()
    {
        if(Source.clip == SoundID.ChargeWindup.GetVariation(0) && Player.Instance.Weapon.AttackRight < 50)
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
