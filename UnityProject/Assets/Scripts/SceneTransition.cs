using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public Canvas currentCanvas;
    public Canvas newCanvas;
    public void LoadScene(string sceneName)
    {
        // Load the specified scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void SwitchCanvas()
    {
        // Deactivate the GameObject this script is attached to
        if (currentCanvas != null) currentCanvas.gameObject.SetActive(false);
        newCanvas.gameObject.SetActive(true);
    }
}
