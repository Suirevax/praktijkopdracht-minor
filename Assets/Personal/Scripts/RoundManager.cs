using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] RenderPipelineAsset renderPipelineGame = null;

    ProgressBar progressBar = null;

    [SerializeField] float MiniGameWinValue = 40;

    XNetworkManager networkManager;

    XNetworkManager NetworkManager
    {
        get
        {
            if (networkManager != null) { return networkManager; }
            return networkManager = Mirror.NetworkManager.singleton as XNetworkManager;
        }
    }


    private void Start()
    {
        progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();

        ProgressBar.OnProgressFull += GoodGuysWon;
        PlayerController.OnPlayerKilled += CheckBadGuysWon;

        GraphicsSettings.renderPipelineAsset = renderPipelineGame;
    }

    [Server]
    public void MiniGameWon()
    {
        RpcMiniGameWon();
        Debug.Log(progressBar.progress);
    }

    [Server]
    public void CheckBadGuysWon()
    {
        var tmp = GameObject.FindGameObjectsWithTag("Player");
        bool badGuysWon = true;

        foreach (var player in tmp)
        {
            if(player.GetComponent<PlayerController>().currentState == PlayerController.playerState.alive && player.GetComponent<PlayerController>().IsImposter == false)
            {
                Debug.Log(player.GetComponent<PlayerController>().playerName);
                badGuysWon = false;
            }
        }

        if (badGuysWon)
        {
            BadGuysWon();
        }
    }

    [Server]
    public void GoodGuysWon()
    {
        RpcGoodGuysWon();
        GameOver();
    }

    [Server]
    public void BadGuysWon()
    {
        RpcBadGuysWon();
        GameOver();
    }

    [Server]
    public void GameOver()
    {
        foreach(var player in FindObjectsOfType<PlayerController>())
        {
            player.RpcSetState(PlayerController.playerState.alive);
            player.IsImposter = false;
            player.playerNamePlateColor = Color.white;
        }
        NetworkManager.ServerChangeScene("Lobby");
    }

    [ClientRpc]
    public void RpcMiniGameWon()
    {
        progressBar.IncreaseProgress(MiniGameWinValue);
    }

    [ClientRpc]
    public void RpcGoodGuysWon()
    {
        Debug.Log("Good Guys Won!!");
    }

    [ClientRpc]
    public void RpcBadGuysWon()
    {
        Debug.Log("Bad Guys Won");
    }
}
