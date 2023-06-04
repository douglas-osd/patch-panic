using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _musicSource, _effectsSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Plays specified sound effect clip once via effects source.
    public void PlaySound(AudioClip clip)
    {
        _effectsSource.PlayOneShot(clip);
    }

    // Stops current music. Assigns specified clip to music source. Plays music.
    public void PlayMusic(AudioClip clip)
    {
        _musicSource.Stop();
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    // Will pause music if bool is true. Will unpause if bool is false.
    public void MusicPauser(bool shouldPause)
    {
        if(shouldPause)
        {
            _musicSource.Pause();
        }
        if(!shouldPause)
        {
            _musicSource.UnPause();
        }
    }

    // Sets effects mute setting to opposite of current setting.
    public void ToggleEffects()
    {
        _effectsSource.mute = !_effectsSource.mute;
    }

    // Sets music mute setting to opposite of current setting.
    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }
}
