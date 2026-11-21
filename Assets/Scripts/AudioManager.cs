using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; 
    public AudioMixer mixer;

    [Header("Musica de Fondo")]
    public AudioSource musicSource; 
    public AudioClip menuMusic;     // Pista 1: Para menus (StartMenu, Ajustes...)
    public AudioClip gameplayMusic; // Pista 2: Para la escena de gameplay

    // El SFX de los botones se pondrá globalmente desde este script para no tener que ir uno por uno
    [Header("SFX de Botones")]
    public AudioSource sfxSource; // El AudioSource que está dentro del propio AudioManager
    public AudioClip buttonClickClip; // El sonido de click de los botones

    [Header("SFX de Partida")]
    public AudioClip bombClip;

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

        // Arranca con la musica de menus
        UpdateMusicForScene(SceneManager.GetActiveScene().name);
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignSoundToAllButtons();          // Asigna el sonido a todos los botones al cargar la nueva escena
        UpdateMusicForScene(scene.name);    // Actualiza la musica de fondo segun la escena   
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

    void UpdateMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        AudioClip targetClip = null;

        // Cambia la musica cuando se entra a la escena de gameplay
        if (sceneName == "Game")
        {
            targetClip = gameplayMusic;
        }
        else
        {
            targetClip = menuMusic;
        }

        // Solo se cambia si es distinta a la que ya esta puesta
        if (targetClip != null && musicSource.clip != targetClip)
        {
            musicSource.clip = targetClip;
            musicSource.Play();
        }
    }

    // Metodo generico para reproducir cualquier clip pasando la referencia
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Metodos publicos para reproducirlo en la escena de Gamepay
    public void PlayBombSFX() => PlaySFX(bombClip);

}
