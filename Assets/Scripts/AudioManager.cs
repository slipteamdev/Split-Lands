using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; 
    public AudioMixer mixer;

    // Se creará una instancia del AudioManager al principio y no se destruirá entre escenas (Singleton)
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Aplica el volumen guardado nada más abrir el juego
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        mixer.SetFloat("MusicVol", Mathf.Log10(savedMusic) * 20);
        mixer.SetFloat("SFXVol", Mathf.Log10(savedSFX) * 20);
    }
}