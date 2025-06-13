using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SaveZoneManager : MonoBehaviour
{
    // Existing variables for interaction
    public float interactionRadius = 2f;
    public PlayerController playerController;
    private bool playerInRange = false;

    // New variables for save notification
    [Header("Save Notification")]
    public TMP_Text saveNotificationText;    // Reference to the TMP text component
    public float notificationDuration = 2f;         // How long the notification stays visible
    public float fadeDuration = 0.5f;              // How long the fade animation takes

    private void Start()
    {
        // Find the player controller if not assigned
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        // Ensure the notification text is hidden at start
        if (saveNotificationText != null)
        {
            saveNotificationText.alpha = 0f;
        }
    }

    private void Update()
    {
        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        if (playerController != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);

            if (distanceToPlayer <= interactionRadius && !playerInRange)
            {
                playerInRange = true;
                playerController.SetInteractableZone(gameObject);
                playerController.interactIsActive = true;
            }
            else if (distanceToPlayer > interactionRadius && playerInRange)
            {
                playerInRange = false;
                playerController.SetInteractableZone(null);
                playerController.interactIsActive = false;
            }
        }
    }

    public void SaveCurrentZone()
    {
        if (playerInRange)
        {
            // Save the current scene index
            int currentSceneId = SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetInt("LastSavedZone", currentSceneId);
            PlayerPrefs.Save();

            Debug.Log("Zona guardada: Escena " + currentSceneId);

            // Show the save notification
            ShowSaveNotification();
        }
    }

    private void ShowSaveNotification()
    {
        if (saveNotificationText != null)
        {
            // Stop any existing notification coroutines
            StopAllCoroutines();
            // Start the notification sequence
            StartCoroutine(SaveNotificationSequence());
        }
    }

    private IEnumerator SaveNotificationSequence()
    {
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            saveNotificationText.alpha = alpha;
            yield return null;
        }
        saveNotificationText.alpha = 1f;

        // Wait for the notification duration
        yield return new WaitForSeconds(notificationDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            saveNotificationText.alpha = alpha;
            yield return null;
        }
        saveNotificationText.alpha = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}