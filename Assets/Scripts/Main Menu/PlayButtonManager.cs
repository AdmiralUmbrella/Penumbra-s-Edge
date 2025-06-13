using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayButtonManager : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public LoadingScene loadingScene;

    private const int NEW_GAME_SCENE_INDEX = 1; // Asumiendo que el primer nivel es la escena 1
    private const string SAVE_KEY = "LastSavedZone";

    private void Update()
    {
        UpdateButtonText();
    }

    public void UpdateButtonText()
    {
        buttonText.text = HasSavedData() ? "Continue" : "New Game";
    }

    private bool HasSavedData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void OnPlayButtonClick()
    {
        Debug.Log("Play button clicked");
        int sceneToLoad = DetermineSceneToLoad();

        if (LoadingScene.Instance != null)
        {
            Debug.Log($"Calling LoadGame on LoadingScene instance. Scene to load: {sceneToLoad}");
            LoadingScene.Instance.LoadGame(sceneToLoad);
        }
        else
        {
            Debug.LogError("LoadingScene instance is null. Loading scene directly.");
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private int DetermineSceneToLoad()
    {
        if (HasSavedData())
        {
            return PlayerPrefs.GetInt(SAVE_KEY);
        }
        return NEW_GAME_SCENE_INDEX;
    }
}