using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    static int score;
    [SerializeField] TMP_Text scoreText;

    void Awake() // Changed Start to Awake to ensure it's set up early
    {
        Debug.Log("ScoreManager Awake. Initializing score display.");
        Refresh(); 
    }

    public static void Add(int pts)
    {
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
}
