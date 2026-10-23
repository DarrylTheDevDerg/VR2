using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadAudioSettings();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip)
        {
            if (!musicSource.isPlaying) musicSource.Play();
            return;
        }
        
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void LoadAudioSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volumeValue)
    {
        musicSource.volume = volumeValue;
        PlayerPrefs.SetFloat(MUSIC_KEY, volumeValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volumeValue)
    {
        sfxSource.volume = volumeValue;
        PlayerPrefs.SetFloat(SFX_KEY, volumeValue);
        PlayerPrefs.Save();
    }

    public void SetMusicPitch(float pitchValue)
    {
        musicSource.pitch = pitchValue;
    }
    
    public void SetSfxPitch(float pitchValue)
    {
        sfxSource.pitch = pitchValue;
    }

    public float GetPitch(string name)
    {
        if (name == "music")
        {
            return musicSource.pitch;
        }
        
        if (name == "sfx")
        {
            return sfxSource.pitch;
        }

        return 0f;
    }

    public void ResetPitch()
    {
        musicSource.pitch = 1f;
        sfxSource.pitch = 1f;
    }

    public void BGMFadeOut()
    {
        while (musicSource.volume > 0)
        {
            musicSource.volume = Mathf.MoveTowards(musicSource.volume, 0, 0.005f);
        }
        
        if (musicSource.volume <= 0) StopMusic();
        LoadAudioSettings();
    }
}
