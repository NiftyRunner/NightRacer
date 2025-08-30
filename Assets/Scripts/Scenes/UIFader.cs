using UnityEngine;
using System;
using System.Collections;

public class UIFader : MonoBehaviour
{
    public static UIFader Instance;
    private CanvasGroup canvasGroup;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Kill the duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Try to find CanvasGroup and SpriteRenderer on this GameObject
        canvasGroup = GetComponent<CanvasGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (canvasGroup == null && spriteRenderer == null)
        {
            Debug.LogError("FadeInOut requires either a CanvasGroup or SpriteRenderer component.");
            enabled = false;
        }


    }

    /// <summary>
    /// Fades the object in or out.
    /// </summary>
    public void Fade(bool fadeIn, float duration = 1f, Action onComplete = null)
    {
        StopAllCoroutines(); // Prevent overlapping fades
        StartCoroutine(FadeRoutine(fadeIn, duration, onComplete));
    }

    /// <summary>
    /// Fades in (alpha → 1)
    /// </summary>
    public void FadeIn(float duration = 1f, Action onComplete = null)
    {
        Fade(true, duration, onComplete);
    }

    /// <summary>
    /// Fades out (alpha → 0)
    /// </summary>
    public void FadeOut(float duration = 1f, Action onComplete = null)
    {
        Fade(false, duration, onComplete);
    }

    /// <summary>
    /// Instantly sets alpha to 1
    /// </summary>
    public void InstantFadeIn()
    {
        SetAlpha(1f);
    }

    /// <summary>
    /// Instantly sets alpha to 0
    /// </summary>
    public void InstantFadeOut()
    {
        SetAlpha(0f);
    }

    private IEnumerator FadeRoutine(bool fadeIn, float duration, Action onComplete)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        // Special case: Instant fade
        if (duration <= 0f)
        {
            SetAlpha(endAlpha);
            onComplete?.Invoke();
            yield break;
        }

        SetAlpha(startAlpha);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetAlpha(currentAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAlpha(endAlpha);
        onComplete?.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.interactable = (alpha >= 1f);
            canvasGroup.blocksRaycasts = (alpha >= 1f);
        }
        else if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
    }
}
