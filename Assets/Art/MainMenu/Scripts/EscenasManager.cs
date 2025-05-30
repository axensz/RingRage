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
        if (panelMenu != null)
        {
            panelMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("panelMenu is not assigned in EscenasManager.");
        }

        if (panelModosDeJuego != null)
        {
            panelModosDeJuego.SetActive(true);
        }
        else
        {
            Debug.LogError("panelModosDeJuego is not assigned in EscenasManager. Please assign it in the Inspector.");
        }
    }

    public void GameModesPanelToMenu() // Changed from ModosDeJuegoToMenu
    {
        if (panelModosDeJuego != null)
        {
            panelModosDeJuego.SetActive(false);
        }
        else
        {
            Debug.LogError("panelModosDeJuego is not assigned in EscenasManager. Please assign it in the Inspector.");
        }

        if (panelMenu != null)
        {
            panelMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("panelMenu is not assigned in EscenasManager.");
        }
    }

    public void ReturnToMainMenu() // Changed from GameToMenu
    {
        SceneManager.LoadScene("MainMenu");
    }
}
