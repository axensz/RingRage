using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider staminaSlider; // Asigna tu Slider de Stamina aquí en el Inspector
    public EntityMovement2D playerMovement; // Asigna el componente EntityMovement2D del jugador

    void Start()
    {
        if (staminaSlider == null)
        {
            Debug.LogError("Stamina Slider no asignado en StaminaBar script!");
            enabled = false;
            return;
        }
        if (playerMovement == null)
        {
            // Intenta encontrarlo si no está asignado y el jugador tiene un tag específico
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMovement = player.GetComponent<EntityMovement2D>();
            }
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement no asignado en StaminaBar script y no se pudo encontrar jugador con tag 'Player'!");
                enabled = false;
                return;
            }
        }

        staminaSlider.maxValue = playerMovement.maxStamina;
        staminaSlider.value = playerMovement.currentStamina;
        staminaSlider.minValue = 0;
    }

    void Update()
    {
        if (playerMovement != null && staminaSlider != null)
        {
            // Asegúrate que maxValue esté actualizado si puede cambiar dinámicamente (aunque usualmente no lo hace)
            if (staminaSlider.maxValue != playerMovement.maxStamina)
            {
                staminaSlider.maxValue = playerMovement.maxStamina;
            }
            staminaSlider.value = playerMovement.currentStamina;
        }
    }
}
