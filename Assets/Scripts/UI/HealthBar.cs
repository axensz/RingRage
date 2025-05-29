using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Slider

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Assign this in the Inspector
    public Health playerHealth; // Assign the Player's Health component here in the Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (healthSlider == null)
        {
            Debug.LogError("Health Slider not assigned in HealthBar script!");
            enabled = false; // Disable the script if slider is not set
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogError("Player Health component not assigned in HealthBar script!");
            enabled = false;
            return;
        }

        // Initialize the slider
        healthSlider.maxValue = playerHealth.MaxHP; // Use the public MaxHP property from Health.cs
        healthSlider.value = playerHealth.CurrentHP;
        healthSlider.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth != null && healthSlider != null)
        {
            // Update the slider's value to the player's current health
            healthSlider.value = playerHealth.CurrentHP;
        }
    }
}
