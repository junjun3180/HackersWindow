using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource BackGroundSource;
    public AudioSource StartSound;
    public AudioSource FireSound;
    public AudioMixer audioMixer;
    public List<AudioClip> backgroundMusicList;

    // Mute 상태 체크
    public bool MasterVolumeMute = false;
    public bool BGMVolumeMute = false;
    public bool SFXVolumeMute = false;

    public float MasterVolume = -12;
    public float BGMVolume = -12;
    public float SFXVolume = -12;

    private int currentTrackIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        BackGroundSource.ignoreListenerPause = true;
        PlayMusic();  // 첫 트랙 재생
    }

    void Update()
    {
        if (!BackGroundSource.isPlaying)
        {
            currentTrackIndex = (currentTrackIndex + 1) % backgroundMusicList.Count;
            PlayMusic();
        }
    }

    private void PlayMusic()
    {
        if (backgroundMusicList.Count == 0) return;

        BackGroundSource.clip = backgroundMusicList[currentTrackIndex];
        BackGroundSource.Play();
    }

    // 마스터 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        MasterVolume = volume;

        if (volume <= -40.0f || MasterVolumeMute)
        {
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            audioMixer.SetFloat("Master", MasterVolume);
        }
    }

    // BGM 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume;

        if (volume <= -40.0f || BGMVolumeMute)
        {
            audioMixer.SetFloat("BGM", -80f);
        }
        else
        {
            audioMixer.SetFloat("BGM", volume);
        }
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;

        if (volume <= -40.0f || SFXVolumeMute)
        {
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            audioMixer.SetFloat("SFX", volume);
        }
    }

    public void StartButtonSound()
    {
        StartSound.Play();
    }

    public void PlayerFireSound()
    {
        FireSound.Play();
    }
}