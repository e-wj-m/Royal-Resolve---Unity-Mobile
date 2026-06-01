using UnityEngine;

public class ActivateSceneTransition : MonoBehaviour
{
    [SerializeField] private string targetScene;

    public void Apply()
    {
        if (SceneTransition.Instance == null)
        {
            Debug.LogWarning("[ActivateSceneTransition] No SceneTransition instance found.");
            return;
        }

        SceneTransition.Instance.LoadScene(targetScene);
    }
}
