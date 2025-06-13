using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCredits : MonoBehaviour
{
    public GameObject credits;
    public GameObject mainMenu;
    public AnimationClip creditsScroll;


    public void ToggleCredits()
    {
        mainMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void BackToMain()
    {
        mainMenu.SetActive(true);
        credits.SetActive(false);
    }
}
