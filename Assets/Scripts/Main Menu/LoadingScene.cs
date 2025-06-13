using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Instance { get; private set; }

    [SerializeField] private GameObject loadingScreenPrefab;
    private Canvas targetCanvas;
    private GameObject loadingScreenInstance;
    private Image loadingBarFill;
    private TextMeshProUGUI loadingText;
    public float minimumLoadTime = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("LoadingScene instance created and set to DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate LoadingScene instance destroyed");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        FindAndAssignCanvas();
        if (loadingScreenInstance != null)
        {
            loadingScreenInstance.SetActive(false);
            Debug.Log("Loading screen hidden");
        }
    }

    private void FindAndAssignCanvas()
    {
        targetCanvas = FindObjectOfType<Canvas>();
        if (targetCanvas == null)
        {
            Debug.LogError("No se encontró un Canvas en la escena. Creando uno nuevo.");
            GameObject canvasObject = new GameObject("LoadingCanvas");
            targetCanvas = canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        Debug.Log("Canvas assigned: " + targetCanvas.name);
    }

    public void LoadGame(int sceneId)
    {
        Debug.Log($"LoadGame called for scene ID: {sceneId}");
        MuteAllSound();
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        Debug.Log("LoadSceneAsync started");

        FindAndAssignCanvas();

        if (loadingScreenInstance == null && loadingScreenPrefab != null && targetCanvas != null)
        {
            loadingScreenInstance = Instantiate(loadingScreenPrefab, targetCanvas.transform);
            loadingBarFill = loadingScreenInstance.GetComponentInChildren<Image>();
            loadingText = loadingScreenInstance.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log("Loading screen instance created in Canvas");
        }

        if (loadingScreenInstance != null)
        {
            loadingScreenInstance.SetActive(true);
            Debug.Log("Loading screen activated");
        }
        else
        {
            Debug.LogError("Loading screen could not be created. Check prefab assignment.");
            yield break;
        }

        float startTime = Time.time;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;
        Debug.Log("Scene load operation started");

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingBarFill != null)
            {
                loadingBarFill.fillAmount = progress;
            }

            if (loadingText != null)
            {
                loadingText.text = $"Loading... {progress * 100:0}%";
            }

            Debug.Log($"Loading progress: {progress * 100:0}%");

            if (operation.progress >= 0.9f && Time.time - startTime >= minimumLoadTime)
            {
                if (loadingText != null)
                {
                    loadingText.text = "Press any key to continue";
                }

                Debug.Log("Waiting for key press to activate scene");

                if (Input.anyKeyDown)
                {
                    Debug.Log("Key pressed, activating scene");
                    operation.allowSceneActivation = true;
                    UnMuteAllSound();
                }
            }

            yield return null;
        }

        Debug.Log("LoadSceneAsync completed");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void MuteAllSound()
    {
        AudioListener.volume = 0;
    }

    public void UnMuteAllSound()
    {
        AudioListener.volume = 1;
    }
}