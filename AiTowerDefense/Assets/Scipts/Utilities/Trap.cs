using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private float timer = 0.0f;
    public GameObject hitEffect;
    private List<Collider2D> triggerList = new List<Collider2D>();

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 3)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);
            foreach(Collider2D objekt in triggerList){
                if (objekt.gameObject.tag == "Enemy"){
                    objekt.gameObject.GetComponent<EnemiesAI>().healthPoints -= 20;
                }
            }
            Destroy(gameObject);
            
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!triggerList.Contains(other))
        {
            triggerList.Add(other);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(triggerList.Contains(other))
        {
            triggerList.Remove(other);
        }
    }
}
