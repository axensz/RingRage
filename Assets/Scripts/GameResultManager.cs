using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class GameResultManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private string playerTag = "Player"; // Tag para identificar al jugador

    [Header("Enemy References")]
    [SerializeField] private Health enemyHealth; // Puedes expandir esto a una lista o buscar por tag si hay múltiples enemigos
    [SerializeField] private string enemyTag = "Enemy"; // Tag para identificar a los enemigos

    [Header("UI References")]
    [SerializeField] private GameObject gameResultPanel; // El panel que contiene el texto del resultado
    [SerializeField] private TextMeshProUGUI resultText; // El componente de texto para mostrar "Has Ganado" / "Has Perdido"

    private bool gameEnded = false;

    void Start()
    {
        // Intentar encontrar automáticamente si no están asignados
        if (playerHealth == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                playerHealth = playerObject.GetComponent<Health>();
            }
        }

        if (enemyHealth == null)
        {
            GameObject enemyObject = GameObject.FindGameObjectWithTag(enemyTag);
            if (enemyObject != null)
            {
                enemyHealth = enemyObject.GetComponent<Health>();
            }
        }
        
        // Suscribirse a los eventos OnDeath
        if (playerHealth != null)
        {
            playerHealth.OnDeath.AddListener(HandlePlayerDeath);
        }
        else
        {
            Debug.LogError("PlayerHealth no está asignado en GameResultManager y no se pudo encontrar por tag.");
        }

        if (enemyHealth != null)
        {
            enemyHealth.OnDeath.AddListener(HandleEnemyDeath);
        }
        else
        {
            Debug.LogError("EnemyHealth no está asignado en GameResultManager y no se pudo encontrar por tag.");
        }

        // Asegurarse de que el panel de resultado esté desactivado al inicio
        if (gameResultPanel != null)
        {
            gameResultPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("GameResultPanel no está asignado en GameResultManager.");
        }

        if (resultText == null && gameResultPanel != null)
        {
            resultText = gameResultPanel.GetComponentInChildren<TextMeshProUGUI>();
             if (resultText == null)
             {
                Debug.LogError("ResultText (TextMeshProUGUI) no está asignado y no se pudo encontrar como hijo de GameResultPanel.");
             }
        }
    }

    void OnDestroy()
    {
        // Desuscribirse para evitar errores si este objeto se destruye antes que los Health
        if (playerHealth != null)
        {
            playerHealth.OnDeath.RemoveListener(HandlePlayerDeath);
        }
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath.RemoveListener(HandleEnemyDeath);
        }
    }

    void HandlePlayerDeath()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("Player Died - Result: Has Perdido");
        ShowResult("Has Perdido");
    }

    void HandleEnemyDeath()
    {
        if (gameEnded) return;
        
        // Aquí podrías añadir lógica para verificar si todos los enemigos han muerto si tienes más de uno.
        // Por ahora, asumimos que la muerte de este 'enemyHealth' significa que el jugador ha ganado.
        gameEnded = true; 
        Debug.Log("Enemy Died - Result: Has Ganado");
        ShowResult("Has Ganado");
    }

    void ShowResult(string message)
    {
        if (gameResultPanel != null && resultText != null)
        {
            resultText.text = message;
            gameResultPanel.SetActive(true);
            
            // Opcional: Pausar el juego
            // Time.timeScale = 0f; 
        }
        else
        {
            Debug.LogError("No se puede mostrar el resultado: gameResultPanel o resultText no están asignados.");
        }
    }
}
