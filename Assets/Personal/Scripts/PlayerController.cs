using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    //Lobby stuff
    [SyncVar]
    private bool isLeader = false;
    public bool IsLeader
    {
        get
        {
            return isLeader;
        }
        set
        {
            isLeader = value;
        }
    }

    [SyncVar]
    public bool IsReady = false;

    //game
    [SerializeField]
    [SyncVar (hook = nameof(ImposterStatusChanged))]
    bool IsImposter = false;

    [SerializeField] float movementSpeed = 0f;
    //[SerializeField] TMP_Text playerNamePlatePrefab;
    [SerializeField] TMP_Text playerNamePlate = null;
    //[SerializeField] Vector2 namePlateOffset = new Vector2(0, 0);

    public enum playerState { alive, dead, inMiniGame };

    [SyncVar(hook = nameof(PlayerNameChanged))]
    public string playerName = null;

    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color playerColor;

    [SyncVar(hook = nameof(PlayerNamePlateColorChanged))]
    public Color playerNamePlateColor;

    [SyncVar]
    public playerState currentState;

    RoundManager roundManager;

    RoundManager RoundManager
    {
        get
        {
            if(roundManager != null) { return roundManager; }
            return roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        }
    }

    XNetworkManager networkManager;

    XNetworkManager NetworkManager
    {
        get
        {
            if (networkManager != null) { return networkManager; }
            return networkManager = Mirror.NetworkManager.singleton as XNetworkManager;
        }
    }
    
    GameManager gameManager;

    GameManager GameManager
    {
        get
        {
            if (networkManager != null) { return gameManager; }
            return gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); ;
        }
    }

    [Client]
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        currentState = playerState.alive;

        if (hasAuthority)
        {
            CmdChangePlayerName(PlayerPrefs.GetString("PlayerName"));
        }

    }

    [Command]
    public void CmdToggleReady()
    {
        IsReady = !IsReady;

        if (IsReady)
        {
            playerNamePlateColor = Color.green;
        }
        else
        {
            playerNamePlateColor = Color.white;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //NetworkManager.GamePlayers.Add(this);

        if(NetworkServer.connections.Count == 1)
        {
            isLeader = true;
        }
    }

    [Client]
    void FixedUpdate()
    {

        if (hasAuthority && (currentState == playerState.alive)){
            Vector2 movement = Vector2.zero;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                movement += Vector2.right;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement += Vector2.left;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement += Vector2.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movement += Vector2.down;
            }

            if(movement != Vector2.zero) CmdMove(movement.normalized);

            if (Input.GetKey(KeyCode.Space) && IsImposter == true)
            {
                var tmp = GetComponent<PolygonCollider2D>();
                var contactFilter2D = new ContactFilter2D();
                contactFilter2D.NoFilter();
                var colliderList = new List<Collider2D>();
                tmp.OverlapCollider(contactFilter2D, colliderList);

                foreach(Collider2D collider in colliderList)
                {
                    if (collider.CompareTag("Player"))
                    {
                        killPlayerCommand(collider.gameObject);
                    }
                }
            }
        }
    }

    [Command]
    private void killPlayerCommand(GameObject otherPlayer)
    {
        otherPlayer.GetComponent<PlayerController>().SetState(playerState.dead);
    }

    [Command]
    private void CmdMove(Vector2 movement) {
        GetComponent<Rigidbody2D>().AddForce(movement.normalized * movementSpeed, ForceMode2D.Force);
    }

    [Command]
    public void CmdColorChange(Color newColor)
    {
        //RpcColorChange(newColor);
        playerColor = newColor;
    }

    //[ClientRpc]
    //void RpcColorChange(Color newColor)
    //{
    //    GetComponent<SpriteRenderer>().color = newColor;
    //}

    private void PlayerColorChanged(Color oldValue, Color newValue)
    {
        GetComponent<SpriteRenderer>().color = newValue;
    }
    
    private void PlayerNamePlateColorChanged(Color oldValue, Color newValue)
    {
        gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = newValue;
    }

    [Command]
    void CmdChangePlayerName(string value)
    {
        playerName = value;
    }

    ////[ClientRpc]
    //void RpcChangePlayerName(string oldValue, string newValue)
    //{
    //    playerNamePlate.GetComponent<TMP_Text>().text = newValue;
    //}

    [ClientRpc]
    public void SetState(playerState newValue)
    {
        currentState = newValue;

        Color tmpColor;
        switch (currentState)
        {
            case playerState.alive:
                tmpColor = GetComponent<SpriteRenderer>().color;
                tmpColor.a = 1;
                GetComponent<SpriteRenderer>().color = tmpColor;
                break;
            case playerState.dead:
                tmpColor = GetComponent<SpriteRenderer>().color;
                tmpColor.a = 0.3f;
                GetComponent<SpriteRenderer>().color = tmpColor;
                break;
            case playerState.inMiniGame:
                break;
        }
    }

    private void OnEnable()
    {
        if (playerNamePlate != null)
        {
            playerNamePlate.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        //playerNamePlate.gameObject.SetActive(false);
    }

    private void PlayerNameChanged(string oldValue, string newValue)
    {
        if (playerNamePlate != null)
        {
            playerNamePlate.GetComponent<TMP_Text>().text = playerName;
        }
    }

    [Command]
    public void CmdMiniGameWon()
    {
        RoundManager.MiniGameWon();
    }

    [Command]
    public void CmdStartRound()
    {
        List<int> keys = new List<int>(NetworkServer.connections.Keys);
        var ImposterAmount = keys.Count / 2;
        List<int> randomPlayerNumbers = new List<int>();

        for (int i = 0; i < ImposterAmount;)
        {
            int randomPlayer = keys[Random.Range(0, keys.Count)];
            if (!randomPlayerNumbers.Contains(randomPlayer))
            {
                randomPlayerNumbers.Add(randomPlayer);
                i++;
            }
        }

        foreach (var randomNumber in randomPlayerNumbers)
        {
            NetworkServer.connections[randomNumber].identity.GetComponent<PlayerController>().IsImposter = true;
        }
    }

    public void ImposterStatusChanged(bool oldValue, bool newValue)
    {
        if (newValue == true)
        {
            CmdChangePlayerNamePlateColor(Color.red);
        }
    }

    [Command]
    void CmdChangePlayerNamePlateColor(Color color)
    {
        playerNamePlateColor = color;
    }
}
