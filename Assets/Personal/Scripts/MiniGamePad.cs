using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniGamePad : MonoBehaviour
{
    [SerializeField] GameObject MiniGameManager = null;

    Color passiveColor = Color.white;
    Color selectedColor = Color.grey;

    bool selected = false;

    private void Update()
    {
        if(selected && Input.GetKey(KeyCode.Space))
        {
            MiniGameManager.GetComponent<MiniGameManager>().setCurrentState(global::MiniGameManager.state.targetPractice);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer)
        {
            GetComponent<SpriteRenderer>().color = selectedColor;
            selected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer)
        {
            GetComponent<SpriteRenderer>().color = passiveColor;
            selected = false;
            MiniGameManager.GetComponent<MiniGameManager>().setCurrentState(global::MiniGameManager.state.none);
        }
    }
}
