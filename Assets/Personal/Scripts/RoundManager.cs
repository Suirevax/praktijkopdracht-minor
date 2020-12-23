using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundManager : NetworkBehaviour
{
    ProgressBar progressBar = null;

    float MiniGameWinValue = 10;

    private void Start()
    {
        progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();
    }

    [Server]
    public void MiniGameWon()
    {
        RpcMiniGameWon();
    }

    [ClientRpc]
    public void RpcMiniGameWon()
    {
        progressBar.IncreaseProgress(MiniGameWinValue);
    }
}
