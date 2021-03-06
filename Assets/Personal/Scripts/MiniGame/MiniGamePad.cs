﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniGamePad : MonoBehaviour
{
    [SerializeField] GameObject MiniGameManager = null;

    [SerializeField] MiniGameManager.state MiniGameType = global::MiniGameManager.state.none;

    Color passiveColor = Color.white;
    Color selectedColor = Color.grey;

    bool selected = false;

    //private void Update()
    //{
    //    if(selected && Input.GetKey(KeyCode.Space))
    //    {
    //        //MiniGameManager.GetComponent<MiniGameManager>().SetCurrentState(global::MiniGameManager.state.targetPractice);
    //        MiniGameManager.GetComponent<MiniGameManager>().SetCurrentState(MiniGameType);
    //    }
    //}

    public void Activate()
    {
        MiniGameManager.GetComponent<MiniGameManager>().SetCurrentState(MiniGameType);
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
            MiniGameManager.GetComponent<MiniGameManager>().SetCurrentState(global::MiniGameManager.state.none);
        }
    }
}