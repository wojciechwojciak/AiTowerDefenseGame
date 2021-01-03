using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesAI : MonoBehaviour
{
    Rigidbody2D rb;
    Transform tr;

    // Private global variables
    private float MAX_WANDER_SPEED = 2;
    private float MAX_CHASING_SPEED = 5;
    private float WANDER_RING_RADIUS = 10;

    // Private variables
    private Vector2 TargetLocation;

    private bool isDead;
    private bool isChasing;
    private bool detectedEnemy;
    private bool isStuned;
    private int stunTime;
    private int HealthPoints;

    // Start is called before the first frame update
    void Start()
    {
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
        if (isChasing == false)
        {
            // Wander
            if (rb.position == TargetLocation)
            {
                Wander();
            }
            else
            {
                rb.position = Vector2.MoveTowards(rb.position, TargetLocation, MAX_WANDER_SPEED * Time.deltaTime);
            }    
        }

    }

    public Vector2 Vector2FromAngle(float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    void Wander()
    {
        Vector2 SinAndCosFromAngle = Vector2FromAngle(Random.Range(0, 360));
        TargetLocation = SinAndCosFromAngle * WANDER_RING_RADIUS + rb.position;
        Debug.Log("\nTarget Location: "+ TargetLocation + "   Sinus And Cosinus From Angle: " + SinAndCosFromAngle);
    }
}
