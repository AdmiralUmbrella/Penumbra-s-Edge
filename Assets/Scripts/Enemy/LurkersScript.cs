using UnityEngine;
using UnityEngine.AI;

public class LurkersScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public float patrolRange = 10f;
    public float chaseSpeed = 5f;
    public Transform playerTransform;

    private Vector3 startingPoint;
    private bool isChasing = false;
    private AudioSource audioSource;

    private FieldOfView playerFieldOfView;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startingPoint = transform.position;
        audioSource = GetComponent<AudioSource>();

        // Encuentra el jugador y obtiene su componente FieldOfView
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerFieldOfView = player.GetComponent<FieldOfView>();
        }
    }

    void Update()
    {
        if (!isChasing && playerFieldOfView != null && playerFieldOfView.visibleTargets.Contains(transform))
        {
            StartChasing();
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            RandomPatrol();
        }
    }

    void RandomPatrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 randomPoint = startingPoint + Random.insideUnitSphere * patrolRange;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRange, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
    }

    void ChasePlayer()
    {
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isChasing)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        // Detiene el movimiento del lurker
        audioSource.Stop();
        agent.isStopped = true;

        // Llama al GameOverScript
        GameOverScript gameOverScript = FindObjectOfType<GameOverScript>();
        if (gameOverScript != null)
        {
            gameOverScript.TriggerGameOver();
        }

        // Desactiva este script para evitar llamadas múltiples a GameOver
        enabled = false;
    }
}