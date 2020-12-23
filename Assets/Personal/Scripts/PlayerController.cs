using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float movementSpeed = 0f;
    [SerializeField] TMP_Text playerNamePlatePrefab;
    TMP_Text playerNamePlate = null;
    [SerializeField] Vector2 namePlateOffset = new Vector2(0, 0);

    public enum playerState { alive, dead, inMiniGame };

    [SyncVar(hook = nameof(PlayerNameChanged))]
    public string playerName = null;

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

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        NetworkManager.GamePlayers.Add(this);
        playerNamePlate = Instantiate(playerNamePlatePrefab, GameObject.Find("GameCanvas").transform);
    }

    [Client]
    private void Start()
    {
        currentState = playerState.alive;
        //if (hasAuthority)
        //{
        //    CmdChangePlayerName(PlayerPrefs.GetString("PlayerName"));
        //}

        if (hasAuthority)
        {
            CmdChangePlayerName(PlayerPrefs.GetString("PlayerName"));
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void Update()
    {
        playerNamePlate.transform.position = transform.position;

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

            if (Input.GetKey(KeyCode.Space))
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
        RpcColorChange(newColor);
    }

    [ClientRpc]
    void RpcColorChange(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
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

    private void OnDestroy()
    {
        Destroy(playerNamePlate);
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
        playerNamePlate.gameObject.SetActive(false);
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
}
