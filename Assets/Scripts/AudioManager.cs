using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; 
    public AudioMixer mixer;

    // El SFX de los botones se pondrá globalmente desde este script para no tener que ir uno por uno
    [Header("SFX de Botones")]
    public AudioSource sfxSource; // El AudioSource que está dentro del propio AudioManager
    public AudioClip buttonClickClip; // El sonido de click de los botones

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

        AssignSoundToAllButtons();  // Init en primera escena
    }

    /// Para aplicar sonido a todos los botones, el AudioManager se subscribirá 
    /// cada vez que se abre una escena para recibir los eventos onClick de los botones nuevos
    
    //Subscribir AudioManager

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Desubscribir AudioManager

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Asigna el sonido a todos los botones al cargar la nueva escena
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignSoundToAllButtons();
    }

    // Busca todos los botones de la escena actual y les añade el sonido al hacer click
    public void AssignSoundToAllButtons()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include);

        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(PlayButtonSound); // Para evitar duplicados 
            btn.onClick.AddListener(PlayButtonSound);    // AudioManager se subscribe al evento onClick
        }
    }

    public void PlayButtonSound()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            sfxSource.PlayOneShot(buttonClickClip);
        }
    }

}
