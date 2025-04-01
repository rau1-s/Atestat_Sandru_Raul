using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource source;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }
    public void PlaySoundQuieter(AudioClip clip, float volume)
    {
        source.PlayOneShot(clip, volume);
    }
}
