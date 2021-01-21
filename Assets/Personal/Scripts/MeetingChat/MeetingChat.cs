using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class MeetingChat : NetworkBehaviour
{

    [SerializeField] TMP_InputField msgInputField;
    [SerializeField] GameObject chatMessageContainer;
    [SerializeField] GameObject textEntryPrefab;
    [SerializeField] GameObject voteScreenContent;
    [SerializeField] GameObject playerSelectButtonPrefab;

    [SerializeField] GameObject MeetingChatScreen;
    [SerializeField] GameObject VotingScreen;

    [SerializeField] Button submitVoteButton;

    GameObject selectedVoteButon = null;

    readonly SyncList<string> chatList = new SyncList<string>();

    List<Button> voteButtons = new List<Button>();

    enum MeetingChatState {ChatScreen, VoteScreen};

    MeetingChatState currentState;

    List<GameObject> chatEntrys = new List<GameObject>();

    PlayerController selectedPlayer = null;

    readonly public SyncDictionary<NetworkIdentity, int> votesDictionary = new SyncDictionary<NetworkIdentity, int>();

    [Client]
    // Start is called before the first frame update
    void Start()
    {
        currentState = MeetingChatState.ChatScreen;

        chatList.Callback += OnChatListUpdated;

    }


    //[Client]
    //private void OnEnable()
    //{
    //    foreach (var player in FindObjectsOfType<PlayerController>())
    //    {
    //        var tmp = Instantiate(playerSelectButtonPrefab, voteScreenContent.transform);
    //        tmp.GetComponent<VoteButton>().AttachedPlayer = player;
    //        voteButtons.Add(tmp.GetComponent<Button>());
    //    }
    //}

    [ClientRpc]
    public void StartOfMeeting()
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if(player.currentState == PlayerController.playerState.alive)
            {
                var tmp = Instantiate(playerSelectButtonPrefab, voteScreenContent.transform);
                tmp.GetComponent<VoteButton>().AttachedPlayer = player;
                voteButtons.Add(tmp.GetComponent<Button>());
            }
        }
        transform.GetChild(0).gameObject.SetActive(true);
    }

    //private void OnDisable()
    //{
    //    foreach(var button in voteButtons)
    //    {
    //        Destroy(button.gameObject);
    //    }
    //    voteButtons.Clear();
    //}

    void OnChatListUpdated(SyncList<string>.Operation op, int index, string oldString, string newString)
    {
        switch (op)
        {
            case SyncList<string>.Operation.OP_ADD:
                //Debug.Log(chatList.Count);
                var newTextEntry = Instantiate(textEntryPrefab, chatMessageContainer.transform);
                newTextEntry.GetComponent<TMP_Text>().text = chatList[chatList.Count - 1];
                chatEntrys.Add(newTextEntry);
                break;
            case SyncList<string>.Operation.OP_CLEAR:
                foreach(var entry in chatEntrys)
                {
                    Destroy(entry);
                }
                chatEntrys.Clear();
                break;
        }
    }

    public void SendChatMessage()
    {
        var localPlayer = NetworkClient.connection.identity.GetComponent<PlayerController>();
        if(localPlayer.currentState == PlayerController.playerState.alive)
        {
            var msg = msgInputField.text;
            localPlayer.CmdSendChatMsg(msg);
        }
    }

    void EndOfMeeting()
    {
        chatList.Clear();
        votesDictionary.Clear();
        RpcEndOfMeeting();

    }

    [ClientRpc]
    void RpcEndOfMeeting()
    {
        foreach (var button in voteButtons)
        {
            Destroy(button.gameObject);
        }
        voteButtons.Clear();
        submitVoteButton.interactable = true;

        transform.GetChild(0).gameObject.SetActive(false);

    }

    public void ChangeScreen()
    {
        if(currentState == MeetingChatState.ChatScreen && NetworkClient.connection.identity.GetComponent<PlayerController>().currentState == PlayerController.playerState.alive)
        {
            currentState = MeetingChatState.VoteScreen;
            MeetingChatScreen.SetActive(false);
            VotingScreen.SetActive(true);
        }
        else if(currentState == MeetingChatState.VoteScreen)
        {
            currentState = MeetingChatState.ChatScreen;
            MeetingChatScreen.SetActive(true);
            VotingScreen.SetActive(false);
        }
    }

    public void PlayerSelectButtonPressed(Button pressedButton)
    {
        foreach(var button in voteButtons)
        {
            button.interactable = true;
        }
        pressedButton.interactable = false;
        selectedPlayer = pressedButton.GetComponent<VoteButton>().AttachedPlayer;

    }

    public void OnVoteButtonPressed()
    {
        if(selectedPlayer != null)
        {
            NetworkClient.connection.identity.GetComponent<PlayerController>().CmdVoteForPlayer(selectedPlayer.netIdentity);
            selectedPlayer = null;
            submitVoteButton.interactable = false;
        }
    }

    public void AddchatList(string value)
    {
        chatList.Add(value);
    }

    public void ClearChatList()
    {
        chatList.Clear();
    }

    public void HandlePlayerVote(NetworkIdentity playerNetworkIdentity)
    {
        int value = -1;
        if (votesDictionary.TryGetValue(playerNetworkIdentity, out value))
        {
            votesDictionary[playerNetworkIdentity] += 1;
        }
        else
        {
            votesDictionary[playerNetworkIdentity] = 1;
        }

        int voteCount = 0;
        foreach(var votes in votesDictionary)
        {
            voteCount += votes.Value;
        }

        int alivePlayerCount = 0;

        foreach(var player in FindObjectsOfType<PlayerController>())
        {
            if(player.currentState == PlayerController.playerState.alive)
            {
                alivePlayerCount++;
            }
        }


        if(voteCount >= alivePlayerCount)
        {
            ConcludeVote();
        }
    }

    void ConcludeVote()
    {
        NetworkClient.connection.identity.GetComponent<PlayerController>().CmdUnFreezeAllPlayers();


        KeyValuePair<NetworkIdentity, int> highestVote;
        int highestVoteValue = 0;
        foreach(var votes in votesDictionary)
        {
            if(votes.Value > highestVoteValue)
            {
                highestVoteValue = votes.Value;
                highestVote = votes;
            }
        }

        highestVote.Key.GetComponent<PlayerController>().VoteKillPlayer();
        EndOfMeeting();
        //end & reset meeting
    }
}
