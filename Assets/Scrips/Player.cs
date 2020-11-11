using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{

    [SerializeField] float movementSpeed;
    void Start()
    {
        
    }

    [Client]
    void Update()
    {
        if (hasAuthority)
        {
            Vector2 movement = Vector2.zero;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                movement += Vector2.right;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement += Vector2.left;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement += Vector2.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movement += Vector2.down;
            }

            transform.Translate(movement * movementSpeed * Time.deltaTime);
        }

    }
}
