using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EscenasManager : MonoBehaviour
{
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelCreditos;
    [SerializeField] private GameObject panelModosDeJuego;
    [SerializeField] private GameObject panelPuntaje; // Nuevo panel para puntajes
    [SerializeField] private TMP_Text highScoresText; // Asigna este campo en el inspector
    [SerializeField] private TMP_Text secretText; // Asigna el TMP_Text a monitorear
    [SerializeField] private GameObject secretImage; // Imagen a mostrar
    [SerializeField] private AudioClip secretSound; // Sonido a reproducir
    [SerializeField] private AudioSource audioSource; // AudioSource para reproducir el sonido
    private int secretClickCount = 0;
    private float lastClickTime = 0f;
    private float clickResetTime = 2f; // Tiempo máximo entre clicks

    // La función Game() puede ser usada por los botones 1vs1 y Training
    public void StartGame() // Changed from Game
    {
        ScoreManager.ResetScore();
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
        // ShowHighScores(); // Ya no se muestra aquí
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
        // ShowHighScores(); // Ya no se muestra aquí
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
        ShowHighScores();
    }

    public void PuntajePanelToMenu()
    {
        if (panelPuntaje != null) panelPuntaje.SetActive(false);
        if (panelMenu != null) panelMenu.SetActive(true);
        // ShowHighScores(); // Ya no se muestra aquí
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
        // Guardar el puntaje al finalizar la partida
        ScoreManager.SaveScore();
    }

    public void RetryGame()
    {
        var timer = UnityEngine.Object.FindFirstObjectByType<TimerUI>();
        if (timer != null) timer.ResetTimer();
        // Recarga la escena que está actualmente activa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowHighScores()
    {
        int[] top = ScoreManager.GetTopScores();
        string tabla = "TOP 5 PUNTAJES\n";
        for (int i = 0; i < top.Length; i++)
        {
            tabla += $"{i + 1}. {top[i]}\n";
        }
        if (highScoresText != null)
            highScoresText.text = tabla;
    }

    void Start()
    {
        if (secretImage != null) secretImage.SetActive(false);
        if (secretText != null)
        {
            secretText.GetComponent<UnityEngine.UI.Button>()?.onClick.AddListener(OnSecretTextClick);
        }
    }

    public void OnSecretTextClick()
    {
        if (Time.time - lastClickTime > clickResetTime) secretClickCount = 0;
        lastClickTime = Time.time;
        secretClickCount++;
        if (secretClickCount >= 5)
        {
            if (secretImage != null) secretImage.SetActive(true);
            if (audioSource != null && secretSound != null) audioSource.PlayOneShot(secretSound);
            secretClickCount = 0;
        }
    }
}
