using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StalkersScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerLocation;

    public float moveTime = 5f;
    public float pauseTime = 2f;

    private bool isMoving = true;

    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(MovementRoutine());
    }

    IEnumerator MovementRoutine()
    {
        while (true)
        {
            // Mover hacia el jugador
            isMoving = true;
            yield return new WaitForSeconds(moveTime);

            // Pausar el movimiento
            isMoving = false;
            agent.ResetPath(); // Detiene el movimiento actual
            yield return new WaitForSeconds(pauseTime);
        }
    }

    void Update()
    {
        if (isMoving)
        {
            agent.SetDestination(playerLocation.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Detiene el movimiento del stalker
            audioSource.Stop();
            StopAllCoroutines();
            agent.ResetPath();

            // Llama al GameOverScript
            GameOverScript gameOverScript = FindObjectOfType<GameOverScript>();
            if (gameOverScript != null)
            {
                gameOverScript.TriggerGameOver();
            }
        }
    }
}