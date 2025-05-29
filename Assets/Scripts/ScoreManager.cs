using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    static int score;
    [SerializeField] TMP_Text scoreText;

    void Start() => Refresh();

    public static void Add(int pts)
    {
        score += pts;
        FindFirstObjectByType<ScoreManager>()?.Refresh();
    }

    void Refresh()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }
}
