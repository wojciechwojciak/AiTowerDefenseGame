﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Test
    public float movementSpeed;
    public Rigidbody2D rb2d;
    float moveHorizontal;
    float moveVertical;
    private Vector2 moveDirection;
    Vector3 mousePos;
    private Camera cam;
    Vector2 lookDir;

    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }
    
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
       
        MovementInputs();
    }

    void FixedUpdate()
    {
        lookDir.x = mousePos.x-rb2d.position.x;
        lookDir.y = mousePos.y - rb2d.position.y;
        float angle = Mathf.Atan2(lookDir.y,lookDir.x)*Mathf.Rad2Deg -180f;
        rb2d.rotation = angle;
    }

    void MovementInputs()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        moveDirection = new Vector2(moveHorizontal, moveVertical);
        rb2d.velocity = new Vector2(moveDirection.x * movementSpeed, moveDirection.y * movementSpeed);

    }

 
}
