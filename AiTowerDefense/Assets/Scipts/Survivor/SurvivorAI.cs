using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SurvivorAI : MonoBehaviour
{
    // Public variables
    public int healthPoints;
    public Text survivorEscaped;
    public Text survivorRemaining;
    public GameObject BombWeapon;

    // Private variables
    [SerializeField] private bool DebugMode = false;
    private Transform tr;
    private CircleCollider2D cirCol;
    private NavMeshAgent agent;
    private float agentNormalSpeed = 3.0f;
    private NavMeshHit hit;
    private int escaped;
    private int remaining;

    [SerializeField] private float maxWanderRingRadius = 50;
    [SerializeField] private float minWanderRingRadius = 10;
    [SerializeField] public Transform target;
    [SerializeField] private Vector3 wanderTarget;
    [SerializeField] private float detectionDistance = 5;
    //[SerializeField] private float alarmDistance = 20;
    //[SerializeField] private float attackDamage = 20;
    // Start is called before the first frame update

    [SerializeField] private bool isRunning;
    [SerializeField] private bool isFollowing;
    [SerializeField] private bool isPlayerNearby;
    [SerializeField] private bool isZombieNearby;
    [SerializeField] private bool hasCooldown;
    private float trapCooldown = 15f;

    // Start is called before the first frame update
    void Start()
    {
        survivorEscaped = GameObject.Find("SurvivorEtext").GetComponent<Text>();
        survivorRemaining = GameObject.Find("Survivortext").GetComponent<Text>();
        
        cirCol = GetComponent<CircleCollider2D>();
        cirCol.radius = detectionDistance;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = gameObject.transform;
        wanderTarget = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        tr = GetComponent<Transform>();
        isRunning = false;
        isFollowing = false;
        isPlayerNearby = false;
        isZombieNearby = false;
        hasCooldown = false;
        healthPoints = 100;
    }

    IEnumerator WaitForCooldownToEnd(){
        //wait 10 sec
        yield return new WaitForSeconds(trapCooldown);
        hasCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        //  Destroy if HP <= 0
        if (healthPoints <= 0)
        {
            remaining = int.Parse(survivorRemaining.text);
            remaining--;
            survivorRemaining.text = (remaining.ToString());
            Destroy(gameObject);
            //dekrementacja ilości
        }
        if ((escaped > 0) && (remaining == 0))
        {

            Debug.Log("YOU WIN!");
            SceneManager.LoadScene(0);
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
                    Instantiate(BombWeapon, transform.position, Quaternion.identity);
                    hasCooldown = true;
                    StartCoroutine(WaitForCooldownToEnd());
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
                    Instantiate(BombWeapon, transform.position, Quaternion.identity);
                    hasCooldown = true;
                    StartCoroutine(WaitForCooldownToEnd());
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
    }
    void OnCollisionExit2D(Collision2D collision) {

    }
    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag == "RescuePoint")
        {
            escaped = int.Parse(survivorEscaped.text);
            escaped++;
            survivorEscaped.text = (escaped.ToString());
            remaining = int.Parse(survivorRemaining.text);
            remaining--;
            survivorRemaining.text = (remaining.ToString());
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
        if(Vector3.Distance(other.gameObject.transform.position, tr.position) <  detectionDistance){
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
                    float randomWaderRingRadius = Random.Range(minWanderRingRadius,maxWanderRingRadius);
                    wanderTarget = tr.position + (Random.insideUnitSphere * randomWaderRingRadius);
                    wanderTarget[2] = -1;
                }
            }
        }
        else if(Vector3.Distance(other.gameObject.transform.position, tr.position) >  detectionDistance){
            bool lostZombie = other.gameObject.tag == "Enemy";
            if(lostZombie){
                isRunning = false;
                isZombieNearby = false;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other) 
    {
        bool lostFollowingTarget = other.gameObject.tag == "Player";
        bool lostZombie = other.gameObject.tag =="Enemy";
        if(lostFollowingTarget){
            isFollowing = false;
            wanderTarget = target.position;
            target = gameObject.transform;
        }
        else if(lostZombie && !isZombieNearby){
            isZombieNearby = false;
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
            float randomWaderRingRadius = Random.Range(minWanderRingRadius,maxWanderRingRadius);
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
