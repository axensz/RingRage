using UnityEngine;
using UnityEngine.SceneManagement;

public class EscenasManager : MonoBehaviour
{
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelCreditos;
    [SerializeField] private GameObject panelModosDeJuego;

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

    public void ReturnToMainMenu() // Changed from GameToMenu
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Reinicia la escena actual. Ideal para un botón de "Retry".
    /// </summary>
    public void RetryGame()
    {
        // Recarga la escena que está actualmente activa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
