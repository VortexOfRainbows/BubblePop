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
        if(Source.clip == SoundID.ChargeWindup.GetVariation(0))
        {
            transform.position = Player.Instance.transform.position;
            if(Player.Instance.Weapon.AttackRight < 50)
            {
                Source.Stop();
                Destroy(gameObject);
            }
        }
        else if(Source.clip == SoundID.TeleportCharge.GetVariation(0) || Source.clip == SoundID.TeleportSustain.GetVariation(0))
        {
            bool usingTeleport = Control.Ability && !ThoughtBubble.FinishedTeleport;
            transform.position = Player.Instance.transform.position;
            if (!Source.isPlaying && usingTeleport)
            {
                AudioManager.PlaySound(SoundID.TeleportSustain, Player.Instance.transform.position, 1f, 1);
            }
            if (!usingTeleport)
            {
                Source.Stop();
                Destroy(gameObject);
            }
        }
        if(Source.clip == SoundID.PylonDrone.GetVariation(0))
        {
            if (Main.PlayerNearPylon || Main.WavesUnleashed)
                Source.loop = true;
            else
                Source.loop = false;
            float dist = Vector2.Distance(transform.position, Player.Position) - 2;
            if (dist < 0)
                dist = 0;
            float vol = 1 - Mathf.Min(1, Mathf.Pow(dist / 8f, 2));
            Source.volume = vol * 0.8f * PlayerData.SFXVolume;
        }
        if (!Source.isPlaying && !Source.loop)
        {
            Destroy(gameObject);
        }
    }
}
