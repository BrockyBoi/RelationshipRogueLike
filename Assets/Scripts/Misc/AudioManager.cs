using GeneralGame;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

public enum EAudioEvent
{
    WinGame,
    LoseGame,
}

[Serializable]
public class AudioEventClips : UnitySerializedDictionary<EAudioEvent, AudioClip> { }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Title("Audio Source"), SerializeField, Required]
    private AudioSource _backgroundMusicSource;

    [SerializeField]
    private AudioEventClips _clips;

    private List<AudioSource> _soundEffectsSources;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _soundEffectsSources = new List<AudioSource>();
    }

    public void PlaySoundEffect(AudioClip soundEffect)
    {
        if (!ensure(soundEffect != null, "Provided null sound effect"))
        {
            return;
        }

        AudioSource sourceToUse = null;
        foreach (AudioSource source in _soundEffectsSources)
        {
            if (source != null)
            {
                if (source.isPlaying)
                {
                    continue;
                }

                sourceToUse = source;
            }
        }

        if (sourceToUse == null)
        {
            sourceToUse = gameObject.AddComponent<AudioSource>();
            _soundEffectsSources.Add(sourceToUse);
        }

        if (ensure(sourceToUse != null, "Could not get audio source for SFX"))
        {
            sourceToUse.clip = soundEffect;
            sourceToUse.Play();
        }
    }

    public void PlayBackgroundMusic(AudioClip backgroundMusic)
    {
        if (ensure(backgroundMusic != null, "Provided background music that's null"))
        {
            _backgroundMusicSource.Stop();
            _backgroundMusicSource.clip = backgroundMusic;
            _backgroundMusicSource.Play();
        }
    }

    public void PlayAudioEvent(EAudioEvent audioEvent)
    {
        if (ensure(_clips.ContainsKey(audioEvent), "Clips dictionary does not contain " + audioEvent + " in AudioManager"))
        {
            PlaySoundEffect(_clips[audioEvent]);
        }
    }
}
