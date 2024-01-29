using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] sounds;

    protected override void Awake()
    {
        base.Awake();
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = 1;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.audioMixerGroup;
        }
    }

    public void AmbianceTransition(string ambiance, string past)
    {
        Stop(past);
        Play(ambiance, true);
    }

    public void Play(string name, bool isFaded = false)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if (sound == null)
        {
            return;
        }

        if (sound.isSingleUsed)
        {
            if (sound.source.isPlaying)
            {
                return;
            }
        }

        if (isFaded)
        {
            sound.source.volume = 0;
            StartCoroutine(FadedIn(sound));
            return;
        }
        sound.source.Play();
    }

    public void PitchModifier(string name, float pitch)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if (sound == null)
        {
            return;
        }

        StartCoroutine(pitchSmootherModifier(sound.source, pitch));
    }

    IEnumerator pitchSmootherModifier(AudioSource source, float toPitch, float duration = 3f)
    {
        float currentPitch = source.pitch;
        float elapsedTime = 0.1f;
        while (elapsedTime < duration)
        {
            float modifierPitch = Mathf.Lerp(currentPitch, toPitch, elapsedTime / duration);
            source.pitch = modifierPitch;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        source.pitch = toPitch;
    }

    public void PlayOnShot(string name, bool isFaded = false)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if (sound == null)
        {
            return;
        }

        if (sound.isSingleUsed)
        {
            if (sound.source.isPlaying)
            {
                return;
            }
        }

        if (isFaded)
        {
            sound.source.volume = 0;
            StartCoroutine(FadedIn(sound));
            return;
        }
        sound.source.PlayOneShot(sound.source.clip);
    }
    public void Play(out AudioSource source, string name, bool isFaded = false)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        source = sound.source;

        if (sound == null)
        {
            return;
        }

        if (sound.isSingleUsed)
        {
            if (sound.source.isPlaying)
            {
                return;
            }
        }

        if (isFaded)
        {
            sound.source.volume = 0;
            StartCoroutine(FadedIn(sound));
            return;
        }
        sound.source.Play();
    }

    public void Stop(string name, bool isFaded = false)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            return;
        }
        if (isFaded)
        {
            sound.source.volume = 1;
            StartCoroutine(FadedOut(sound));
            return;
        }
        sound.source.Stop();
    }

    public void StopAudioType(AudioType type)
    {
        Sound[] soundAll = Array.FindAll(sounds, sound => sound.audioType == type);
        if (soundAll == null)
        {
            return;
        }
        foreach (var sound in soundAll)
        {
            sound.source.Stop();
        }
    }

    public void StopAll()
    {
        foreach (Sound sound in sounds)
        {
            sound.source.Stop();
        }
    }

    private IEnumerator FadedOut(Sound sound)
    {
        float elapsedTime = 0;
        while (elapsedTime <= 1f)
        {
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
            sound.source.volume = Mathf.Lerp(1, 0, elapsedTime);
        }
        sound.source.Stop();
    }

    private IEnumerator FadedIn(Sound sound)
    {
        sound.source.Play();
        float elapsedTime = 0;
        while (elapsedTime <= 1f)
        {
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
            sound.source.volume = Mathf.Lerp(0, 1, elapsedTime);
        }

    }
}

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public string name;

    public AudioMixerGroup audioMixerGroup;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool loop;
    public bool isSingleUsed;
    public AudioType audioType;

    [HideInInspector]
    public AudioSource source;




}

public enum AudioType
{
    music,
    sfx,
    ambiance
}