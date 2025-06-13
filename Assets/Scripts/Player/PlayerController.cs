using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //MOVEMENT AND ROTATION
    public float moveSpeed = 3f;
    private Vector2 move, mouseLook, joystickLook;
    private Vector3 rotationTarget;
    public bool isPC;

    //LANTERN
    public GameObject lanternContainer;
    public Image lanternBar;
    public float lanternLife, maxLanternLife;
    public bool isLanternOn = false;

    public float cooldownTime = 0.5f;
    private float lastTurnOffTime;

    //ENEMY
    private FieldOfView fov;

    //INTERACT
    public bool interactIsActive = false;
    public GameObject keysContainer;
    public List<GameObject> keys;
    public GameObject interactPrompt;
    private GameObject currentInteractableNPC;

    //SECRET PATHS
    private SecretPathsManager secretPathsManager;

    //SAVE & LOAD
    private GameObject currentInteractableZone;

    //PAUSE MENU
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public bool gameIsPaused = false;
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Start()
    {
        fov = gameObject.GetComponent<FieldOfView>();
        GetComponent<FieldOfView>().enabled = false;
        secretPathsManager = FindObjectOfType<SecretPathsManager>();

        foreach (Transform child in keysContainer.transform)
        {
            keys.Add(child.gameObject);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }    
    
    public void OnMouseLook(InputAction.CallbackContext context)
    {
        mouseLook = context.ReadValue<Vector2>();
    }    
    
    public void OnJoystickLook(InputAction.CallbackContext context)
    {
        joystickLook = context.ReadValue<Vector2>();
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        OnLantern(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Interact(context);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Pause(context);
    }

    void FixedUpdate()
    {
        if (isPC)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mouseLook);

            if(Physics.Raycast(ray, out hit))
            {
                rotationTarget = hit.point;
            }

            MovePlayerWithAim();
        }
        else
        {
            if(joystickLook.x == 0 && joystickLook.y == 0)
            {
                MovePlayer();
            }
            else
            {
                MovePlayerWithAim();
            }
        }
    }

    void Update()
    {
        LanternMeter();
        UpdateInteractPrompt();
    }

    public void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }

        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime, Space.World);
    }

    public void MovePlayerWithAim()
    {
        if (isPC)
        {
            var lookPos = rotationTarget - transform.position;
            lookPos.y = 0f;
            var rotation = Quaternion.LookRotation(lookPos);

            Vector3 aimDirection = new Vector3(rotationTarget.x, 0f, rotationTarget.z);

            if(aimDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
            }
        }
        else
        {
            Vector3 aimDirection = new Vector3(joystickLook.x, 0f, joystickLook.y);

            if (aimDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(aimDirection), 0.15f);
            }

        }

        Vector3 movement = new Vector3(move.x, 0f, move.y);

        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime, Space.World);
    }

    private void OnLantern(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && lanternLife > 0 && Time.time - lastTurnOffTime > cooldownTime)
        {
            TurnOnLantern();
        }
        else if (ctx.canceled || lanternLife <= 0)
        {
            TurnOffLantern();
        }
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && interactIsActive)
        {
            if (DialogueManager.Instance.IsDialogueActive())
            {
                DialogueManager.Instance.DisplayNextSentence();
            }
            else if (currentInteractableNPC != null)
            {
                NPCInteraction npcInteraction = currentInteractableNPC.GetComponent<NPCInteraction>();
                if (npcInteraction != null)
                {
                    npcInteraction.TriggerDialogue();
                }
                InteractWithNPC(currentInteractableNPC);
            }
            else if (currentInteractableZone != null)
            {
                InteractWithSaveZone(currentInteractableZone);
            }
            else if (keys.Count > 0)
            {
                CollectNearestKey();
            }
        }
    }

    private void Pause(InputAction.CallbackContext ctx)
    {
        if (ctx.started && !gameIsPaused)
        {
            playerInput.DeactivateInput();
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            gameIsPaused = true;
        }
        else if (ctx.started && gameIsPaused)
        {
            playerInput.ActivateInput();
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
            gameIsPaused = false;
        }
    }

    private void LanternMeter()
    {
        if (isLanternOn)
        {
            lanternLife -= Time.deltaTime;
            lanternBar.fillAmount = lanternLife / maxLanternLife;

            if (lanternLife <= 0)
            {
                TurnOffLantern();
            }
        }
        else if (lanternLife < maxLanternLife && Time.time - lastTurnOffTime > cooldownTime)
        {
            lanternLife += Time.deltaTime;
            lanternBar.fillAmount = lanternLife / maxLanternLife;
            lanternLife = Mathf.Min(lanternLife, maxLanternLife);
        }
    }

    private void TurnOnLantern()
    {
        lanternContainer.SetActive(true);
        GetComponent<FieldOfView>().enabled = true;
        isLanternOn = true;

    }

    private void TurnOffLantern()
    {
        lanternContainer.SetActive(false);
        GetComponent<FieldOfView>().enabled = false;
        isLanternOn = false;
        lastTurnOffTime = Time.time;
    }

    private void CollectNearestKey()
    {
        if (keys.Count > 0)
        {
            GameObject nearestKey = null;
            float nearestDistance = float.MaxValue;

            foreach (GameObject key in keys)
            {
                if (key != null)
                {
                    float distance = Vector3.Distance(transform.position, key.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestKey = key;
                    }
                }
            }

            if (nearestKey != null)
            {
                Debug.Log("Collected key: " + nearestKey.name);
                keys.Remove(nearestKey);
                Destroy(nearestKey);
                interactIsActive = false;
                UpdateInteractPrompt();
            }
        }
    }

    private void InteractWithNPC(GameObject npc)
    {
        Debug.Log("Interacting with NPC: " + npc.name);

        if (secretPathsManager != null)
        {
            secretPathsManager.OnNPCInteraction(npc);
        }

        // Aquí puedes añadir más lógica de interacción si es necesario
    }

    private void InteractWithSaveZone(GameObject saveZone)
    {
        SaveZoneManager saveZoneManager = saveZone.GetComponent<SaveZoneManager>();
        if (saveZoneManager != null)
        {
            saveZoneManager.SaveCurrentZone();
        }
    }

    private void UpdateInteractPrompt()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(interactIsActive);
        }
    }

    public void SetInteractableNPC(GameObject npc)
    {
        currentInteractableNPC = npc;
    }

    public void SetInteractableZone(GameObject zone)
    {
        currentInteractableZone = zone;
    }
}
