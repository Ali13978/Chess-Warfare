using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class MatchMaking : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text[] TotalCoinsText;

    [SerializeField]
    private Text Player2NameText;

    [SerializeField]
    private Text Player2LevelText;

    [SerializeField]
    private Image Player2Image;

    [SerializeField]
    private GameObject EmpirePanel, ChessVsChessPanel,LobbyPanel;

    [SerializeField]
    private InputField RoomNameText;

    private bool inCoroutine;

    private int EmpireNumber=-1;

    private string UserId;
    private int coins = 0;
    private int prize = 0;

    private bool isConnecting;

    string gameVersion = "0.27";
    private byte maxPlayersPerRoom = 2;

    private int Lobby = 1; //default 1 , 2 for friends
    private int JoinOrMake = 0; //1 = join, 2 = make (room)
    private int timer = 0;

    private InfoPanel infoPanel;

    private void Start()
    {
        infoPanel = FindObjectOfType<InfoPanel>();

        // #Critical, we must first and foremost connect to Photon Online Server.
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void SetEmpire(int EmpireNumber)
    {
        this.EmpireNumber=EmpireNumber;
    }

    public void SetTimer(int timer)
    {
        this.timer = timer;
    }

    public void SetCoins(int coins)
    {
        this.coins = coins;
        for(int i=0;i<TotalCoinsText.Length;i++)
        {
            TotalCoinsText[i].text = coins.ToString();
        }
    }

    public void SetPlayer2Name(string Player2Name)
    {
        Player2NameText.text = Player2Name;
    }

    public void SetPlayer2Level(int Level)
    {
        Player2LevelText.text = Level.ToString();
    }

    public void SetPrize(int prize)
    {
        this.prize = prize;
    }

    public void SetLobby(int Lobby) // 2 for friends 1 for normal
    {
        this.Lobby = Lobby;
        Debug.Log("Current Lobby" +this.Lobby);
    }

    public void JoinFriends()
    {
        JoinOrMake = 1;
        FindAndStartMatch();
    }

    public void MakeCustomRoom()
    {
        JoinOrMake = 2;
        FindAndStartMatch();
    }

    public void FindAndStartMatch()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            infoPanel.SetText("Please connect to internet first");
            infoPanel.ShowInfoPanel();
            return;
        }

        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();
        if(playerProfile.pD.Gld<=0)
        {
            infoPanel.SetText("Looks like you have no coins . Watch Ad to get coins");
            infoPanel.ShowInfoPanel();
            return;
        }

        if(playerProfile.pD.Gld<coins)
        {
            infoPanel.SetText("you must have atleast "+coins+" coins to start this match");
            infoPanel.ShowInfoPanel();
            return;
        }

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnectedAndReady)
        {
            isConnecting = true;
            Firebase.Auth.FirebaseAuth firebaseAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            UserId = firebaseAuth.CurrentUser.UserId;
            PhotonNetwork.LocalPlayer.NickName = UserId;

            TypedLobby typedLobby = new TypedLobby(EmpireNumber.ToString()+Lobby.ToString(), LobbyType.Default);

            if(Lobby==1)
            {
                ChessVsChessPanel.SetActive(true);
                PhotonNetwork.JoinLobby(typedLobby);
            }
            else if(Lobby==2)
            {
                if(JoinOrMake!=0)
                {
                    ChessVsChessPanel.SetActive(true);
                    PhotonNetwork.JoinLobby(typedLobby);
                }
                else
                {
                    LobbyPanel.SetActive(true);
                }
            }
            
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();

            infoPanel.SetText("Can't connect to online servers");
            infoPanel.ShowInfoPanel();
        }
    }

    public void CancelSearch()
    {
        if(isConnecting)
        {
            if(inCoroutine==false)
            {
                StartCoroutine(CancelSearchCoroutine());
            }
        }
        else
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                JoinOrMake = 0;
            }
            else if (PhotonNetwork.InLobby)
            {
                Debug.Log("Leavig Lobby");
                PhotonNetwork.LeaveLobby();
            }
        }
    }

    private IEnumerator CancelSearchCoroutine()
    {
        inCoroutine = true;
        yield return new WaitUntil(()=>isConnecting == false);
        inCoroutine = false;
        CancelSearch();
    }

    public override void OnLeftRoom()
    {
        if(PlayerPrefs.HasKey("ii"))
        {
            if(PlayerPrefs.GetInt("ii")==2)
            {
                Debug.Log("On left room");
                ChessVsChessPanel.SetActive(false);
                EmpirePanel.SetActive(true);
            }
            PlayerPrefs.DeleteKey("ii");
        }
        else
        {
            Debug.Log("On left room");
            ChessVsChessPanel.SetActive(false);
            EmpirePanel.SetActive(true);
        }
    }

    public override void OnJoinedLobby()
    {
        if (Lobby == 1)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else if(Lobby==2)
        {
            if(JoinOrMake==1) //join
            {
                if(RoomNameText.text.Equals(""))
                {
                    infoPanel.SetText("Please Enter Room Name");
                    infoPanel.ShowInfoPanel();
                    ChessVsChessPanel.SetActive(false);
                    return;
                }
                if(RoomNameText.text.Length>15)
                {
                    infoPanel.SetText("Room name cannot be more then 15 characters");
                    infoPanel.ShowInfoPanel();
                    ChessVsChessPanel.SetActive(false);
                    return;
                }
                PhotonNetwork.JoinRoom(RoomNameText.ToString());
            }
            else if(JoinOrMake==2) //make
            {
                if (RoomNameText.text.Equals(""))
                {
                    infoPanel.SetText("Please Enter Room Name");
                    infoPanel.ShowInfoPanel();
                    ChessVsChessPanel.SetActive(false);
                    return;
                }
                if (RoomNameText.text.Length > 15)
                {
                    infoPanel.SetText("Room name cannot be more then 15 characters");
                    infoPanel.ShowInfoPanel();
                    ChessVsChessPanel.SetActive(false);
                    return;
                }
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayersPerRoom;
                roomOptions.IsVisible = false;

                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                roomOptions.CustomRoomProperties.Add("bd", PlayerPrefs.GetInt("currntBoard")); //current board
                roomOptions.CustomRoomProperties.Add("pz", prize); //prize
                roomOptions.CustomRoomProperties.Add("cn", coins); //coins
                roomOptions.CustomRoomProperties.Add("me", false); //match ended? true if one wins game
                roomOptions.CustomRoomProperties.Add("tm", timer); //timer
                roomOptions.CustomRoomProperties.Add("rm1", false); //master client want rematch? will be changed after match end 
                roomOptions.CustomRoomProperties.Add("rm2", false); //other player want rematch? will be changed after match end

                PhotonNetwork.CreateRoom(RoomNameText.ToString(), roomOptions, null);
            }
            else if(JoinOrMake==0)
            {
                LobbyPanel.SetActive(true);
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if(Lobby == 1)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.

            Player2NameText.text = "Creating Room ...";

            RoomOptions roomOptions = new RoomOptions();

            roomOptions.MaxPlayers = maxPlayersPerRoom;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add("bd", PlayerPrefs.GetInt("currntBoard")); //current board
            roomOptions.CustomRoomProperties.Add("pz", prize); //prize
            roomOptions.CustomRoomProperties.Add("cn", coins); //coins
            roomOptions.CustomRoomProperties.Add("me", false); //match ended? true if one wins game
            roomOptions.CustomRoomProperties.Add("tm", timer); //timer
            roomOptions.CustomRoomProperties.Add("rm1", false); //master client want rematch? will be changed after match end 
            roomOptions.CustomRoomProperties.Add("rm2", false); //other player want rematch? will be changed after match end


            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(Lobby==2)
        {
            if(JoinOrMake==1) // join
            {
                infoPanel.SetText("Room not found. Ask your friend if the lobby is created or make your own room");
            }
            else if(JoinOrMake==2) //make
            {
                infoPanel.SetText("This room name is already taken try different room");
            }
            infoPanel.ShowInfoPanel();
            ChessVsChessPanel.SetActive(false);
            isConnecting = false;
        }
    }

    public override void OnJoinedRoom()
    {
        isConnecting = false;
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        if(PhotonNetwork.CurrentRoom.PlayerCount==2)
        {
            if((int)PhotonNetwork.CurrentRoom.CustomProperties["bd"]<PlayerPrefs.GetInt("currntBoard"))
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("bd", PlayerPrefs.GetInt("currntBoard"));
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (!PhotonNetwork.PlayerList[i].NickName.Equals(UserId))
                {
                    StartCoroutine(SetP2Name(i));
                }
            }
        }
        else
        {
            if(Lobby==1)
            {
                SetPlayer2Name("Finding Player...");
            }
            else if(Lobby==2)
            {
                SetPlayer2Name("Waiting for friend");
            }
        }
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("A new Player entered room");

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (!PhotonNetwork.PlayerList[i].NickName.Equals(UserId))
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    StartCoroutine(SetP2Name(i));
                }
            }

            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }



    private IEnumerator SetP2Name(int i)
    {
        DatabaseController databaseController = FindObjectOfType<DatabaseController>();

        databaseController.ResetAsyncResult();
        databaseController.DownloadProfileTask(PhotonNetwork.PlayerList[i].NickName);
        yield return new WaitWhile(() => databaseController.GetAsyncResult() == 0);

        if (databaseController.GetAsyncResult() == 3)
        {
            if (databaseController.GetJson() != null)
            {
                PlayerProfile player2Profile = JsonUtility.FromJson<PlayerProfile>(databaseController.GetJson());
                SetPlayer2Name(player2Profile.nm);

                Player2LevelText.text = player2Profile.Lvl.ToString();

                //start game
                infoPanel.SetText("Match Found just a momemt");
                infoPanel.ShowInfoPanel();

                SetP2Image(player2Profile);

                ProfileSaver profileSaver = new ProfileSaver();

                profileSaver.SaveClass<PlayerProfile>(player2Profile, "P2");
                
                PlayerProfile playerProfile = profileSaver.LoadProfile();
                playerProfile.pD.Gld -= coins;
                

                databaseController.ResetAsyncResult();
                playerProfile.TL++;
                profileSaver.SaveProfile(playerProfile);
                databaseController.UpdateMatch(playerProfile.UID, playerProfile.TL, "TL",playerProfile.C);

                yield return new WaitWhile(() => databaseController.GetAsyncResult()==0);

                databaseController.ResetAsyncResult();

                databaseController.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                yield return new WaitWhile(()=> databaseController.GetAsyncResult()==0);

                LeaderBoard leaderBoard = FindObjectOfType<LeaderBoard>();
                leaderBoard.ResetGrid();
                PlayerPrefs.SetInt("offline", 0);
                SceneManager.LoadScene("Game");
            }
        }
    }

    public void SetP2Image(PlayerProfile playerProfile)
    {
        string avatarId = playerProfile.av;
        bool found = false;
        foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
        {
            if (sprite.name.Equals(avatarId))
            {
                found = true;
                Player2Image.sprite = sprite;
            }
        }
        if (!found)
        {
            foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
            {
                if (sprite.name.Equals(avatarId))
                {
                    found = true;
                    Player2Image.sprite = sprite;
                }
            }
        }
    }
}
