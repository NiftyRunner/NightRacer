using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float fadeDuration = 1f; // Fade duration

    [Header("References")]
    [SerializeField] UIFader fader; // Assign in inspector

    private bool isLoading = false;

    private void Start()
    {
        if (fader != null)
        {
            fader.InstantFadeIn(); // Scene starts fully visible
            fader.FadeOut(fadeDuration); // Fade out from black
        }
    }

    public void CallLoadCoroutine(string nextScene)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneWithFade(nextScene));
        }
    }

    private System.Collections.IEnumerator LoadSceneWithFade(string nextScene)
    {
        isLoading = true;

        // Fade in to black
        fader.FadeIn(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        // Load the next scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene);
        asyncLoad.allowSceneActivation = false;

        // Wait one frame so the scene is loaded
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        fader.FadeOut(fadeDuration);
        asyncLoad.allowSceneActivation = true;

        yield return null;

        //// Fade out to reveal the new scene
        //UIFader newFader = FindFirstObjectByType<UIFader>();
        //if (newFader != null)
        //{
        //    newFader.InstantFadeIn();
        //    newFader.FadeOut(fadeDuration);
        //}
    }
}
