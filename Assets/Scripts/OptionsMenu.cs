using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMPro.TMP_Dropdown qualityDropdown;
    [SerializeField] private TMPro.TMP_Dropdown difficultyDropdown;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    Resolution[] resolutions;

    public static string DIFFICULTY_KEY = "difficulty";

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(setMusicVolume);
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && 
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        qualityDropdown.value = QualitySettings.GetQualityLevel();

        difficultyDropdown.value = PlayerPrefs.GetInt(DIFFICULTY_KEY, 0);

        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 1f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.SFX_KEY, sfxSlider.value);
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
    }
    public void setSFXVolume(float volume)
    {
        audioMixer.SetFloat(AudioManager.SFX_KEY, Mathf.Log10(volume) * 20);
    }

    public void setMusicVolume(float volume)
    {
        audioMixer.SetFloat(AudioManager.MUSIC_KEY, Mathf.Log10(volume) * 20);
    }

    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void setDifficulty(int difficultyIndex)
    {
        PlayerPrefs.SetInt(DIFFICULTY_KEY, difficultyIndex);
    }
}
