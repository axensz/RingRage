using UnityEngine;
using UnityEngine.SceneManagement;

public class EscenasManager : MonoBehaviour
{
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelCreditos;

    public void Game()
    {
        SceneManager.LoadScene("FightScene");
    }

    public void Creditos()
    {
        panelMenu.SetActive(false);
        panelCreditos.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreditosToMenu()
    {
        panelCreditos.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void GameToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
