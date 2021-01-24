﻿using System.Collections;
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
    [SerializeField] private float detectionDistance = 10;
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

        //Cooldown
        if (hasCooldown)
        {
            //create trap on the ground
            //StartCoroutine(WaitForCooldownToEnd());
        }
        
        // Follow player if in range
        if (isPlayerNearby)
        {
            isRunning = false;
            isFollowing = true;
            agent.SetDestination(target.position);
            if(isZombieNearby)
            {
                //use traps
            }
        }
        // Run away from zombie if in range
        else if (isZombieNearby)
        {
            isFollowing = false;
            isRunning = true;
            //use traps
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
