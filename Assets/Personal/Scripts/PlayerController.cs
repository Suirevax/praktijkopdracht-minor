using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float movementSpeed = 10f;

    [Client]
    void Update()
    {
        if (hasAuthority){
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

            if(movement != Vector2.zero) CmdMove(movement);

            if (Input.GetKey(KeyCode.Space)) CmdColorChange(Random.ColorHSV());
        }
    }

    [Command]
    private void CmdMove(Vector2 movement) => RpcMove(movement);

    [ClientRpc]
    private void RpcMove(Vector2 movement)
    {
        GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + movement * movementSpeed * Time.deltaTime);
    }

    //private void RpcMove(Vector2 movement) => transform.Translate(movement * movementSpeed * Time.deltaTime);

    [Command]
    public void CmdColorChange(Color newColor)
    {
        RpcColorChange(newColor);
    }

    [ClientRpc]
    void RpcColorChange(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }
}
