using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public const string MUSIC_KEY = "musicVolume";
    public const string SFX_KEY = "sfxVolume";

    public Sound[] sounds;
    public Sound[] bgMusic;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    public int bgMusicIndex;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;    
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        loadVolume();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxGroup;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

        foreach (Sound m in bgMusic)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.outputAudioMixerGroup = musicGroup;
            m.source.clip = m.clip;
            m.source.volume = m.volume;
            m.source.pitch = m.pitch;
        }

        bgMusicIndex = 0;
        bgMusic[bgMusicIndex].source.Play();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            return;
        }
        s.source.Play();
    }

    private void Update()
    {
        if (!bgMusic[bgMusicIndex].source.isPlaying || !bgMusic[bgMusicIndex].source)
        {
            if (bgMusicIndex + 1 == bgMusic.Length - 1)
            {
                bgMusicIndex = 0;
            }
            else
            {
                bgMusicIndex++;
            }

            bgMusic[bgMusicIndex].source.Play();
        }
    }

    void loadVolume()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        mixer.SetFloat(MUSIC_KEY, Mathf.Log10(musicVolume) * 20);
        mixer.SetFloat(SFX_KEY, Mathf.Log10(sfxVolume) * 20);
    }
}
