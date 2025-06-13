using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public PlayerController playerController;
    public float interactionRadius = 2f;
    private DialogueTrigger dialogueTrigger;

    private bool playerInRange = false;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        if (dialogueTrigger == null)
        {
            Debug.LogWarning("DialogueTrigger component not found on " + gameObject.name);
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
                playerController.SetInteractableNPC(gameObject);
                playerController.interactIsActive = true;
            }
            else if (distanceToPlayer > interactionRadius && playerInRange)
            {
                playerInRange = false;
                playerController.SetInteractableNPC(null);
                playerController.interactIsActive = false;
            }
        }
    }

    public void TriggerDialogue()
    {
        if (dialogueTrigger != null)
        {
            dialogueTrigger.TriggerDialogue();
        }
        else
        {
            Debug.LogWarning("Attempted to trigger dialogue, but DialogueTrigger is missing.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}