using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombie : MonoBehaviour
{
    private float zombieLimiter = 20f;
    private float zombieSpawn = 3f;
    private float zombieSpawnL = 0f;
    private float timer = 0.0f;
    public GameObject EnemyPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (zombieLimiter-1 >= zombieSpawnL)
        {
            timer += Time.deltaTime;
            if (timer >= zombieSpawn)
            {
                Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
                zombieSpawnL++;
                timer = 0.0f;
            }
           
        }
        Debug.Log(zombieSpawnL);
    }
}
