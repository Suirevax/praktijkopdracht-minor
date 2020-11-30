﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ColorChangePad : MonoBehaviour
{
    [SerializeField] Canvas colorChangeCanvas = null;

    Color passiveColor = Color.white;
    Color selectedColor = Color.grey;
    bool selected = false;

    private void Awake()
    {
        colorChangeCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(selected && Input.GetKey(KeyCode.Space))
        {
            colorChangeCanvas.gameObject.SetActive(true);
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
            colorChangeCanvas.gameObject.SetActive(false);
        }
    }
}
