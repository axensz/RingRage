using UnityEngine;
using UnityEngine.SceneManagement;

public class EscenasManager : MonoBehaviour
{
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelCreditos;
    [SerializeField] private GameObject panelModosDeJuego;
    [SerializeField] private GameObject panelPuntaje; // Nuevo panel para puntajes

    // La función Game() puede ser usada por los botones 1vs1 y Training
    public void StartGame() // Changed from Game
    {
        SceneManager.LoadScene("FightScene");
    }

    public void ShowCreditsPanel() // Changed from Creditos
    {
        panelMenu.SetActive(false);
        panelCreditos.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreditsPanelToMenu() // Changed from CreditosToMenu
    {
        panelCreditos.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void ShowGameModesPanel() // Changed from MostrarModosDeJuego
    {
        panelMenu.SetActive(false);
        panelModosDeJuego.SetActive(true);

    }

    public void GameModesPanelToMenu() // Changed from ModosDeJuegoToMenu
    {
        panelModosDeJuego.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void ShowPuntajePanel()
    {
        // Detener movimiento de todos los jugadores
        foreach (var move in FindObjectsByType<EntityMovement2D>(FindObjectsSortMode.None))
        {
            move.enabled = false;
        }
        // Pausar el timer si existe
        var timer = FindFirstObjectByType<TimerUI>();
        if (timer != null) timer.PauseTimer();
        // Mostrar solo el panel de puntaje
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelPuntaje != null) panelPuntaje.SetActive(true);
    }

    public void PuntajePanelToMenu()
    {
        if (panelPuntaje != null) panelPuntaje.SetActive(false);
        if (panelMenu != null) panelMenu.SetActive(true);
    }

    public void ReturnToMainMenu() // Changed from GameToMenu
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Llama esto cuando termine la pelea
    public void ShowPanelFinish()
    {
        // Deshabilitar todos los scripts de los jugadores
        foreach (var move in UnityEngine.Object.FindObjectsByType<EntityMovement2D>(FindObjectsSortMode.None))
        {
            foreach (var script in move.GetComponents<MonoBehaviour>())
            {
                script.enabled = false;
            }
        }
        // Pausar el timer si existe
        var timer = UnityEngine.Object.FindFirstObjectByType<TimerUI>();
        if (timer != null) timer.PauseTimer();
        // Buscar paneles por nombre para evitar ambigüedad
        var menu = GameObject.Find("PanelMenu");
        var puntaje = GameObject.Find("PanelPuntaje");
        if (menu != null) menu.SetActive(false);
        if (puntaje != null) puntaje.SetActive(true);
    }

    public void RetryGame()
    {
        var timer = UnityEngine.Object.FindFirstObjectByType<TimerUI>();
        if (timer != null) timer.ResetTimer();
        // Recarga la escena que está actualmente activa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
