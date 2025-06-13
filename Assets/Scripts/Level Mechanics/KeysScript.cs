using UnityEngine;

public class KeysScript : MonoBehaviour
{
    public PlayerController controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.interactIsActive = true;
            controller.keys.Add(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.interactIsActive = false;
            controller.keys.Remove(gameObject);
        }
    }
}