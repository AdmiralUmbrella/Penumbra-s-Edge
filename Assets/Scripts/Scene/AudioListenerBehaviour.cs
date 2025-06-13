using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerBehaviour : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 initialRotation;

    void Start()
    {
        // Asumimos que este script est� en un objeto hijo del jugador
        playerTransform = transform.parent;

        // Guardamos la rotaci�n inicial
        initialRotation = transform.eulerAngles;

        // Aseguramos que el AudioListener est� en este objeto
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
            // Mantenemos la posici�n del jugador
            transform.position = playerTransform.position;

            // Mantenemos una rotaci�n constante
            transform.eulerAngles = initialRotation;
        }
    }
}
