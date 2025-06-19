using UnityEngine;
using System.Collections;

public class UIButtonSound : MonoBehaviour
{
    public GameObject soundSource;
    public GameObject menuCanvas;
    public bool isQuitButton = false; // Flag für Quit

    public void onClick()
    {
        if (soundSource != null)
            AkUnitySoundEngine.PostEvent("Play_click_sound", soundSource);
        else
            AkUnitySoundEngine.PostEvent("Play_click_sound", gameObject);

        StartCoroutine(DoAfterSound());
    }

    private IEnumerator DoAfterSound()
    {
        yield return new WaitForSeconds(0.1f);

        if (isQuitButton)
        {
            Application.Quit();
            Debug.Log("Quits game");
        }
        else if (menuCanvas != null)
        {
            menuCanvas.GetComponent<SceneTransition>().SwitchCanvas();
        }
    }
}