using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum SceneState { TITLE, MAINMENU, LOBBY, GAME }
    public SceneState currentSceneState {
        get { return currentSceneState; } 
        set 
        {
            OnStateChanged(); 
        } 
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        currentSceneState = SceneState.TITLE;
    }

    private void OnStateChanged()
    {
        switch (currentSceneState)
        {
            case SceneState.TITLE:
                break;
            case SceneState.MAINMENU:
                break;
            case SceneState.LOBBY:
                break;
            case SceneState.GAME:
                break;
        }
    }
}
