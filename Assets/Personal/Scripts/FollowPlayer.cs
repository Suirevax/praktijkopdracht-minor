using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FollowPlayer : MonoBehaviour
{
    GameObject localPlayer = null;

    GameObject LocalPlayer
    {
        get
        {
            if (localPlayer != null) return localPlayer;
            return localPlayer = NetworkClient.connection.identity.gameObject;
        }
    }

    float zPos;

    private void Start()
    {
        zPos = transform.position.z;
    }

    private void FixedUpdate()
    {
        if(LocalPlayer != null)
        {
            transform.position = new Vector3(LocalPlayer.transform.position.x, LocalPlayer.transform.position.y, zPos);

        }
    }
}
