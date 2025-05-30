using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Opcional: Métodos para pausar/reanudar el timer
    public void PauseTimer() => isRunning = false;
    public void ResumeTimer() => isRunning = true;
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
