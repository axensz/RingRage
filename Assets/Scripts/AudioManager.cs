using UnityEngine;
using System; // Necesario para System.Action

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Tooltip("Arrastra aquí el AudioSource que reproduce la música (ej. el de la Main Camera).")]
    public AudioSource musicSource;

    // Evento que se dispara cuando el estado de la música cambia (true si se está reproduciendo, false si está pausada/detenida)
    public static event Action<bool> OnMusicStateChanged;

    // Propiedad para saber si la música se está reproduciendo actualmente
    public bool IsPlaying => musicSource != null && musicSource.isPlaying;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Opcional: Descomenta la siguiente línea si quieres que el AudioManager persista entre escenas
            // DontDestroyOnLoad(gameObject); 
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
        {
            Debug.LogError("AudioManager: ¡MusicSource no ha sido asignado en el Inspector!");
            // Intenta encontrarlo en la Main Camera como alternativa
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); // Asegúrate de que tu Main Camera tenga el tag "MainCamera"
            if (mainCamera != null)
            {
                musicSource = mainCamera.GetComponent<AudioSource>();
                if (musicSource != null)
                {
                    Debug.Log("AudioManager: MusicSource encontrado en MainCamera mediante tag.");
                }
                else
                {
                    Debug.LogError("AudioManager: MainCamera encontrada, pero no tiene un componente AudioSource.");
                    enabled = false; // Deshabilita el script si no hay fuente de música
                    return;
                }
            }
            else
            {
                 Debug.LogError("AudioManager: MusicSource no asignado y MainCamera con tag 'MainCamera' no encontrada.");
                 enabled = false; // Deshabilita el script si no hay fuente de música
                 return;
            }
        }
    }

    /// <summary>
    /// Alterna entre pausar y reproducir la música.
    /// </summary>
    public void ToggleMusic()
    {
        if (musicSource == null)
        {
            Debug.LogError("AudioManager: No se puede alternar la música, MusicSource no está disponible.");
            return;
        }

        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            Debug.Log("Música Pausada");
        }
        else
        {
            musicSource.Play();
            Debug.Log("Música Reproducida/Resumida");
        }
        // Disparar el evento con el nuevo estado
        OnMusicStateChanged?.Invoke(musicSource.isPlaying);
    }

    // Métodos opcionales para control más específico si los necesitas después:
    public void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
