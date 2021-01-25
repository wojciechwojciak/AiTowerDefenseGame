using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefabs;
    public float cooldownFire = 1f;
    private float timer = 0.0f;
    public float bulletForce = 20f;

    // Update is called once per frame
    void Update()
    {
        cooldownFire -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
        { 
            if (cooldownFire <= 0)
            {
                Shoot();
                cooldownFire = 1f;
            }
            
        }
        //Debug.Log(cooldownFire);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefabs, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
}
