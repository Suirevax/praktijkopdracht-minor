using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class XNetworkManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;
    [Scene] [SerializeField] public string gameScene = string.Empty;
    [Scene] [SerializeField] private string lobbyScene = string.Empty;

    //[SerializeField] GameManager gameManagerPrefab = null;

    [Header("Game")]
    [SerializeField] private PlayerController gamePlayerPrefab = null;

    public static event UnityAction OnClientConnected;
    public static event UnityAction OnClientDisconnected;

    //public List<PlayerController> players = new List<PlayerController>();

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);

        var tmpPlayers = FindObjectsOfType<PlayerController>();

        foreach (var player in tmpPlayers)
        {
            player.resetPlayerPosition();
            player.IsReady = false;
        }

        
    }

    public void StartLobby()
    {
        ServerChangeScene(lobbyScene);
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        StartLobby();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (/*SceneManager.GetActiveScene().name == menuScene*/ true)
        {
            PlayerController playerControllerInstance = Instantiate(gamePlayerPrefab);

            NetworkServer.AddPlayerForConnection(conn, playerControllerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<PlayerController>();


            //NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        //GamePlayers.Clear();
    }

    //public void NotifyPlayersOfReadyState()
    //{
    //    foreach (var player in FindObjectsOfType<PlayerController>())
    //    {
    //        //player.HandleReadyToStart(IsReadyToStart());
    //    }
    //}

    public bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in NetworkServer.connections)
        {
            if (!player.Value.identity.GetComponent<PlayerController>().IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (/*SceneManager.GetActiveScene().name == menuScene*/ true)
        {
            if (IsReadyToStart())
            {
                ServerChangeScene(gameScene);
            }
        }
    }

    //public void UpdateGamePlayers()
    //{
    //    var playerObjects = FindObjectsOfType<PlayerController>();

    //    //foreach (var player in playerObjects)
    //    //{
    //    //    var playerController = player.GetComponent<PlayerController>();
    //    //    if (!players.Contains(playerController))
    //    //    {
    //    //        players.Add(playerController);
    //    //    }
    //    //}

    //    players.Clear();
        
    //    foreach(var playerObject in playerObjects)
    //    {
    //        players.Add(playerObject);
    //    }

    //}

}
