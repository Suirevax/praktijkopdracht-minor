using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using Mirror;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private XNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject nameInputPanel = null;
    [SerializeField] private GameObject ipJoinPanel = null;

    [Header("Change Name")]
    [SerializeField] private TMP_Text currentNameText = null;
    [SerializeField] private TMP_InputField currentNameInputField = null;

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

    private void Update()
    {
        currentNameText.text = PlayerPrefs.GetString(PlayerPrefsNameKey);
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

    public void BackButtonIpJoinPressed()
    {
        ChangeState(MainMenuStates.LANDINGPAGE);
    }

    public void ChangeNameSaveButtonPressed()
    {
        var newName = currentNameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, newName);
        currentNameText.text = newName;
    }
}
