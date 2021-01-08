using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesAI : MonoBehaviour
{
    Rigidbody2D rb;
    Transform tr;

    // Private global variables
    private float WANDER_RING_RADIUS = 3;
    private NavMeshAgent agent;
    private NavMeshHit hit;

    // Private variables
    [SerializeField] private Transform Target;
    [SerializeField] private Vector3 WanderTarget;

    private bool isDead;
    private bool isChasing;
    private bool detectedEnemy;
    private bool isStuned;
    private int stunTime;
    private int HealthPoints;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        WanderTarget = new Vector3(0, 0, 0);
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        isChasing = false;
        detectedEnemy = false;
        isStuned = false;
        stunTime = 0;
        HealthPoints = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if(WanderTarget.x == 0 && WanderTarget.y == 0)
        {
            Wander();
        }

        if (isChasing == false)
        {
            // Wander
            if ((tr.position.x < WanderTarget.x + 0.2 && tr.position.x > WanderTarget.x - 0.2)  && (tr.position.y < WanderTarget.y + 0.2 && tr.position.y > WanderTarget.y - 0.2))
            {
                //Wander();
            }
            else
            {
                agent.SetDestination(WanderTarget);
            }    
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            Debug.Log("\nKolizja wykryta.");
            Wander();
        }
    }

    void Wander()
    {
        while (true)
        {
            Vector3 randomPoint = tr.position + Random.insideUnitSphere;
            randomPoint[2] = 0;
            Debug.Log("\nrandomPoint" + randomPoint);
            WanderTarget = randomPoint * WANDER_RING_RADIUS;
            if (NavMesh.SamplePosition(WanderTarget, out hit, 1f, NavMesh.AllAreas))
            {
                Debug.Log("\nPunkt w navmeshu");
                Debug.Log("\nWander Target Location: " + WanderTarget +"        Transform Position: " + tr.position);
                break;
            }
        }
        //Vector3 test = new Vector3(Target.x, Target.y, 0);
    }
}
