using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoteButton : MonoBehaviour
{
    PlayerController attachedPlayer;

    public PlayerController AttachedPlayer
    {
        set 
        {
            this.attachedPlayer = value;
            SetButtonText(attachedPlayer.playerName);
}
get
{ return attachedPlayer; }
    }


    void SetButtonText(string newButtonText)
    {
        transform.GetChild(0).GetComponent<TMP_Text>().text = newButtonText;
    }

    public void OnButtonPressed()
    {
        FindObjectOfType<MeetingChat>().PlayerSelectButtonPressed(GetComponent<Button>());
    }
}
