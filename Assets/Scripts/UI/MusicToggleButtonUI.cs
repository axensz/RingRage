using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MusicToggleButtonUI : MonoBehaviour
{
    [Header("Sprites del Botón")]
    [Tooltip("Sprite para mostrar cuando la música se está reproduciendo (ej. icono de pausa).")]
    public Sprite musicPlayingSprite; // Ícono de Pausa

    [Tooltip("Sprite para mostrar cuando la música está pausada (ej. icono de play).")]
    public Sprite musicPausedSprite;  // Ícono de Play

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("MusicToggleButtonUI: No se encontró un componente Image en este GameObject. Asegúrate de que el botón tenga una Image.");
            enabled = false;
            return;
        }

        // Suscribirse al evento de cambio de estado de la música
        AudioManager.OnMusicStateChanged += UpdateButtonSprite;

        // Establecer el sprite inicial basado en el estado actual de la música
        if (AudioManager.instance != null)
        {
            UpdateButtonSprite(AudioManager.instance.IsPlaying);
        }
        else
        {
            Debug.LogWarning("MusicToggleButtonUI: AudioManager.instance no está disponible en Start. El sprite inicial podría no ser correcto.");
            // Opcionalmente, podrías intentar obtener el estado directamente del AudioSource si tienes una referencia
        }
    }

    void OnDestroy()
    {
        // Desuscribirse del evento para evitar errores
        AudioManager.OnMusicStateChanged -= UpdateButtonSprite;
    }

    /// <summary>
    /// Actualiza el sprite del botón según el estado de reproducción de la música.
    /// </summary>
    /// <param name="isPlaying">True si la música se está reproduciendo, false si está pausada.</param>
    void UpdateButtonSprite(bool isPlaying)
    {
        if (buttonImage == null) return;

        if (isPlaying)
        {
            if (musicPlayingSprite != null)
            {
                buttonImage.sprite = musicPlayingSprite;
            }
            else
            {
                // Opcional: Cambiar el texto del botón si no usas sprites
                // GetComponentInChildren<Text>()?.text = "Pausar"; 
            }
        }
        else
        {
            if (musicPausedSprite != null)
            {
                buttonImage.sprite = musicPausedSprite;
            }
            else
            {
                // Opcional: Cambiar el texto del botón si no usas sprites
                // GetComponentInChildren<Text>()?.text = "Play"; 
            }
        }
    }
}
