using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Rendering;


public class PlayerController : NetworkBehaviour
{
    [SerializeField] RenderPipelineAsset renderPipelineDefault;
    [SerializeField] RenderPipelineAsset renderPipelineCustom;


    [SerializeField] GameObject playerCorpse;

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

    [SyncVar (hook = nameof(newImposter))]
    public bool IsImposter = false;

    [SerializeField] float movementSpeed = 0f;
    //[SerializeField] TMP_Text playerNamePlatePrefab;
    [SerializeField] TMP_Text playerNamePlate = null;
    //[SerializeField] Vector2 namePlateOffset = new Vector2(0, 0);

    public enum playerState { alive, dead };

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
            if (roundManager == null)
            {
                roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
            }
            return roundManager;
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

    Animator animator;
    new Rigidbody2D rigidbody2D;

    public static event UnityAction OnPlayerKilled;

    ContactFilter2D imposterContactFilter;
    ContactFilter2D goodGuyContactFilter;
    ContactFilter2D ghostContactFilter;


    bool ActionOnCooldown = false;

    [Client]
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        currentState = playerState.alive;

        if (hasAuthority)
        {
            CmdChangePlayerName(PlayerPrefs.GetString("PlayerName"));
        }

        animator = GetComponent<Animator>();

        rigidbody2D = GetComponent<Rigidbody2D>();



        imposterContactFilter.NoFilter();
        imposterContactFilter.SetLayerMask(LayerMask.GetMask("Players", "PlayerCorpse"));
        goodGuyContactFilter.NoFilter();
        goodGuyContactFilter.SetLayerMask(LayerMask.GetMask("MiniGame", "PlayerCorpse"));
        ghostContactFilter.NoFilter();
        ghostContactFilter.SetLayerMask(LayerMask.GetMask("MiniGame"));
    }

    [Client]
    void newImposter(bool oldValue, bool newValue)
    {
        if (NetworkClient.connection.identity.GetComponent<PlayerController>().IsImposter)
        {
            foreach(var player in FindObjectsOfType<PlayerController>())
            {
                if (player.IsImposter)
                {
                    player.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                }
            }
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

        if(NetworkServer.connections.Count == 1)
        {
            isLeader = true;
        }
    }

    [Client]
    void FixedUpdate()
    {
        transform.GetChild(1).gameObject.SetActive(hasAuthority);

        if (hasAuthority)
        {
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

            if (movement != Vector2.zero)
            {
                CmdMove(movement.normalized);
            }
            else
            {

            }


            if (Input.GetKey(KeyCode.Space) && !ActionOnCooldown && !(GetComponent<Rigidbody2D>().constraints == RigidbodyConstraints2D.FreezeAll))
            {
                PerformAction();
            }
        }
    }

    void PerformAction()
    {
        var colliderList = new List<Collider2D>();

        //Debug.Log("Perform Action");

        if(currentState == playerState.alive)
        {
            if (IsImposter)
            {
                //Debug.Log("Imposter Action");
                GameObject playerObject = null;
                GameObject CorpseObject = null;
                GetComponent<BoxCollider2D>().OverlapCollider(imposterContactFilter, colliderList);
                foreach(var collider in colliderList)
                {
                    if(collider.gameObject.layer == 9)
                    {
                        //Debug.Log("player action");
                        playerObject = collider.gameObject;
                    }
                    else if(collider.gameObject.layer == 13)
                    {
                        CorpseObject = collider.gameObject;
                    }
                }

                if(playerObject != null)
                {
                    CmdkillPlayer(playerObject);
                    StartActionCoolDown();
                    return;
                }
                else if(CorpseObject != null)
                {
                    //activate corpse object
                    //FindObjectOfType<MeetingChat>().gameObject.SetActive(true);
                    CmdStartMeeting();
                    StartActionCoolDown();
                    return;
                }
            }
            else
            {
                GameObject CorpseObject = null;
                GameObject MiniGameObject = null;
                GetComponent<BoxCollider2D>().OverlapCollider(goodGuyContactFilter, colliderList);
                foreach (var collider in colliderList)
                {
                    if (collider.gameObject.layer == 13)
                    {
                        CorpseObject = collider.gameObject;
                    }
                    else if (collider.gameObject.layer == 14)
                    {
                        MiniGameObject = collider.gameObject;
                    }
                }

                if (CorpseObject != null)
                {
                    //active corpse object;
                    //FindObjectOfType<MeetingChat>().gameObject.SetActive(true);
                    CmdStartMeeting();
                    StartActionCoolDown();

                    return;
                }
                else if(MiniGameObject != null)
                {
                    //activate minigame
                    MiniGameObject.GetComponent<MiniGamePad>().Activate();
                    StartActionCoolDown();
                    CmdFreezeRigidBody();
                    return;
                }
            }
        }
        else if(currentState == playerState.dead)
        {
            GetComponent<BoxCollider2D>().OverlapCollider(ghostContactFilter, colliderList);

            colliderList[0].GetComponent<MiniGamePad>().Activate();
            StartActionCoolDown();


        }

    }

    void StartActionCoolDown()
    {
        ActionOnCooldown = true;
        Invoke("resetActionCooldown",3f);
    }

    void resetActionCooldown()
    {
        ActionOnCooldown = false;
    }

    [Command]
    void CmdStartMeeting()
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            player.RpcFreezeRigidBody();
            player.RpcResetPlayerPosition();
        }

        FindObjectOfType<MeetingChat>().StartOfMeeting();
    }

    [ClientRpc]
    void RpcStartMeeting()
    {
        //FindObjectOfType<MeetingChat>.StartMeeting();
    }


    [Client]
    private void Update()
    {
        if (hasAuthority)
        {
            var animator = GetComponent<Animator>();

            var currentVelocity = GetComponent<Rigidbody2D>().velocity;

            animator.SetFloat("xVelocity", currentVelocity.x);
            animator.SetFloat("yVelocity", currentVelocity.y);


            var contactFilter = new ContactFilter2D();
            LayerMask layerMask = LayerMask.GetMask("PlayerCorpse");
            contactFilter.SetLayerMask(layerMask);
            contactFilter.NoFilter();
            contactFilter.useLayerMask = true;
            var colliderList = new List<Collider2D>();
            GetComponent<BoxCollider2D>().OverlapCollider(contactFilter, colliderList);


            //Debug.Log(colliderList.Count);

        }
    }

    [Command]
    private void CmdkillPlayer(GameObject otherPlayer)
    {
        //Debug.Log("Kill player");
        otherPlayer.GetComponent<PlayerController>().RpcSetState(playerState.dead);
        otherPlayer.GetComponent<PlayerController>().currentState = PlayerController.playerState.dead;
        OnPlayerKilled?.Invoke();
        var tmp = Instantiate(playerCorpse);
        tmp.transform.position = otherPlayer.transform.position;
        NetworkServer.Spawn(tmp);
    }

    [Server]
    public void VoteKillPlayer()
    {
        Debug.Log("player killed by voting");
        RpcSetState(playerState.dead);
        currentState = PlayerController.playerState.dead;
        OnPlayerKilled?.Invoke();
    }

    [ClientRpc]
    private void RpcHandlePlayerKilled()
    {
        //var tmp = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log("Whjaty");
        //StartVote();
    }

    void StartVote()
    {
        //FindObjectOfType<MeetingChat>().gameObject.SetActive(true);
    }

    private void CmdMove(Vector2 movement) {
        GetComponent<Rigidbody2D>().AddForce(movement.normalized * movementSpeed, ForceMode2D.Force);
    }

    [Command]
    public void CmdColorChange(Color newColor)
    {
        playerColor = newColor;
    }

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

    [ClientRpc]
    public void RpcSetState(playerState newValue)
    {
        currentState = newValue;

        Color tmpColor;
        switch (currentState)
        {
            case playerState.alive:
                tmpColor = GetComponent<SpriteRenderer>().color;
                tmpColor.a = 1;
                GetComponent<SpriteRenderer>().color = tmpColor;
                gameObject.layer = 9;
                gameObject.transform.GetChild(0).gameObject.layer = 9;
                if (hasAuthority)
                {
                    GraphicsSettings.renderPipelineAsset = renderPipelineCustom;

                }
                break;
            case playerState.dead:
                gameObject.layer = 12;
                gameObject.transform.GetChild(0).gameObject.layer = 12;
                tmpColor = GetComponent<SpriteRenderer>().color;
                tmpColor.a = 0.3f;
                GetComponent<SpriteRenderer>().color = tmpColor;
                if (hasAuthority)
                {
                    GraphicsSettings.renderPipelineAsset = renderPipelineDefault;

                }
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
        RpcUnFreezeRigidBody();
    }

    [Command]
    public void CmdStartRound()
    {
        if(GameObject.FindGameObjectsWithTag("Player").Length == 1)
        {
            return;
        }

        List<int> keys = new List<int>(NetworkServer.connections.Keys);
        var ImposterAmount = keys.Count / 2;
        List<int> randomPlayerNumbers = new List<int>();

        //for (int i = 0; i < ImposterAmount;)
        //{
        //    int randomPlayer = keys[Random.Range(0, keys.Count)];
        //    if (!randomPlayerNumbers.Contains(randomPlayer))
        //    {
        //        randomPlayerNumbers.Add(randomPlayer);
        //        i++;
        //    }
        //}

        randomPlayerNumbers.Add(0);
        randomPlayerNumbers.Add(keys[1]);

        foreach (var randomNumber in randomPlayerNumbers)
        {
            NetworkServer.connections[randomNumber].identity.GetComponent<PlayerController>().IsImposter = true;
        }

        //foreach (var randomNumber in randomPlayerNumbers)
        //{
        //    TargetSetImposterUI(NetworkServer.connections[randomNumber]);
        //}

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

    //[TargetRpc]
    //public void TargetSetImposterUI(NetworkConnection target)
    //{
    //    foreach(var conn in NetworkServer.connections)
    //    {
    //        var tmp = conn.Value.identity.GetComponent<PlayerController>();
    //        if (tmp.IsImposter)
    //        {
    //            tmp.setLocalPlayerNamePlateColor(Color.red);
    //        }
    //    }
    //}

    //public void setLocalPlayerNamePlateColor(Color newColor)
    //{
    //    gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = newColor;
    //}

    //[ClientRpc]
    //public void RpcUpdatePlayerList()
    //{
    //    NetworkManager.UpdateGamePlayers();
    //}

    public void resetPlayerPosition()
    {
        transform.position = Vector2.zero;
        
    }

    [Command]
    public void CmdSendChatMsg(string msg)
    {
        FindObjectOfType<MeetingChat>().AddchatList(msg);
    }

    [Command]
    public void CmdVoteForPlayer(NetworkIdentity player)
    {
        FindObjectOfType<MeetingChat>().HandlePlayerVote(player);
    }

    [ClientRpc]
    public void RpcFreezeRigidBody()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    [Command]
    public void CmdFreezeRigidBody()
    {
        RpcFreezeRigidBody();
    }

    [ClientRpc]
    public void RpcUnFreezeRigidBody()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    [Command]
    public void CmdUnFreezeRigidBody()
    {
        RpcUnFreezeRigidBody();
    }

    [Command]
    public void CmdUnFreezeAllPlayers()
    {
        RpcUnFreezeAllPlayers();
    }

    [ClientRpc]
    public void RpcUnFreezeAllPlayers()
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    [ClientRpc]
    public void RpcResetPlayerPosition()
    {
        resetPlayerPosition();
    }
}
