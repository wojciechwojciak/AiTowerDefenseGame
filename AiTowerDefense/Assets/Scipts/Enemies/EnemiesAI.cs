using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesAI : MonoBehaviour
{
    public bool DebugMode = false;
    Transform tr;

    // Private global variables
    private float WANDER_RING_RADIUS = 10;
    private NavMeshAgent agent;
    private NavMeshHit hit;

    // Private variables
    [SerializeField] private Transform Target;
    [SerializeField] private Vector3 WanderTarget;

    private bool isChasing;
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
        isChasing = false;
        isStuned = false;
        stunTime = 0;
        HealthPoints = 100;
    }

    // Update is called once per frame
    void Update()
    {
        // Wander if there is no surviviors or player
        if (isChasing == false)
        {
            // Random wander
            if ((tr.position.x < WanderTarget[0] + 0.25 && tr.position.x > WanderTarget[0] - 0.25)  && (tr.position.y < WanderTarget[1] + 0.25 && tr.position.y > WanderTarget[1] - 0.25))
            {
                Wander();
            }
            else
            {
                agent.SetDestination(WanderTarget);
            }    
        }
        // Chase player if in range
        if (isChasing == true)
        {
            agent.SetDestination(Target.position);
        }
    }

    // On Collision enter
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Change wander location if wall is hitted
        if (collision.gameObject.tag == "Map")
        {
            if (DebugMode)
            {
                Debug.Log("\nKolizja z mapą wykryta.");
            }
            Wander();
        }

        // Attack if catched survavior or player
        if (collision.gameObject.tag == "Survivor" || collision.gameObject.tag == "Player")
        {
            if (DebugMode)
            {
                if (collision.gameObject.tag == "Survivor")
                {
                    Debug.Log("\nWykryta kolizja z ocalałym.");
                }
                if (collision.gameObject.tag == "Player")
                {
                    Debug.Log("\nWykryta kolizja z graczem.");
                }
            }
        }
    }

    // On Collision exit
    void OnCollisionExit2D(Collision2D other) 
    {
        //Stop chasing if lost sight of target
        if (other.gameObject == Target.gameObject)
        {
            if (DebugMode)
            {
                Debug.Log("\nStracono cel z oczu.");
            }
            isChasing = false;
            WanderTarget = Target.position;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Check to see if the tag on the collider is equal to Survivor or Player
        if (other.gameObject.tag == "Survivor" || other.gameObject.tag == "Player")
        {
            if (DebugMode)
            {
                if (other.gameObject.tag == "Survivor")
                {
                    Debug.Log("Triggered by Survivor");
                }
                if (other.gameObject.tag == "Player")
                {
                    Debug.Log("Triggered by Player");
                }
            }
            Target = other.gameObject.transform;
        }

        // Start chasing enemy
        isChasing = true;
        agent.SetDestination(other.gameObject.transform.position);
    }

    // Random wandering 
    void Wander()
    {
        while (true)
        {
            WanderTarget = tr.position + (Random.insideUnitSphere * WANDER_RING_RADIUS);
            WanderTarget[2] = -1;
            if (DebugMode)
            {
                Debug.Log("\n------------");
                Debug.Log("\nWanderTarget"+ WanderTarget);
            }
            if (NavMesh.SamplePosition(WanderTarget, out hit, 1f, NavMesh.AllAreas))
                {
                    if (DebugMode)
                    {
                        Debug.Log("\nPunkt w navmeshu");
                        Debug.Log("\nWander Target Location: " + WanderTarget + "        Transform Position: " + tr.position);
                    
                    }
                    break;
                }
            else if (DebugMode)
            {
                Debug.Log("\nPunkt poza navmeshem");
            }
        }
    }
}

