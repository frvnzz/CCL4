using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerInput uiInput;

    private bool isPaused = false;

    void Awake()
    {

    }

    void OnEnable()
    {
        var actions = uiInput.actions;
        actions["Pause"].performed += OnPaused;
    }

    void OnDisable()
    {
        var actions = uiInput.actions;
        actions["Pause"].performed -= OnPaused;
    }

    public void OnPaused(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        playerInput.DeactivateInput();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;

        HUD.instance.ShowPauseMenu(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        playerInput.ActivateInput();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;

        HUD.instance.ShowPauseMenu(false);
    }
}