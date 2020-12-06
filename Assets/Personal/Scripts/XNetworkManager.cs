using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class XNetworkManager : NetworkManager
{
    List<GameObject> players = new List<GameObject>();

    int minPlayers = 1;



    [Scene] [SerializeField] string lobby;
    [Scene] [SerializeField] string game;



    //private void CmdJoin(NetworkConnection conn)
    //{
    //    ClientScene.AddPlayer(ClientScene.readyConnection);
    //    GameObject addedPlayer = ClientScene.localPlayer.gameObject;
    //    players.Add(addedPlayer);
    //    addedPlayer.GetComponent<PlayerController>().playerName = "Player" + players.Count.ToString();
    //    addedPlayer.name = "Player" + players.Count.ToString();
    //}

    public override void OnStartHost()
    {
        base.OnStartHost();
        ServerChangeScene(lobby);
    }
}
