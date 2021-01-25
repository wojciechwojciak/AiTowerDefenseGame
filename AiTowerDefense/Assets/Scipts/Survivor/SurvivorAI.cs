using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SurvivorAI : MonoBehaviour
{
    // Public variables
    public int healthPoints;

    // Private variables
    [SerializeField] private bool DebugMode = false;
    private Transform tr;
    private CircleCollider2D cirCol;
    private NavMeshAgent agent;
    private float agentNormalSpeed = 3.0f;
    private NavMeshHit hit;

    [SerializeField] private float waderRingRadius = 10;
    [SerializeField] public Transform target;
    [SerializeField] private Vector3 wanderTarget;
    [SerializeField] private float detectionDistance = 5;
    //[SerializeField] private float alarmDistance = 20;
    //[SerializeField] private float attackDamage = 20;
    // Start is called before the first frame update

    private bool isRunning;
    private bool isFollowing;
    private bool isPlayerNearby;
    private bool isZombieNearby;
    private bool hasCooldown;
    private float trapCooldown = 10f;

    // Start is called before the first frame update
    void Start()
    {
        cirCol = GetComponent<CircleCollider2D>();
        cirCol.radius = detectionDistance;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = gameObject.transform;
        wanderTarget = new Vector3(0, 0, 0);
        tr = GetComponent<Transform>();
        isRunning = false;
        isFollowing = false;
        isPlayerNearby = false;
        isZombieNearby = false;
        hasCooldown = false;
        healthPoints = 100;
    }

    IEnumerator CooldownFortrapPlacing(){
        //wait 10 sec
        agent.speed = 0;
        agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(trapCooldown);
        agent.speed = agentNormalSpeed;
        hasCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        //  Destroy if HP <= 0
        if (healthPoints <= 0)
        {
            Destroy(gameObject);
        }
        
        // Follow player if in range
        if (isFollowing)
        {
            agent.SetDestination(target.position);
            if(isZombieNearby)
            {
                //use traps
                if (!hasCooldown)
                {
                    //create trap on the ground
                    //StartCoroutine(WaitForCooldownToEnd());
                }
            }
        }
        // Run away from zombie if in range
        else if (isRunning)
        {
            agent.SetDestination(wanderTarget);
            //use traps
                if (!hasCooldown)
                {
                //Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
                //create trap on the ground
                //StartCoroutine(WaitForCooldownToEnd());
                 }
        }
        else
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

        // Atacked by enemy
        if (collision.gameObject.tag == "Enemy")
        {
            if (DebugMode)
            {
                if (collision.gameObject.tag == "Enemy")
                {
                    Debug.Log("\nWykryta kolizja z zombie.");
                }
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision) {

    }
    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag == "RescuePoint")
        {
            Destroy(gameObject);
        }
        isPlayerNearby = other.gameObject.tag == "Player";
        if(isPlayerNearby){
            isFollowing = true;
            isRunning = false;
            target = other.gameObject.transform;
        }

    }

    void OnTriggerStay2D(Collider2D other){
        isZombieNearby = other.gameObject.tag == "Enemy";
        if(!isPlayerNearby && isZombieNearby){
            isFollowing = false;
            isRunning = true;
            Vector3 dir = other.gameObject.transform.position - target.position;
            wanderTarget = target.position - dir;
            while(true){
                if (NavMesh.SamplePosition(wanderTarget, out hit, 1f, NavMesh.AllAreas)){
                    break;
                }
                wanderTarget = tr.position + (Random.insideUnitSphere * waderRingRadius);
                wanderTarget[2] = -1;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other) 
    {
        bool lostFollowingTarget = other.gameObject.tag == "Player";
        bool lostZombie = other.gameObject.tag =="Enemy";
        if(lostFollowingTarget && !isZombieNearby){
            isFollowing = false;
            wanderTarget = target.position;
            target = gameObject.transform;
        }
        else if(lostZombie && !isZombieNearby){
            isRunning = false;
        }
    }
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
                        Debug.Log("\nWander Target Location: " + wanderTarget + "        Transform Position: " + tr.position);
                    }
                    break;
                }
        }
    }
}
