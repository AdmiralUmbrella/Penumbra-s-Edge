using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerBehaviour : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 initialRotation;

    void Start()
    {
        // Asumimos que este script está en un objeto hijo del jugador
        playerTransform = transform.parent;

        // Guardamos la rotación inicial
        initialRotation = transform.eulerAngles;

        // Aseguramos que el AudioListener esté en este objeto
        AudioListener listener = GetComponent<AudioListener>();
        if (listener == null)
        {
            listener = gameObject.AddComponent<AudioListener>();
        }
    }

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Mantenemos la posición del jugador
            transform.position = playerTransform.position;

            // Mantenemos una rotación constante
            transform.eulerAngles = initialRotation;
        }
    }
}
