using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Cargar los valores guardados al entrar en ajustes
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderValue); //???
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
