using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinMenu : MonoBehaviour
{
    [SerializeField] private XNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAdressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        XNetworkManager.OnClientConnected += HandleClientConnected;
        XNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }
    
    private void OnDisable()
    {
        XNetworkManager.OnClientConnected -= HandleClientConnected;
        XNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAdress = ipAdressInputField.text;
        networkManager.networkAddress = ipAdress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
