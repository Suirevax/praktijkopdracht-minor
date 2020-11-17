using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ColorChangePad : MonoBehaviour
{
    [SerializeField] Canvas colorChangeCanvas = null;

    private void Awake()
    {
        colorChangeCanvas.gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer)
        {            
            colorChangeCanvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer)
        {
            colorChangeCanvas.gameObject.SetActive(false);
        }
    }
}
