using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesAI : MonoBehaviour
{
    // Public variables
    public int healthPoints;
    
    // Private variables
    [SerializeField] private bool DebugMode = false;
    private Transform tr;
    private NavMeshAgent agent;
    private NavMeshHit hit;

    [SerializeField] private float waderRingRadius = 10;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 wanderTarget;
    [SerializeField] private float aggresiveAggroDistance = 3;
    [SerializeField] private float attackDamage = 20;

    private bool isChasing;
    private bool isStuned;
    private int stunTime;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        wanderTarget = new Vector3(0, 0, 0);
        tr = GetComponent<Transform>();
        isChasing = false;
        isStuned = false;
        stunTime = 0;
        healthPoints = 100;
    }

    // Update is called once per frame
    void Update()
    {
        // Wander if there is no surviviors or player
        if (isChasing == false)
        {
            // Random wander
            if ((tr.position.x < wanderTarget[0] + 0.25 && tr.position.x > wanderTarget[0] - 0.25)  && (tr.position.y < wanderTarget[1] + 0.25 && tr.position.y > wanderTarget[1] - 0.25))
            {
                Wander();
            }
            else
            {
                agent.SetDestination(wanderTarget);
            }    
        }
        // Chase player if in range
        if (isChasing == true)
        {
            agent.SetDestination(target.position);
        }
    }

    // On Collision enter
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Change wander location if wall is hitted
        if (collision.gameObject.tag == "Wall")
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
            target = other.gameObject.transform;
        }

        // Start chasing enemy
        isChasing = true;
        agent.SetDestination(other.gameObject.transform.position);
    }

    // On Trigger Stay
    void OnTriggerStay2D(Collider2D other)
    {
        // Change target if someone is in agresive aggro range
        if ((other.gameObject.tag == "Survivor" || other.gameObject.tag == "Player") && other.gameObject != target.gameObject)
        {
            if (other.gameObject != target.gameObject ) {
                {
                    float distanceBetweenPlayerAndCollision = Vector3.Distance(other.gameObject.transform.position,tr.position);
                    if (distanceBetweenPlayerAndCollision <=  aggresiveAggroDistance)
                    {
                        if (DebugMode)
                        {
                            Debug.Log("\nNowy cel w bardzo bliskiej odległości.");
                        }
                        float distanceBetweenPlayerAndTarget = Vector3.Distance(target.position,tr.position);
                        if (distanceBetweenPlayerAndCollision < distanceBetweenPlayerAndTarget)
                        {
                            if (DebugMode)
                            {
                                Debug.Log("\nZmieniam cel ataku.");
                            }
                            target = other.gameObject.transform;
                        }
                    }
                }
            }
        }
    }

    // On Trigger exit
    void OnTriggerExit2D(Collider2D other) 
    {
        //Stop chasing if lost sight of target
        if (other.gameObject == target.gameObject)
        {
            if (DebugMode)
            {
                Debug.Log("\nStracono cel z oczu.");
            }
            isChasing = false;
            wanderTarget = target.position;
            target = null;
        }
    }

    // Random wandering 
    void Wander()
    {
        if (DebugMode)
        {
            Debug.Log("\n------------");
        }
        while (true)
        {
            wanderTarget = tr.position + (Random.insideUnitSphere * waderRingRadius);
            wanderTarget[2] = -1;
            if (NavMesh.SamplePosition(wanderTarget, out hit, 1f, NavMesh.AllAreas))
                {
                    if (DebugMode)
                    {
                        Debug.Log("\nPunkt w navmeshu");
                        Debug.Log("\nWander Target Location: " + wanderTarget + "        Transform Position: " + tr.position);
                    
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