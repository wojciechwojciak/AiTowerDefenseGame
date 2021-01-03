using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesAI : MonoBehaviour
{
    // Public variables
    public float movementSpeed;

    // Private variables
    private bool isDead;
    private bool isWandering;
    private bool detectedEnemy;
    private bool isStuned;
    private int stunTime;
    private int HealthPoints;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        HealthPoints = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
