using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static event Action<float> OnScoreUpdated; // For updating UI dynamically
    public static ScoreManager Instance; // Singleton

    [Header("References")]
    [SerializeField] TextMeshProUGUI finalScoreText;

    [Header("Scoring Settings")]
    public float distanceMultiplier = 1f;   // Points per unit distance
    public float nearMissBase = 50f;        // Base points for near miss
    public float nearMissSpeedMultiplier = 2f; // Extra points based on speed
    public float overtakeBonus = 100f;      // Bonus for overtaking cars

    private float score = 0f;
    private float distanceTraveled = 0f;

    private bool isCountingScore;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    private void CollisionHandlerNew_OnPlayerCollision()
    {
        isCountingScore = false;

        finalScoreText.text = "Score: " + Mathf.RoundToInt(score).ToString(); ;
    }

    private void Start()
    {
        isCountingScore = true;
    }

    private void Update()
    {

        if (!isCountingScore) return;
        // Increase score based on distance traveled over time
        AddDistanceScore(Time.deltaTime);
    }

    // -------------------------
    //   SCORE ADDITION METHODS
    // -------------------------

    public void AddNearMiss(float playerSpeed)
    {
        float points = nearMissBase + (playerSpeed * nearMissSpeedMultiplier);
        AddScore(points);
    }

    public void AddOvertake()
    {
        AddScore(overtakeBonus);
    }

    public void AddDistanceScore(float deltaTime)
    {
        float points = deltaTime * distanceMultiplier;
        AddScore(points);
        distanceTraveled += deltaTime;
    }

    // -------------------------
    //   CORE ADD SCORE METHOD
    // -------------------------
    private void AddScore(float points)
    {
        score += points;
        OnScoreUpdated?.Invoke(score);
    }

    // -------------------------
    //   GETTERS
    // -------------------------
    public float GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0f;
        distanceTraveled = 0f;
        OnScoreUpdated?.Invoke(score);
    }
}
