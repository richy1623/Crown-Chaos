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

    private bool musicPaused = false;

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
        if (s == null)
        {
            return;
        }
        s.source.Play();
    }

    private void Update()
    {
        if ((!bgMusic[bgMusicIndex].source.isPlaying || !bgMusic[bgMusicIndex].source) && !musicPaused)
        {
            if (bgMusicIndex + 1 >= bgMusic.Length - 3)
            {
                bgMusicIndex = 0;
            }
            else
            {
                bgMusicIndex++;
            }

            bgMusic[bgMusicIndex].source.Play();
            Debug.Log("TRUE");
        }
        else if (!bgMusic[bgMusic.Length - 1].source.isPlaying && !bgMusic[bgMusic.Length - 2].source.isPlaying && musicPaused)
        {
            bgMusic[bgMusicIndex].source.UnPause();
            musicPaused = false;
            Debug.Log("FALSE");
        }
    }

    void loadVolume()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        mixer.SetFloat(MUSIC_KEY, Mathf.Log10(musicVolume) * 20);
        mixer.SetFloat(SFX_KEY, Mathf.Log10(sfxVolume) * 20);
    }

    public void playDefeat()
    {
        musicPaused = true;
        bgMusic[bgMusicIndex].source.Pause();
        bgMusic[bgMusic.Length - 2].source.Play();
    }

    public void playVictory() {
        musicPaused = true;
        bgMusic[bgMusicIndex].source.Pause();
        bgMusic[bgMusic.Length - 1].source.Play();
    }

    public void interruptEndMusic()
    {
        if (bgMusic[bgMusic.Length - 1].source.isPlaying && musicPaused)
        {
            bgMusic[bgMusic.Length - 1].source.Stop();
            bgMusic[bgMusicIndex].source.UnPause();
            musicPaused = false;
        }
        else if (bgMusic[bgMusic.Length - 2].source.isPlaying && musicPaused)
        {
            bgMusic[bgMusic.Length - 2].source.Stop();
            bgMusic[bgMusicIndex].source.UnPause();
            musicPaused = false;
        }
    }
}
