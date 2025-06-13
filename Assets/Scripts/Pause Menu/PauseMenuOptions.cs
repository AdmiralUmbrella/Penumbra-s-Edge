using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuOptions : MonoBehaviour
{
    public GameObject settingsMenu;
    private LoadingScene loadingScene;
    public PlayerController playerController;

    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        loadingScene = FindObjectOfType<LoadingScene>();
    }
    public void Resume()
    {
        playerController.gameIsPaused = false;
        playerInput.ActivateInput();
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }

    public void Settings()
    {
        gameObject.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        loadingScene.LoadGame(0);
    }

    public void HideSettings()
    {
        settingsMenu.SetActive(false);
        gameObject.SetActive(true);
    }
}
