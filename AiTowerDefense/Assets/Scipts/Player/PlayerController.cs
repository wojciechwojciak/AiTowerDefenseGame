using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public Rigidbody2D rb2d;
    float moveHorizontal;
    float moveVertical;
    private Vector2 moveDirection;

    void Update()
    {
        MovementInputs();
    }

    void MovementInputs()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        moveDirection = new Vector2(moveHorizontal, moveVertical);
        rb2d.velocity = new Vector2(moveDirection.x * movementSpeed, moveDirection.y * movementSpeed);

    }

 
}
