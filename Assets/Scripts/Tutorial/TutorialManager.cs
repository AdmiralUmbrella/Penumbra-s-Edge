using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel negro que se usará para el fade")]
    public Image fadePanel;

    [Tooltip("Texto que mostrará las instrucciones")]
    public TMP_Text tutorialText;

    [Header("Tutorial Settings")]
    [Tooltip("Duración del fade in/out en segundos")]
    public float fadeDuration = 1.5f;

    [Tooltip("Tiempo que cada mensaje permanece en pantalla")]
    public float messageDisplayTime = 4f;

    [Header("Tutorial Messages")]
    [Tooltip("Mensajes que se mostrarán en secuencia")]
    public string[] tutorialMessages = {
        "Usa WASD para moverte...",
        "Mantén click izquierdo para encender la linterna...",
        "No dejes que los desolladores te vean...",
        "Presiona E para interactuar..."
    };

    private PlayerInput playerInput;
    private PlayerController playerController;

    private void Start()
    {
        // Obtener referencias necesarias
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();

        // Asegurarnos que los componentes UI estén configurados correctamente
        SetupUIComponents();

        // Iniciar la secuencia del tutorial
        StartCoroutine(TutorialSequence());
    }

    private void SetupUIComponents()
    {
        // Configurar el panel de fade
        if (fadePanel != null)
        {
            fadePanel.color = Color.black;
            fadePanel.gameObject.SetActive(true);
        }

        // Configurar el texto del tutorial
        if (tutorialText != null)
        {
            tutorialText.alpha = 0f;
            tutorialText.gameObject.SetActive(true);
        }
    }

    private IEnumerator TutorialSequence()
    {
        // Desactivar el input del jugador inicialmente
        if (playerInput != null) playerInput.DeactivateInput();
        if (playerController != null) playerController.enabled = false;

        // Esperar un momento antes de empezar
        yield return new WaitForSeconds(1f);

        // Realizar el fade out del negro
        yield return StartCoroutine(FadeOut());

        // Activar el input del jugador
        if (playerInput != null) playerInput.ActivateInput();
        if (playerController != null) playerController.enabled = true;

        // Mostrar cada mensaje del tutorial
        foreach (string message in tutorialMessages)
        {
            yield return StartCoroutine(ShowTutorialMessage(message));
        }

        // Limpiar la UI del tutorial
        CleanupTutorialUI();
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = Color.black;
        Color endColor = new Color(0f, 0f, 0f, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeDuration;

            if (fadePanel != null)
            {
                fadePanel.color = Color.Lerp(startColor, endColor, normalizedTime);
            }

            yield return null;
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowTutorialMessage(string message)
    {
        if (tutorialText != null)
        {
            // Fade in del texto
            tutorialText.text = message;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                tutorialText.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }

            // Mantener el mensaje visible
            yield return new WaitForSeconds(messageDisplayTime);

            // Fade out del texto
            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                tutorialText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                yield return null;
            }

            // Pequeña pausa entre mensajes
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void CleanupTutorialUI()
    {
        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(false);
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false);
        }
    }
}