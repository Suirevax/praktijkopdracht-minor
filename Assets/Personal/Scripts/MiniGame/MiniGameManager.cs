using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniGameManager : MonoBehaviour
{
    //[SerializeField] RoundManager roundManager = null;

    [SerializeField] GameObject miniGameCanvas = null;
    [SerializeField] GameObject grammatica1 = null;

    public enum state { none, targetPractice, grammatica1 };
    state currentState = state.none;

    private void Awake()
    {
        miniGameCanvas.SetActive(false);
    }

    public void SetCurrentState(state newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case state.none:
                grammatica1.SetActive(false);
                miniGameCanvas.SetActive(false);
                break;
            case state.targetPractice:
                miniGameCanvas.SetActive(true);
                break;
            case state.grammatica1:
                grammatica1.SetActive(true);
                break;
        }
    }

    public void Win()
    {
        PlayerController localplayer = NetworkClient.connection.identity.GetComponent<PlayerController>();
        SetCurrentState(MiniGameManager.state.none);
        //roundManager.MiniGameWon();
        localplayer.CmdMiniGameWon();
    }
}
