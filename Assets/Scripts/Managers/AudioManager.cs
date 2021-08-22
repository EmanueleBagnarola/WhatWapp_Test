using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public Sound[] Sounds = null;

    public static AudioManager Instance = null;

    private List<float> _audioVolumes = new List<float>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else Destroy(this);

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.loop = s.Loop;
            s.Source.playOnAwake = s.PlayOnAwake;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
        }
    }

    private void Start()
    {
        Play("MusicLoop");

        DontDestroyOnLoad(this);

        RegisterStartingAudioVolumes();
    }

    private void RegisterStartingAudioVolumes()
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            Sound s = Sounds[i];
            _audioVolumes.Add(s.Source.volume);
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        else
        {
            if (s.Source.isPlaying)
            {
                return;
            }

            s.Source.Play();
        }
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (s.Source.isPlaying)
        {
            s.Source.Stop();
        }

        s.Source.PlayOneShot(s.Clip, 1f);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.Source.Stop();
    }

    public void MuteSFX(bool v)
    {
        List<float> startingVolumes = new List<float>();
        for (int i = 0; i < Sounds.Length; i++)
        {
            Sound s = Sounds[i];
            startingVolumes.Add(s.Source.volume);

            if (s.Name.Contains("SFX"))
            {
                if (v == true)
                {
                    s.Source.volume = 0;
                }
                else
                {
                    s.Source.volume = startingVolumes[i];
                }
            }
        }
    }

    public void MuteMusic(bool v)
    {
        List<float> startingVolumes = new List<float>();
        for (int i = 0; i < Sounds.Length; i++)
        {
            Sound s = Sounds[i];
            startingVolumes.Add(s.Source.volume);

            if (!s.Name.Contains("SFX"))
            {
                if(v == true)
                {
                    s.Source.volume = 0;
                }
                else
                {
                    s.Source.volume = startingVolumes[i];
                }
            }
        }
    }

    public void MuteAll(bool v)
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            Sound s = Sounds[i];

            if (v == true)
            {
                s.Source.volume = 0;
            }
            else
            {
                s.Source.volume = _audioVolumes[i];
            }
        }
    }

    private void Pause(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.Source.Pause();
    }

    private void UnPause(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.Source.UnPause();
    }
}
