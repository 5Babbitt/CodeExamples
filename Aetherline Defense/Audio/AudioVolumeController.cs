using UnityEngine;

public class AudioVolumeController
{
    public float masterVolume {  get; private set; }
    public float musicVolume { get; private set; }
    public float sfxVolume { get; private set; }

    public void Init()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 75);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 75);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 75);
    }

    public void UpdateVolumes()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);
        AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);
        AkSoundEngine.SetRTPCValue("SFXVolume", sfxVolume);
    }

    public void SetVolume(EVolume type, float volume)
    {
        switch (type)
        {
            case EVolume.Master:
                masterVolume = volume;
                break;
            case EVolume.Music:
                musicVolume = volume; 
                break;
            case EVolume.SFX: 
                sfxVolume = volume; 
                break;
        }

        UpdateVolumes();
    }
}

public enum EVolume
{ 
    Master,
    Music,
    SFX
}