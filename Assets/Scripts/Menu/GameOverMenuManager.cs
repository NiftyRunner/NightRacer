using UnityEngine;

public class GameOverMenuManager : MonoBehaviour
{
    [SerializeField] SceneLoader loader;
    [SerializeField] string menuSceneName;
    [SerializeField] string gameSceneName;

    public void RestartGame()
    {
        loader.CallLoadCoroutine(gameSceneName);
    }

    public void LoadMenu()
    {
        loader.CallLoadCoroutine(menuSceneName);
    }
}
