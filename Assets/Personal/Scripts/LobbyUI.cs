using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button startButton = null;

    [Scene] [SerializeField] string gameScene = string.Empty;

    XNetworkManager networkManager;

    XNetworkManager NetworkManager
    {
        get
        {
            if (networkManager != null) { return networkManager; }
            return networkManager = Mirror.NetworkManager.singleton as XNetworkManager;
        }
    }

    public void ReadyButtonPressed()
    {
        PlayerController localplayer = NetworkClient.connection.identity.GetComponent<PlayerController>();
        localplayer.CmdToggleReady();
    }

    public void StartButtonPressed()
    {
        NetworkClient.connection.identity.GetComponent<PlayerController>().CmdStartRound();

        NetworkManager.ServerChangeScene(gameScene);
    }

    private void Start()
    {
        startButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        var localPlayer = NetworkClient.connection.identity;
        if (localPlayer == null)
        {
            startButton.gameObject.SetActive(false);
            return;
        }

        var localPlayerController = localPlayer.GetComponent<PlayerController>();

        if (localPlayerController != null)
        {
            if (localPlayerController.IsLeader)
            {
                startButton.gameObject.SetActive(true);

                if (NetworkManager.IsReadyToStart())
                {
                    startButton.interactable = true;
                }
                else
                {
                    startButton.interactable = false;
                }
                return;
            }
        }
        startButton.gameObject.SetActive(false);
    }
}
