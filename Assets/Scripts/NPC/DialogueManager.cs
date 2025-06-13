using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private GameObject dialoguePanel;
    private GameObject lanternBar;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI nameText;
    private Image portraitImage;

    private Queue<string> sentences;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentSentence;
    private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        sentences = new Queue<string>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitializeDialogueElements());
    }

    private IEnumerator InitializeDialogueElements()
    {
        // Espera un frame para asegurarse de que todos los objetos de la escena estén cargados
        yield return null;

        FindDialogueElements();
        ResetDialogueState();
        playerInput = FindObjectOfType<PlayerInput>();
    }

    private void FindDialogueElements()
    {
        GameObject dialogueUI = GameObject.Find("Dialogue UI");
        if (dialogueUI != null)
        {
            dialoguePanel = dialogueUI.transform.Find("DialoguePanel")?.gameObject;
            dialogueText = dialogueUI.transform.Find("DialogueText")?.GetComponent<TextMeshProUGUI>();
            nameText = dialogueUI.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            portraitImage = dialogueUI.transform.Find("PortraitImage")?.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Dialogue UI not found in the scene");
        }

        lanternBar = GameObject.Find("Lantern Bar");
    }

    private void ResetDialogueState()
    {
        isDialogueActive = false;
        isTyping = false;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        sentences.Clear();
        HideDialogueElements();
    }

    private void Update()
    {
        // Manejar la entrada del usuario
        if (isDialogueActive && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        ShowDialogueElements();
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
            if (lanternBar != null) lanternBar.SetActive(false);
        }

        if (nameText != null) nameText.text = dialogue.name;
        if (portraitImage != null) portraitImage.sprite = dialogue.portrait;

        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        StopAllCoroutines();
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        if (dialogueText != null)
        {
            dialogueText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
        }
        isTyping = false;
    }

    void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        if (dialogueText != null) dialogueText.text = currentSentence;
        isTyping = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        HideDialogueElements();
        if (playerInput != null)
        {
            playerInput.ActivateInput();
            if (lanternBar != null) lanternBar.SetActive(true);
        }
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    private void ShowDialogueElements()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);
        if (nameText != null) nameText.gameObject.SetActive(true);
        if (portraitImage != null) portraitImage.gameObject.SetActive(true);
    }

    private void HideDialogueElements()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);
        if (nameText != null) nameText.gameObject.SetActive(false);
        if (portraitImage != null) portraitImage.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}