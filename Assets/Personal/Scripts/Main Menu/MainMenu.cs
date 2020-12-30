using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private XNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject nameInputPanel = null;
    [SerializeField] private GameObject ipJoinPanel = null;

    public enum MainMenuStates {NAMEINPUT, LANDINGPAGE, IPJOIN, NONE };

    private MainMenuStates mainMenuState = MainMenuStates.NAMEINPUT;

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsNameKey))
        {
            ChangeState(MainMenuStates.LANDINGPAGE);
        }
    }

    private void ChangeState(MainMenuStates newState)
    {
        mainMenuState = newState;

        switch (mainMenuState)
        {
            case MainMenuStates.NAMEINPUT:
                landingPagePanel.SetActive(false);
                nameInputPanel.SetActive(true);
                ipJoinPanel.SetActive(false);
                break;

            case MainMenuStates.LANDINGPAGE:
                landingPagePanel.SetActive(true);
                nameInputPanel.SetActive(false);
                ipJoinPanel.SetActive(false);
                break;

            case MainMenuStates.IPJOIN:
                landingPagePanel.SetActive(false);
                nameInputPanel.SetActive(false);
                ipJoinPanel.SetActive(true);
                break;

            case MainMenuStates.NONE:
                landingPagePanel.SetActive(false);
                nameInputPanel.SetActive(false);
                ipJoinPanel.SetActive(false);
                break;
        }
    }

    public void HostLobbyButtonPressed()
    {
        ChangeState(MainMenuStates.NONE);
        networkManager.StartHost();
    }
}
