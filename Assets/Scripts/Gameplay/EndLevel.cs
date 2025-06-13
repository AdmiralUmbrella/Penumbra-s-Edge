using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public AudioClip levelEndSound;
    public Image fadeOutImage;

    private AudioSource audioSource;
    private PlayerInput playerInput;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerInput = FindObjectOfType<PlayerInput>();

        // Configuración inicial de la imagen de fade out
        if (fadeOutImage != null)
        {
            fadeOutImage.gameObject.SetActive(false); // Inicialmente desactivada
        }
        else
        {
            Debug.LogError("Fade out image is not assigned in the inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(EndLevelSequence());
        }
    }

    private IEnumerator EndLevelSequence()
    {
        Debug.Log("EndLevelSequence started");

        // Desactivar los inputs del jugador
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }

        // Activar la imagen de fade out
        if (fadeOutImage != null)
        {
            fadeOutImage.gameObject.SetActive(true);
            fadeOutImage.color = Color.clear; // Completamente transparente
        }

        // Reproducir el sonido y realizar el fade out simultáneamente
        if (audioSource != null && levelEndSound != null)
        {
            audioSource.clip = levelEndSound;
            audioSource.Play();

            float elapsedTime = 0f;
            float fadeDuration = levelEndSound.length;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);
                if (fadeOutImage != null)
                {
                    fadeOutImage.color = Color.Lerp(Color.clear, Color.white, t);
                }
                yield return null;
            }

            // Asegurarse de que la imagen esté completamente blanca al final
            if (fadeOutImage != null)
            {
                fadeOutImage.color = Color.white;
            }

            // Esperar a que termine el audio
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }

        // Desactivar la imagen de fade out
        if (fadeOutImage != null)
        {
            fadeOutImage.gameObject.SetActive(false);
        }

        Debug.Log("Fade out and audio completed");

        // Cargar el siguiente nivel
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        Debug.Log($"LoadNextLevel called. Current scene: {currentSceneIndex}, Next scene: {nextSceneIndex}");

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (LoadingScene.Instance != null)
            {
                Debug.Log("Calling LoadingScene.Instance.LoadGame");
                LoadingScene.Instance.LoadGame(nextSceneIndex);
            }
            else
            {
                Debug.LogError("LoadingScene instance not found. Loading scene directly.");
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
        else
        {
            Debug.Log("¡Has completado todos los niveles!");
            // Aquí puedes añadir lógica para el final del juego
        }
    }
}