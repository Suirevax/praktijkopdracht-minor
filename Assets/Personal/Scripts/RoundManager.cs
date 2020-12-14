using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] ProgressBar progressBar = null;

    float MiniGameWinValue = 10;

    public void MiniGameWon()
    {
        progressBar.IncreaseProgress(MiniGameWinValue);
    }

}
