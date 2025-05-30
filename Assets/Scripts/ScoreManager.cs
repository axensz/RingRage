using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    static int score = 0;
    static bool scoreLocked = false;
    [SerializeField] TMP_Text scoreText;

    void Awake() // Changed Start to Awake to ensure it's set up early
    {
        Debug.Log("ScoreManager Awake. Initializing score display.");
        Refresh(); 
    }

    public static void Add(int pts)
    {
        if (scoreLocked) return;
        score += pts;
        Debug.Log($"ScoreManager: Add called. New score: {score}");
        ScoreManager instance = FindFirstObjectByType<ScoreManager>();
        if (instance != null)
        {
            instance.Refresh();
        }
        else
        {
            Debug.LogError("ScoreManager: Instance not found. Cannot refresh score UI.");
        }
    }

    void Refresh()
    {
        Debug.Log($"ScoreManager: Refresh called. Current score: {score}");
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
            Debug.Log($"ScoreManager: scoreText updated to: {scoreText.text}");
        }
        else
        {
            Debug.LogError("ScoreManager: scoreText is not assigned in the Inspector!");
        }
    }

    public static void SaveScore()
    {
        // Cargar los puntajes actuales
        List<int> scores = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            scores.Add(PlayerPrefs.GetInt($"HighScore{i}", 0));
        }
        // Agregar el score actual
        scores.Add(score);
        // Ordenar y quedarnos con los 5 mejores
        scores.Sort((a, b) => b.CompareTo(a));
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt($"HighScore{i}", scores[i]);
        }
        PlayerPrefs.Save();
    }

    public static int[] GetTopScores()
    {
        int[] top = new int[5];
        for (int i = 0; i < 5; i++)
        {
            top[i] = PlayerPrefs.GetInt($"HighScore{i}", 0);
        }
        return top;
    }

    public static void ResetScore()
    {
        score = 0;
        UnlockScore();
        ScoreManager instance = FindFirstObjectByType<ScoreManager>();
        if (instance != null)
        {
            instance.Refresh();
        }
    }

    public static void LockScore() { scoreLocked = true; }
    public static void UnlockScore() { scoreLocked = false; }
    public static bool IsScoreLocked() { return scoreLocked; }
}
