using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using TMPro;
using Mirror;

public class LocalIpDisplay : MonoBehaviour
{
    void Start()
    {
        var localPlayer = NetworkClient.connection.identity;
        if (localPlayer == null)
        {
            gameObject.SetActive(false);
            return;
        }

        string hostname = Dns.GetHostName();
        var addresslist = Dns.GetHostEntry(hostname).AddressList;
        transform.GetChild(0).GetComponent<TMP_Text>().text = addresslist[1].MapToIPv4().ToString();
    }
}
