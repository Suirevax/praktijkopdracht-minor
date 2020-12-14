using Mirror;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar]
    public string displayName = "Loading...";

    private XNetworkManager room;
    private XNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as XNetworkManager;
        }
    }


    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        //Room.GamePlayers.Add(this);
    }

    //public override void OnNetworkDestroy()
    //{
    //    Room.GamePlayers.Remove(this);

    //}

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

}
