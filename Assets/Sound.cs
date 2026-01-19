using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField]
    public AudioSource Source;
    private bool HasEnded = false;
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
        if (!Source.isPlaying && !Source.loop)
        {
            Destroy(gameObject);
        }
    }
    public void PylonSoundUpdate(Pylon p)
    {
        if (Source.clip == SoundID.PylonDrone.GetVariation(0))
        {
            float dist = Vector2.Distance(transform.position, Player.Position);
            if (!HasEnded)
                Source.loop = dist <= Main.PylonActivationDist;
            else
            {
                HasEnded = true;
                Source.loop = false;
            }
            float vol = 1 - Mathf.Min(1, Mathf.Pow(dist / Main.PylonActivationDist, 2));
            Source.volume = vol * 0.9f * PlayerData.SFXVolume;
            if (p.Complete)
            {
                float percent = p.CompleteAnimCounter >= 200 ? 1 : p.CompleteAnimCounter / 200f * 0.75f;
                if(percent >= 1)
                    Source.pitch = Mathf.Lerp(Source.pitch, 0.5f, 0.04f);
                else
                    Source.pitch = Mathf.Lerp(1.0f, 1.5f, percent);
            }
        }
    }
}
