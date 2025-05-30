using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonClickFeedbackUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Sprites de Interacción")]
    [Tooltip("Sprite normal del botón (cuando no está presionado).")]
    public Sprite normalSprite;

    [Tooltip("Sprite para mostrar cuando el botón está siendo presionado.")]
    public Sprite pressedSprite;

    private Image buttonImage;
    private bool isPointerOver = false;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("ButtonClickFeedbackUI: No se encontró un componente Image en este GameObject.");
            enabled = false;
            return;
        }

        // Establecer el sprite normal al inicio
        if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
        else
        {
            // Si no se asigna un sprite normal, usa el que ya tiene la imagen como normal.
            normalSprite = buttonImage.sprite;
            if (normalSprite == null)
            {
                Debug.LogWarning("ButtonClickFeedbackUI: No hay un sprite normal asignado ni un sprite inicial en el componente Image.");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null && pressedSprite != null)
        {
            buttonImage.sprite = pressedSprite;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
        // Si el puntero salió mientras estaba presionado y luego se soltó fuera, 
        // OnPointerExit ya debería haber restaurado el sprite normal si isPointerOver es true.
        // Si se soltó sobre el botón, esto lo restaura.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        // Si el botón fue presionado y el puntero sale de él ANTES de soltar el clic,
        // restauramos al sprite normal para que no se quede "presionado".
        if (buttonImage != null && normalSprite != null && buttonImage.sprite == pressedSprite)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    // Opcional: Si quieres que el sprite normal se fuerce al habilitar el script
    // void OnEnable()
    // {
    //     if (buttonImage != null && normalSprite != null)
    //     {
    //         buttonImage.sprite = normalSprite;
    //     }
    // }
}
