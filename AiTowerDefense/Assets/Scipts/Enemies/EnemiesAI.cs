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
    private CircleCollider2D cirCol;
    private NavMeshAgent agent;
    private float agentNormalSpeed = 3.5f;
    private float agentchasingSpeed = 4f;
    private NavMeshHit hit;


    [SerializeField] private float maxWaderRingRadius = 100;
    [SerializeField] private float minWaderRingRadius = 15;
    [SerializeField] public Transform target;
    [SerializeField] private Vector3 wanderTarget;
    [SerializeField] private float aggresiveAggroDistance = 5;
    [SerializeField] private float aggroDistance = 10;
    [SerializeField] private float alarmDistance = 25;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private int bulletDamage = 20;
    [SerializeField] private int trapDamage = 20;
    [SerializeField] private bool isChasing;
    [SerializeField] private bool isStuned;
    private float bombStunTime = 5f;
    private float hitStunTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        cirCol = GetComponent<CircleCollider2D>();
        cirCol.radius = alarmDistance;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = gameObject.transform;
        wanderTarget = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        tr = GetComponent<Transform>();
        isChasing = false;
        isStuned = false;
        healthPoints = 100;
    }

    IEnumerator trapStun() {
     // Wait 5 sec seconds
     agent.speed = 0;
     agent.velocity = Vector3.zero;
     yield return new WaitForSeconds(bombStunTime);
     agent.speed = agentNormalSpeed;
     isStuned = false;
    }

    IEnumerator hitStun() {
     // Wait 2 sec seconds
     agent.speed = 0;
     agent.velocity = Vector3.zero;
     yield return new WaitForSeconds(hitStunTime);
     agent.speed = agentNormalSpeed;
     isStuned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing)
        {
            agent.speed = agentchasingSpeed;
        }
        else
        {
            agent.speed = agentNormalSpeed;
        }
        //  Destroy if HP <= 0
        if (healthPoints <= 0)
        {
            Destroy(gameObject);
        }

        if (target == null)
        {
            isChasing = false;
            target = gameObject.transform;
        }
        else
        {
            //Stun time
            if (isStuned)
            {
                agent.speed = 0;
                agent.velocity = Vector3.zero;
                isChasing = false;
            }
            else
            {
                // Wander if there is no surviviors or player
                if (!isChasing)
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


        if (collision.gameObject.tag == "Bullet")
        {
            if (DebugMode)
            {
                Debug.Log("\nWykryta kolizja z pociskiem.");
            }

            //HP decrecement
            healthPoints -= bulletDamage;
        }

        if (collision.gameObject.tag == "Trap")
        {
            if (DebugMode)
            {
                Debug.Log("\nWykryta kolizja z pułapką.");
            }
            StartCoroutine(trapStun());
            //HP decrecement
            healthPoints -= trapDamage;
            isStuned = true;
            isChasing = false;
            Destroy(collision.gameObject);
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
            // Attack
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerController>().healthPoints = collision.gameObject.GetComponent<PlayerController>().healthPoints - attackDamage;
                isStuned = true;
                isChasing = false;
                StartCoroutine(hitStun());
            }

            if (collision.gameObject.tag == "Survivor")
            {
                collision.gameObject.GetComponent<SurvivorAI>().healthPoints = collision.gameObject.GetComponent<SurvivorAI>().healthPoints - attackDamage;
                isStuned = true;
                isChasing = false;
                StartCoroutine(hitStun());
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision) 
    {

    }
    void OnCollisionExit2D(Collision2D collision) {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(DebugMode)
        {
            Debug.Log("\n"+other.gameObject.GetInstanceID());
        }
    }

    // On Trigger Stay
    void OnTriggerStay2D(Collider2D other)
    {
        if(DebugMode)
        {
            //Debug.Log("\n"+other.gameObject.tag);
        }

        if (!isStuned)
        {
            bool hasPossibleTarget = other.gameObject.tag == "Survivor" || other.gameObject.tag == "Player";
            if (hasPossibleTarget)
            {
                bool hasAggro = Vector3.Distance(other.gameObject.transform.position, tr.position) <  aggroDistance;
                if(DebugMode)
                {
                    //Debug.Log("\n"+Vector3.Distance(other.gameObject.transform.position, tr.position));
                }
                // Start chasing if found player or survivor
                if (hasAggro && !isChasing)
                {
                    if (DebugMode)
                    {
                        if (other.gameObject.tag == "Survivor")
                        {
                            Debug.Log("\nTriggered by Survivor");
                        }
                        if (other.gameObject.tag == "Player")
                        {
                            Debug.Log("\nTriggered by Player");
                        }
                    }
                    // Start chasing enemy
                    target = other.gameObject.transform;
                    isChasing = true; 
                }

                if(target != null)
                {
                    bool hasOtherObjectThanTarget = other.gameObject != target.gameObject;
                    bool otherObjectIsExtremelyClose = Vector3.Distance(other.gameObject.transform.position,tr.position) <  aggresiveAggroDistance;
                    // Change target if someone is in agresive aggro range
                    if (hasOtherObjectThanTarget && isChasing && otherObjectIsExtremelyClose)
                    {
                        float distanceBetweenPlayerAndCollision = Vector3.Distance(other.gameObject.transform.position,tr.position);
                        float distanceBetweenPlayerAndTarget = Vector3.Distance(target.position,tr.position);
                        if (DebugMode)
                        {
                            Debug.Log("\nNowy cel w bardzo bliskiej odległości.");
                        }

                        if (distanceBetweenPlayerAndCollision < distanceBetweenPlayerAndTarget)
                        {
                            if (DebugMode)
                            {
                                Debug.Log("\nZmieniam target na:"+other.gameObject.tag+"     Odległość od targetu:"+distanceBetweenPlayerAndTarget+"       Odległość do nowego celu: "+distanceBetweenPlayerAndCollision);
                            }
                            target = other.gameObject.transform;
                        }
                    }
                }

            } // end of hasPossibleTarget

            //Stop chasing if lost sight of target
            if(target != null)
            {
                if (other.gameObject == target.gameObject && Vector3.Distance(other.gameObject.transform.position,tr.position) >  aggroDistance)
                {
                    if (DebugMode)
                    {
                        Debug.Log("\nStracono cel z oczu.");
                    }
                    isChasing = false;
                    wanderTarget = target.position;
                    target = gameObject.transform;
                }
            }

            // Start grouping enemies
            bool zombieInRange = other.gameObject.tag == "Enemy" && Vector3.Distance(other.gameObject.transform.position,tr.position) <= alarmDistance;
            if (isChasing && zombieInRange)
            {
                if (other.GetComponent<EnemiesAI>().target != target)
                {
                    if (DebugMode)
                    {
                        Debug.Log("\nPrzekazuję target:" + target.gameObject.GetInstanceID() + "do obiektu:" + other.gameObject.GetInstanceID());
                    }
                    other.GetComponent<EnemiesAI>().target = target;
                }
            }
        }
    }

    // On Trigger exit
    void OnTriggerExit2D(Collider2D other) 
    {

    }

    // Random wandering 
    void Wander()
    {
        while (true)
        {
            float randomWaderRingRadius = Random.Range(minWaderRingRadius,maxWaderRingRadius);
            wanderTarget = tr.position + (Random.insideUnitSphere * randomWaderRingRadius);
            wanderTarget[2] = -1;
            if (NavMesh.SamplePosition(wanderTarget, out hit, 1f, NavMesh.AllAreas))
                {
                    if (DebugMode)
                    {
                        Debug.Log("\nWander Target Location: " + wanderTarget + "        Transform Position: " + tr.position);
                    }
                    break;
                }
        }
    }
}