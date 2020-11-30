using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class XNetworkManager : NetworkManager
{
    List<GameObject> players = new List<GameObject>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        players.Add(player);
        player.GetComponent<PlayerController>().playerNumber = players.Count;


        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
