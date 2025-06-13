using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public AudioClip doorDestroySound;
    public GameObject NPC;
    private AudioSource audioSource;
    private bool doorDestroyed = false;
    private bool isPlayingSound = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = doorDestroySound;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!doorDestroyed && !isPlayingSound && GameObject.FindGameObjectWithTag("Keys") == null)
        {
            StartCoroutine(DestroyDoorSequence());
        }
    }

    IEnumerator DestroyDoorSequence()
    {
        Destroy(NPC);
        doorDestroyed = true;
        isPlayingSound = true;

        // Desactivamos el renderer y el collider de la puerta
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;

        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // Reproducimos el sonido
        if (audioSource != null && doorDestroySound != null)
        {
            audioSource.Play();
            // Esperamos a que el audio termine de reproducirse
            yield return new WaitForSeconds(10f);
        }

        // Destruimos el objeto de la puerta después de que el sonido termine
        Destroy(gameObject);
    }
}
