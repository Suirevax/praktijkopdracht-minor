using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class XNetworkManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;
    [Scene] [SerializeField] private string gameScene = string.Empty;

    [Header("Room")]
    [SerializeField] private RoomPlayer roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private PlayerController gamePlayerPrefab = null;

    public static event UnityAction OnClientConnected;
    public static event UnityAction OnClientDisconnected;

    public List<RoomPlayer> RoomPlayers { get; } = new List<RoomPlayer>();
    public List<PlayerController> GamePlayers { get; } = new List<PlayerController>();

    public override void OnStartServer()
    {

    }

    public override void OnStartClient()
    {

    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("numPlayers = " + numPlayers);
        Debug.Log("maxConnections = " + maxConnections);
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        //if (/*SceneManager.GetActiveScene().name != menuScene*/ true)
        //{
        //    conn.Disconnect();
        //    return;
        //}
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (/*SceneManager.GetActiveScene().name == menuScene*/ true)
        {
            bool isLeader = RoomPlayers.Count == 0;

            RoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<RoomPlayer>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (/*SceneManager.GetActiveScene().name == menuScene*/ true)
        {
            if (!IsReadyToStart()) return;

            ServerChangeScene(gameScene);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        for (int i = RoomPlayers.Count - 1; i >= 0; i--)
        {
            //Debug.Log(i);
            var conn = RoomPlayers[i].connectionToClient;
            var gameplayerInstance = Instantiate(gamePlayerPrefab);
            RoomPlayers.RemoveAt(i);
            //NetworkServer.Destroy(conn.identity.gameObject);
            //NetworkServer.DestroyPlayerForConnection(conn);
            //gameplayerInstance.name = player.DisplayName;

            NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            //NetworkServer.AddPlayerForConnection(conn, gameplayerInstance.gameObject);
        }

        base.OnServerSceneChanged(sceneName);
    }
}
