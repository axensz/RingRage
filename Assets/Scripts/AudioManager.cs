using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource; // Asigna esto en el Inspector

    public bool IsPlaying => musicSource != null && musicSource.isPlaying;

    public static event Action<bool> OnMusicStateChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: para que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
            {
                Debug.LogError("AudioManager: No AudioSource component found on this GameObject. Please add one.");
                enabled = false; // Deshabilitar si no hay AudioSource
            }
        }
    }

    void Start()
    {
        // Opcional: Iniciar la música automáticamente
        // PlayMusic(); 
    }

    public void ToggleMusic()
    {
        if (musicSource == null) return;

        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
        else
        {
            musicSource.Play();
        }
        OnMusicStateChanged?.Invoke(musicSource.isPlaying);
    }

    public void PlayMusic()
    {
        if (musicSource == null) return;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
            OnMusicStateChanged?.Invoke(true);
        }
    }

    public void PauseMusic()
    {
        if (musicSource == null) return;

        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            OnMusicStateChanged?.Invoke(false);
        }
    }

    // Puedes añadir métodos para SFX aquí también si lo necesitas
}