using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private void OnEnable()
    {
        ScoreManager.OnScoreUpdated += UpdateScoreUI;
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreUpdated -= UpdateScoreUI;
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    private void Start()
    {
        scoreText.enabled = true;
    }

    private void CollisionHandlerNew_OnPlayerCollision()
    {
        scoreText.enabled = false;
    }

    private void UpdateScoreUI(float newScore)
    {
        scoreText.text = Mathf.RoundToInt(newScore).ToString();
    }
}
