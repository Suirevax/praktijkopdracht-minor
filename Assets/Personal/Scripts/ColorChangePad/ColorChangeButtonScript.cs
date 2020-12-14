using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ColorChangeButtonScript : MonoBehaviour
{
    public void OnChangeColorButtonPressed()
    {
        ClientScene.localPlayer.gameObject.GetComponent<PlayerController>().CmdColorChange(GetComponent<Image>().color);
    }
}
