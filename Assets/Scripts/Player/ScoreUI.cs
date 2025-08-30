using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        ScoreManager.OnScoreUpdated += UpdateScoreUI;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreUpdated -= UpdateScoreUI;
    }

    private void UpdateScoreUI(float newScore)
    {
        scoreText.text = Mathf.RoundToInt(newScore).ToString();
    }
}
