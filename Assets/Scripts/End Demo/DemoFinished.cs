using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoFinished : MonoBehaviour
{
    private LoadingScene loadingScene;

    private void Start()
    {
        loadingScene = FindObjectOfType<LoadingScene>();
    }

    public void MainMenu()
    {
        loadingScene.LoadGame(0);
    }
}
