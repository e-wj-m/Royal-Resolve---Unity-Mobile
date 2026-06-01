using UnityEngine;

public class GameUIActions : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";

    public void ReturnToMenu()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(menuSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
