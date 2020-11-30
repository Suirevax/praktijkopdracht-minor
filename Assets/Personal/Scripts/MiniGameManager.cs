using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] GameObject miniGameCanvas = null;
    [SerializeField] GameObject progressBar = null;

    public enum state { none, targetPractice };
    state currentState = state.none;

    private void Awake()
    {
        miniGameCanvas.SetActive(false);
    }

    private void Update()
    {
        switch (currentState)
        {
            case state.none:
                miniGameCanvas.SetActive(false);
                break;
            case state.targetPractice:
                miniGameCanvas.SetActive(true);
                break;
        }
    }

    public void setCurrentState(state newState)
    {
        currentState = newState;
    }
}
