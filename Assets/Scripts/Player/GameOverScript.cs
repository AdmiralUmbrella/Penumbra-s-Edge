using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public PlayableDirector gameOverTimeline;
    private bool isGameOver = false;
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    public void TriggerGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            playerInput.DeactivateInput();
            StartGameOverSequence();
        }
    }

    private void StartGameOverSequence()
    {
        // Activa la Timeline de Game Over
        if (gameOverTimeline != null)
        {
            gameOverTimeline.Play();

            // Programar la recarga de la escena para cuando termine la timeline
            Invoke("ReloadCurrentScene", (float)gameOverTimeline.duration);
        }
        else
        {
            Debug.LogError("Game Over Timeline not assigned!");
            ReloadCurrentScene(); // Recargar inmediatamente si no hay timeline
        }
    }

    private void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (LoadingScene.Instance != null)
        {
            LoadingScene.Instance.LoadGame(currentSceneIndex);
        }
        else
        {
            Debug.LogError("LoadingScene singleton no encontrado. Cargando escena directamente.");
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}