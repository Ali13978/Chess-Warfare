using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public Camera Maincamera,SecondaryCamera;
    public Board board;

    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    public GameObject UndoButton;

    private GameObject[,] pieces;
    private List<GameObject> movedPawns;

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    private bool rightCastle=true;
    private bool leftCastle = true;

    private int boardId;
    private Coroutine CountdownCoroutine=null;

    PlayerProfile playerProfile;
    PlayerProfile player2Profile;
    ProfileSaver profileSaver;

    private bool thisPlayerTurn;
    private bool OfflineMode = false; //vs computer true
    private int myTimer = 0;
    private bool inTileSelector = false;
    private bool inMoveSelector = false;
    private float timeLeft = 15;
    private bool isRematching = false;
    private bool RoutineStopped = false;

    private Stack<Vector2Int> MovedLocations = new Stack<Vector2Int>(); //stack for doing undo
    private Stack<GameObject> MovedPieces = new Stack<GameObject>(); //stack for doing undo 
    private Stack<int> MovesToUndo = new Stack<int>(); //stack for doing undo
    private Stack<bool> CastleUndo = new Stack<bool>(); //stack for doing castle undo

    private bool EnPeasant=false;
    public List<Vector2Int> OtherPlayerEnPeasent = new List<Vector2Int>();
    public List<Vector2Int> MyEnPeasent = new List<Vector2Int>();

    private List<Vector2Int> MovesForKing=new List<Vector2Int>();

    public List<Vector2Int> PawnLocations = new List<Vector2Int>();
    public List<Vector2Int> EnPassantMoves = new List<Vector2Int>();
    

    private WinnerPanel winnerPanel;
    private InfoPanel infoPanel;
    private DatabaseController databaseController;

    private PhotonView photonView=null;

    void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        if(PlayerPrefs.GetInt("offline")==0)
        {
            OfflineMode = false;
        }
        else if(PlayerPrefs.GetInt("offline")==1)
        {
            OfflineMode = true;
        }

        winnerPanel = FindObjectOfType<WinnerPanel>();
        infoPanel = FindObjectOfType<InfoPanel>();

        pieces = new GameObject[8, 8];
        movedPawns = new List<GameObject>();

        profileSaver = new ProfileSaver();

        playerProfile = profileSaver.LoadProfile();

        ProfileInitializer.Instance.SetPlayerName(playerProfile.nm, 0);
        ProfileInitializer.Instance.SetPlayerLevel(playerProfile.Lvl.ToString(), 0);
        ProfileInitializer.Instance.SetPlayerImage(playerProfile.av, 0);

        Confirmation.Instance.SetText("Are you sure you want to quit match? This will result in defeat");
        Confirmation.Instance.SetYes(1);

        if (!OfflineMode)
        {
            UndoButton.SetActive(true);
            photonView = gameObject.GetComponent<PhotonView>();
            myTimer = (int)PhotonNetwork.CurrentRoom.CustomProperties["tm"];
            if(myTimer==0)
            {
                Timer.Instance.Hide();
            }
            else
            {
                CountdownCoroutine= StartCoroutine(CountDownTimer());
            }
            
            databaseController = FindObjectOfType<DatabaseController>();
            player2Profile = profileSaver.LoadClass<PlayerProfile>("P2");
            
            ProfileInitializer.Instance.SetBet((int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);

            ProfileInitializer.Instance.SetPlayerName(player2Profile.nm, 1);
            ProfileInitializer.Instance.SetPlayerLevel(player2Profile.Lvl.ToString(), 1);
            ProfileInitializer.Instance.SetPlayerImage(player2Profile.av, 1);

            MyPacks myPacks = profileSaver.LoadMyPacks();
            if(myPacks==null)
            {
                Debug.Log("my packs null");
                ProfileInitializer.Instance.HideChatButton();
                StartCoroutine(LoadChatPacks());
            }
            else
            {
                for (int i = 0; i < myPacks.packs.Count; i++)
                {
                    if (myPacks.packs[i].id == 0 && myPacks.packs[i].s == true)
                    {
                        Chat.Instance.AddNormalChat();
                    }
                    else if (myPacks.packs[i].id == 1 && myPacks.packs[i].s == true)
                    {
                        Chat.Instance.AddLanguagePack();
                    }
                    else if (myPacks.packs[i].id == 2 && myPacks.packs[i].s == true)
                    {
                        Chat.Instance.AddSupportPack();
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                thisPlayerTurn = true;
                white = new Player(playerProfile.nm, true);
                black = new Player(player2Profile.nm, false);
            }
            else
            {
                thisPlayerTurn = false;
                white = new Player(player2Profile.nm, true);
                black = new Player(playerProfile.nm, false);

                SecondaryCamera.gameObject.SetActive(true);
                Maincamera.gameObject.SetActive(false);
            }
        }
        else
        {
            Timer.Instance.Hide();
            UndoButton.SetActive(true);
            ProfileInitializer.Instance.HideChatButton();

            ProfileInitializer.Instance.HideBet();
            ProfileInitializer.Instance.HideLevel();

            ProfileInitializer.Instance.SetPlayerName("Computer", 1);

            thisPlayerTurn = true;
            white = new Player(playerProfile.nm, true);
            black = new Player("Computer", false);
        }

        currentPlayer = white;
        otherPlayer = black;

        InitialSetup();
    }

    IEnumerator LoadChatPacks()
    {
        DatabaseController.Instance.ResetAsyncResult();
        DatabaseController.Instance.DownloadMyPacks(playerProfile.UID);
        yield return new WaitWhile(() => DatabaseController.Instance.GetAsyncResult() == 0);
        Chat.Instance.AddNormalChat();
        if (DatabaseController.Instance.GetJson() != null)
        {
            Debug.Log("downloaded my packs");
            MyPacks myPacks = JsonUtility.FromJson<MyPacks>(DatabaseController.Instance.GetJson());
            profileSaver.SaveMyPacks(myPacks);

            for (int i = 0; i < myPacks.packs.Count; i++)
            {
                if (myPacks.packs[i].id == 1 && myPacks.packs[i].s == true)
                {
                    Chat.Instance.AddLanguagePack();
                }
                else if (myPacks.packs[i].id == 2 && myPacks.packs[i].s == true)
                {
                    Chat.Instance.AddSupportPack();
                }
            }
            ProfileInitializer.Instance.ShowChatButton();
        }
        else
        {
            //hide chat
            ProfileInitializer.Instance.HideChatButton();
        }
    }

    private void InitialSetup()
    {
        //change skins of Board

        if (OfflineMode)
        {
            boardId = PlayerPrefs.GetInt("currntBoard");
            if(boardId>0 && boardId<=31)
            {
                SkinChanger.Instance.SetBoard(boardId);
            }
            else
            {
                SkinChanger.Instance.SetBoard(1);
            }
        }
        else
        {
            boardId= (int)PhotonNetwork.CurrentRoom.CustomProperties["bd"];
            if (boardId > 0 && boardId <= 31)
            {
                SkinChanger.Instance.SetBoard(boardId);
            }
            else
            {
                SkinChanger.Instance.SetBoard(1);
            }
        }

        AddPiece(whiteRook, white, 0, 0,1);
        AddPiece(whiteKnight, white, 1, 0, 1);
        AddPiece(whiteBishop, white, 2, 0, 1);
        AddPiece(whiteQueen, white, 3, 0, 1);
        AddPiece(whiteKing, white, 4, 0, 1);
        AddPiece(whiteBishop, white, 5, 0, 1);
        AddPiece(whiteKnight, white, 6, 0, 1);
        AddPiece(whiteRook, white, 7, 0, 1);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(whitePawn, white, i, 1, 1);
        }

        AddPiece(blackRook, black, 0, 7, 2);
        AddPiece(blackKnight, black, 1, 7, 2);
        AddPiece(blackBishop, black, 2, 7, 2);
        AddPiece(blackQueen, black, 3, 7, 2);
        AddPiece(blackKing, black, 4, 7, 2);
        AddPiece(blackBishop, black, 5, 7, 2);
        AddPiece(blackKnight, black, 6, 7, 2);
        AddPiece(blackRook, black, 7, 7, 2);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(blackPawn, black, i, 6, 2);
        }

        Material material = SkinChanger.Instance.GetMaterial2(boardId);
        if (material != null)
        {
            AiAgent.instance.SetMyMaterial( material);
        }
        
    }

    public void AddToUndoCastleList(bool castle)
    {
        CastleUndo.Push(castle);
    }

    public void AddToUndoList(GameObject Piece)
    {
        MovedPieces.Push(Piece);
        MovedLocations.Push(GridForPiece(Piece));
    }

    public void Rematch()
    {
        if(OfflineMode)
        {
            if (playerProfile.pD.Gld>=50)
            {
                UIManager.Instance.VsComputer();
            }
            else
            {
                infoPanel.SetText("Sorry cant rematch. You must have atleast 50 coins to rematch");
                infoPanel.ShowInfoPanel();
                return;
            }
        }
        else
        {
            isRematching = true;
            if(playerProfile.pD.Gld>= (int)PhotonNetwork.CurrentRoom.CustomProperties["cn"])
            {
                if(PhotonNetwork.CurrentRoom.PlayerCount==2)
                {
                    winnerPanel.HideLeaderboardButton();
                    if(PhotonNetwork.IsMasterClient)
                    {
                        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                        props.Add("rm1", true);
                        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                    }
                    else
                    {
                        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                        props.Add("rm2", true);
                        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                    }

                    StartCoroutine(WaitRematchResponse());

                }
                else
                {
                    infoPanel.SetText("Player disclined rematch");
                    infoPanel.ShowInfoPanel();
                    winnerPanel.HideRematch();
                    isRematching = false;
                    return;
                }
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                    props.Add("rm1", false);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                }
                else
                {
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                    props.Add("rm2", false);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                }

                PhotonNetwork.LeaveRoom();
                infoPanel.SetText("Sorry can't rematch. You must have atleast "+ (int)PhotonNetwork.CurrentRoom.CustomProperties["cn"] + " coins to rematch");
                infoPanel.ShowInfoPanel();
                return;
            }
        }
    }

    IEnumerator WaitRematchResponse()
    {
        StartCoroutine(LoseTime());
        if(PhotonNetwork.IsMasterClient)
        {
            yield return new WaitUntil(() => (bool)PhotonNetwork.CurrentRoom.CustomProperties["rm2"] == true || timeLeft <= 0);
        }
        else
        {
            yield return new WaitUntil(() => (bool)PhotonNetwork.CurrentRoom.CustomProperties["rm1"] == true || timeLeft <= 0);
        }
        
        if(timeLeft<=0) //player did not responded so leave this player from room
        {
            if(PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("rm1", false);
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
            else
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("rm2", false);
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }

            //exit room
            PhotonNetwork.LeaveRoom();
            winnerPanel.HideRematch();
            // winnerPanel.ShowLeaderBoardButton();
            winnerPanel.HideLeaderboardButton();

            infoPanel.SetText("Cant rematch, Player did not responded within time limit");
            infoPanel.ShowInfoPanel();
            yield break;
        }

        infoPanel.SetText("Player accepted invitation. Restarting Match");
        infoPanel.ShowInfoPanel();

        playerProfile.pD.Gld -= (int)PhotonNetwork.CurrentRoom.CustomProperties["cn"];

        databaseController.ResetAsyncResult();
        playerProfile.TL++;
        profileSaver.SaveProfile(playerProfile);
        databaseController.UpdateMatch(playerProfile.UID, playerProfile.TL, "TL", playerProfile.C);

        yield return new WaitWhile(() => databaseController.GetAsyncResult() == 0);

        databaseController.ResetAsyncResult();

        databaseController.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

        yield return new WaitWhile(() => databaseController.GetAsyncResult() == 0);

        //player responded so rematch
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props.Add("rm2", false);
            props.Add("me", false);
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props.Add("rm1", false);
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator LoseTime()
    {
        winnerPanel.ShowResponeTimer();
        timeLeft = 15;
        while (timeLeft > 0f)
        {
            yield return new WaitForSecondsRealtime(1);
            timeLeft--;
            winnerPanel.SetTimer(timeLeft);
        }
    }

    public void CancelAnyRunningTask()
    {
        if(isRematching)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("rm1", false);
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
            else
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("rm2", false);
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void UndoMove()
    {
        Debug.Log("undo move");
        if(MovesToUndo.Count==0)
        {
            infoPanel.SetText("can't undo any move");
            infoPanel.ShowInfoPanel();
            return;
        }

        if(OfflineMode) //undo 2 moves so this player takes turn
        {
            if(thisPlayerTurn) //2 moves 
            {
                int moves = MovesToUndo.Pop();
                if(moves==1)
                {
                    //getting ai agent's piece
                    GameObject APiece = MovedPieces.Pop(); 
                    Vector2Int APieceLocation = MovedLocations.Pop();

                    //AiAgent.instance.RemoveLastLocation();
                    //AiAgent.instance.AddLocation(APieceLocation.x, APieceLocation.y);

                    UndoPiece(APiece, APieceLocation);

                    moves = MovesToUndo.Pop();
                    if(moves==1)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                    }
                    else if(moves==2)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting ai agent's piece (deleted)
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        currentPlayer.capturedPieces.RemoveAt(currentPlayer.capturedPieces.Count - 1);
                        otherPlayer.pieces.Add(APiece);
                        APiece.transform.Find("default").gameObject.SetActive(true);
                        //AiAgent.instance.AddLocation(APieceLocation.x, APieceLocation.y);

                        UndoPiece(APiece, APieceLocation);
                    }
                    else if (moves == 3)
                    {
                        //getting This player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        //right castle
                        rightCastle = CastleUndo.Pop();

                        //left castle
                        leftCastle = CastleUndo.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        //APiece.transform.Find("default").gameObject.SetActive(true);
                        UndoPiece(APiece, APieceLocation);
                        KingChecker();
                    }

                    MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
                    if(moveSelector.enabled==true)
                    {
                        moveSelector.CancelMove();
                    }
                    else
                    {
                        TileSelector tileSelector = FindObjectOfType<TileSelector>();
                        tileSelector.enabled = false;
                        tileSelector.enabled = true;
                    }
                }
                else if(moves==2)
                {
                    //getting ai agent's piece
                    GameObject APiece = MovedPieces.Pop();
                    Vector2Int APieceLocation = MovedLocations.Pop();

                    //AiAgent.instance.RemoveLastLocation();
                    //AiAgent.instance.AddLocation(APieceLocation.x, APieceLocation.y);

                    UndoPiece(APiece, APieceLocation);

                    //getting this player's piece (captured)
                    APiece = MovedPieces.Pop();
                    APieceLocation = MovedLocations.Pop();

                    otherPlayer.capturedPieces.RemoveAt(otherPlayer.capturedPieces.Count - 1);
                    currentPlayer.pieces.Add(APiece);
                    APiece.transform.Find("default").gameObject.SetActive(true);
                    UndoPiece(APiece, APieceLocation);

                    moves = MovesToUndo.Pop();
                    if (moves == 1)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                    }
                    else if (moves == 2)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting ai agent's piece (deleted)
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        currentPlayer.capturedPieces.RemoveAt(currentPlayer.capturedPieces.Count - 1);
                        otherPlayer.pieces.Add(APiece);
                        APiece.transform.Find("default").gameObject.SetActive(true);
                        //AiAgent.instance.AddLocation(APieceLocation.x, APieceLocation.y);
                        UndoPiece(APiece, APieceLocation);
                    }
                    else if (moves == 3)
                    {
                        //getting This player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        //right castle
                        rightCastle = CastleUndo.Pop();

                        //left castle
                        leftCastle = CastleUndo.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                        KingChecker();

                    }

                    MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
                    if (moveSelector.enabled == true)
                    {
                        moveSelector.CancelMove();
                    }
                    else
                    {
                        TileSelector tileSelector = FindObjectOfType<TileSelector>();
                        tileSelector.enabled = false;
                        tileSelector.enabled = true;
                    }
                }
                else if(moves==3) //castle move undo
                {
                    //getting ai agent's piece
                    GameObject APiece = MovedPieces.Pop();
                    Vector2Int APieceLocation = MovedLocations.Pop();

                    UndoPiece(APiece, APieceLocation);

                    //getting ai agent's piece
                    APiece = MovedPieces.Pop();
                    APieceLocation = MovedLocations.Pop();

                    UndoPiece(APiece, APieceLocation);

                    moves = MovesToUndo.Pop();
                    if (moves == 1)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                    }
                    else if (moves == 2)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting ai agent's piece (deleted)
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        currentPlayer.capturedPieces.RemoveAt(currentPlayer.capturedPieces.Count - 1);
                        otherPlayer.pieces.Add(APiece);
                        APiece.transform.Find("default").gameObject.SetActive(true);
                        //AiAgent.instance.AddLocation(APieceLocation.x, APieceLocation.y);

                        UndoPiece(APiece, APieceLocation);
                    }
                    else if(moves==3)
                    {
                        //getting This player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        //right castle
                        rightCastle = CastleUndo.Pop();

                        //left castle
                        leftCastle = CastleUndo.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                        KingChecker();

                    }

                    MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
                    if (moveSelector.enabled == true)
                    {
                        moveSelector.CancelMove();
                    }
                    else
                    {
                        TileSelector tileSelector = FindObjectOfType<TileSelector>();
                        tileSelector.enabled = false;
                        tileSelector.enabled = true;
                    }
                }
            }
        }
        else
        {
            float undocost = (int)PhotonNetwork.CurrentRoom.CustomProperties["cn"];
            undocost /= 100;
            undocost *= 20;
            if(thisPlayerTurn)
            {
                Confirmation.Instance.SetText("Are you sure you want to undo? Undo cost " + undocost + " coins (20% of match fee)");
                Confirmation.Instance.SetYes(3);
                Confirmation.Instance.Show();
            }
            else
            {
                Confirmation.Instance.SetText("You can only undo move when its your turn");
                Confirmation.Instance.SetYes(3);
                Confirmation.Instance.Show();
            }
        }
    }

    public bool CanEnPeasant()
    {
        return EnPeasant;
    }

    public void SetEnPeasent(bool value)
    {
        EnPeasant = value;
    }

    [PunRPC]
    private void UndoNetwork()
    {
        GameObject APiece = MovedPieces.Pop();
        Vector2Int APieceLocation = MovedLocations.Pop();

        UndoPiece(APiece, APieceLocation);
    }

    private void UndoNetwork2(bool thisPlayer)
    {
        //getting other players piece (deleted)
        GameObject APiece = MovedPieces.Pop();
        Vector2Int APieceLocation = MovedLocations.Pop();

        if(thisPlayer)
        {
            currentPlayer.capturedPieces.RemoveAt(currentPlayer.capturedPieces.Count - 1);
            otherPlayer.pieces.Add(APiece);
        }
        else
        {
            otherPlayer.capturedPieces.RemoveAt(otherPlayer.capturedPieces.Count - 1);
            currentPlayer.pieces.Add(APiece);
        }

        
        APiece.transform.Find("default").gameObject.SetActive(true);
        UndoPiece(APiece, APieceLocation);
    }

    [PunRPC]
    private void UndoNetwork3()
    {
        MovesToUndo.Pop();
    }

    [PunRPC]
    private void UndoNetwork4()
    {
        infoPanel.SetText("Other player did an undo move");
        infoPanel.ShowInfoPanel();
    }

    [PunRPC]
    private void UndoNetwork5()
    {
        GameObject APiece = MovedPieces.Pop();
        Vector2Int APieceLocation = MovedLocations.Pop();

        UndoPiece(APiece, APieceLocation);

        APiece = MovedPieces.Pop();
        APieceLocation = MovedLocations.Pop();

        UndoPiece(APiece, APieceLocation);

        EnPassant();
        OtherPlayerKingChecker();
    }

    public void UndoMovePlayer()
    {
        float undocost = (int)PhotonNetwork.CurrentRoom.CustomProperties["cn"];
        undocost /= 100;
        undocost *= 20;
        if (playerProfile.pD.Gld>=undocost)
        {
            if (thisPlayerTurn) //2 moves 
            {
                int moves = MovesToUndo.Pop();

                photonView.RPC("UndoNetwork4", RpcTarget.Others);

                photonView.RPC("UndoNetwork3", RpcTarget.Others);
                if (moves == 1)
                {
                    //getting other player's piece
                    GameObject APiece = MovedPieces.Pop();
                    Vector2Int APieceLocation = MovedLocations.Pop();

                    UndoPiece(APiece, APieceLocation);

                    photonView.RPC("UndoNetwork", RpcTarget.Others);

                    moves = MovesToUndo.Pop();
                    photonView.RPC("UndoNetwork3", RpcTarget.Others);
                    if (moves == 1)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork", RpcTarget.Others);
                    }
                    else if (moves == 2)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork", RpcTarget.Others);

                        //getting other players piece (deleted)
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        currentPlayer.capturedPieces.RemoveAt(currentPlayer.capturedPieces.Count - 1);
                        otherPlayer.pieces.Add(APiece);
                        APiece.transform.Find("default").gameObject.SetActive(true);
                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork2", RpcTarget.Others,false);
                    }
                    else if (moves == 3)
                    {
                        //getting This player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        //right castle
                        rightCastle = CastleUndo.Pop();

                        //left castle
                        leftCastle = CastleUndo.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                        EnPassant();
                        KingChecker();
                        photonView.RPC("UndoNetwork5", RpcTarget.Others);
                    }

                    MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
                    if (moveSelector.enabled == true)
                    {
                        moveSelector.CancelMove();
                    }
                    else
                    {
                        TileSelector tileSelector = FindObjectOfType<TileSelector>();
                        tileSelector.enabled = false;
                        tileSelector.enabled = true;
                    }
                }
                else if (moves == 2)
                {
                    //getting other player's piece
                    GameObject APiece = MovedPieces.Pop();
                    Vector2Int APieceLocation = MovedLocations.Pop();

                    UndoPiece(APiece, APieceLocation);

                    photonView.RPC("UndoNetwork", RpcTarget.Others);

                    //getting this player's piece (killed or deleted)
                    APiece = MovedPieces.Pop();
                    APieceLocation = MovedLocations.Pop();

                    otherPlayer.capturedPieces.RemoveAt(otherPlayer.capturedPieces.Count - 1);
                    currentPlayer.pieces.Add(APiece);
                    APiece.transform.Find("default").gameObject.SetActive(true);
                    UndoPiece(APiece, APieceLocation);

                    photonView.RPC("UndoNetwork2", RpcTarget.Others,true);

                    moves = MovesToUndo.Pop();
                    photonView.RPC("UndoNetwork3", RpcTarget.Others);
                    if (moves == 1)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork", RpcTarget.Others);
                    }
                    else if (moves == 2)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork", RpcTarget.Others);

                        //getting other player's piece (deleted)
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        currentPlayer.capturedPieces.RemoveAt(currentPlayer.capturedPieces.Count - 1);
                        otherPlayer.pieces.Add(APiece);
                        APiece.transform.Find("default").gameObject.SetActive(true);
                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork2", RpcTarget.Others,false);
                    }
                    else if (moves == 3)
                    {
                        //getting This player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        //right castle
                        rightCastle = CastleUndo.Pop();

                        //left castle
                        leftCastle = CastleUndo.Pop();

                        UndoPiece(APiece, APieceLocation);

                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                        EnPassant();
                        KingChecker();
                        photonView.RPC("UndoNetwork5", RpcTarget.Others);
                    }

                    MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
                    if (moveSelector.enabled == true)
                    {
                        moveSelector.CancelMove();
                    }
                    else
                    {
                        TileSelector tileSelector = FindObjectOfType<TileSelector>();
                        tileSelector.enabled = false;
                        tileSelector.enabled = true;
                    }
                }
                else if(moves==3)
                {
                    //getting other player's piece
                    GameObject APiece = MovedPieces.Pop();
                    Vector2Int APieceLocation = MovedLocations.Pop();

                    UndoPiece(APiece, APieceLocation);

                    //getting other player's piece
                    APiece = MovedPieces.Pop();
                    APieceLocation = MovedLocations.Pop();

                    UndoPiece(APiece, APieceLocation);
                    EnPassant();
                    KingChecker();
                    photonView.RPC("UndoNetwork5", RpcTarget.Others);

                    moves = MovesToUndo.Pop();
                    photonView.RPC("UndoNetwork3", RpcTarget.Others);
                    
                    if (moves == 1)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork", RpcTarget.Others);
                    }
                    else if (moves == 2)
                    {
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork", RpcTarget.Others);

                        //getting other players piece (deleted)
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        currentPlayer.capturedPieces.RemoveAt(currentPlayer.capturedPieces.Count - 1);
                        otherPlayer.pieces.Add(APiece);
                        APiece.transform.Find("default").gameObject.SetActive(true);
                        UndoPiece(APiece, APieceLocation);

                        photonView.RPC("UndoNetwork2", RpcTarget.Others, false);
                    }
                    else if (moves == 3)
                    {
                        //getting This player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        //right castle
                        rightCastle = CastleUndo.Pop();

                        //left castle
                        leftCastle = CastleUndo.Pop();

                        UndoPiece(APiece, APieceLocation);
                        EnPassant();
                        KingChecker();
                        //getting this player's piece
                        APiece = MovedPieces.Pop();
                        APieceLocation = MovedLocations.Pop();

                        UndoPiece(APiece, APieceLocation);
                        photonView.RPC("UndoNetwork5", RpcTarget.Others);
                    }

                    MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
                    if (moveSelector.enabled == true)
                    {
                        moveSelector.CancelMove();
                    }
                    else
                    {
                        TileSelector tileSelector = FindObjectOfType<TileSelector>();
                        tileSelector.enabled = false;
                        tileSelector.enabled = true;
                    }
                }
            }
        }
        else
        {
            infoPanel.SetText("You dont have enought coins to undo move");
            infoPanel.ShowInfoPanel();
        }
    }

    private void UndoPiece(GameObject APiece,Vector2Int APieceLocation)
    {
        Piece pieceComponent = APiece.GetComponent<Piece>();
        if (pieceComponent.type == PieceType.Pawn && HasPawnMoved(APiece))
        {
            Debug.Log("undoing piece of type pawn");
            movedPawns.RemoveAt(movedPawns.Count - 1);
        }

        Vector2Int startGridPoint = GridForPiece(APiece);

        if(startGridPoint.x>=0 && startGridPoint.x<=7 && startGridPoint.y>=0 && startGridPoint.y<=7)
        {
            pieces[startGridPoint.x, startGridPoint.y] = null; //current position
        }
        
        pieces[APieceLocation.x, APieceLocation.y] = APiece;
        board.MovePiece(APiece, APieceLocation);
    }

    public void AddMovesToUndo(int count)
    {
        MovesToUndo.Push(count);
    }

    public void AddPiece(GameObject prefab, Player player, int col, int row,int PlayerNumber)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);

        if(PlayerNumber==1)
        {
            pieceObject.transform.Find("default").GetComponent<MeshRenderer>().material = SkinChanger.Instance.GetMaterial1(boardId);
        }
        else if(PlayerNumber==2)
        {
            Material material= SkinChanger.Instance.GetMaterial2(boardId);
            if(material!=null)
            {
                pieceObject.transform.Find("default").GetComponent<MeshRenderer>().material = material;
            }
        }

        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    public bool isThisPlayerTurn()
    {
        return thisPlayerTurn;
    }

    public bool isOfflineMode()
    {
        return OfflineMode;
    }

    public void SelectPieceAtGrid(Vector2Int gridPoint)
    {
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
        {
            board.SelectPiece(selectedPiece);
        }
    }

    public List<Vector2Int> MovesForPiece(GameObject pieceObject) //possible moves for current piece
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        List<Vector2Int> locations = piece.MoveLocations(gridPoint);

        // filter out offboard locations
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);

        // filter out locations with friendly piece
        locations.RemoveAll(gp => FriendlyPieceAt(gp));

        return locations;
    }

    public List<Vector2Int> MovesForOtherPiece(GameObject pieceObject,bool AllPawnLoc,bool AllLoc) //possible moves for current piece
    {
        Player temp = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = temp;
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        List<Vector2Int> locations = null;

        if (AllPawnLoc)
        {
            Pawn pawn = pieceObject.GetComponent<Pawn>();
            locations = pawn.MoveLocationsAll(gridPoint);
        }
        else
        {
            locations = piece.MoveLocations(gridPoint);
        }


        // filter out offboard locations
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);

        if(!AllLoc)
        {
            // filter out locations with friendly piece
            locations.RemoveAll(gp => FriendlyPieceAt(gp));
        }

        temp = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = temp;

        return locations;
    }

    public void SetInTileSelector(bool option)
    {
        inTileSelector = option;
    }

    public void SetInMoveSelector(bool option)
    {
        inMoveSelector=option;
    }

    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        Piece pieceComponent = piece.GetComponent<Piece>();
        
        if (pieceComponent.type == PieceType.Pawn && !HasPawnMoved(piece))
        {
            movedPawns.Add(piece);
        }

        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);
    }

    public void PawnMoved(GameObject pawn)
    {
        movedPawns.Add(pawn);
    }

    public bool HasPawnMoved(GameObject pawn)
    {
        return movedPawns.Contains(pawn);
    }

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        SoundManager.instance.PlayKillPiece();
        GameObject pieceToCapture = PieceAtGrid(gridPoint);

        if (pieceToCapture.GetComponent<Piece>().type == PieceType.King)
        {
            Debug.Log(currentPlayer.name + " wins!");

            Confirmation.Instance.SetYes(2);
            Confirmation.Instance.SetText("Are you sure you want to Rematch?");

            if (OfflineMode)
            {
                if(thisPlayerTurn) //player won
                {
                    SoundManager.instance.PlayWinner();
                    Debug.Log("this player turn");
                    winnerPanel.SetPlayerLevel(0, playerProfile.Lvl);
                    winnerPanel.SetPlayerName(0, playerProfile.nm );
                    winnerPanel.SetPlayerImage(0, playerProfile.av);
                    winnerPanel.SetWinningAmount(0, 50);

                    playerProfile.pD.Gld += 50;
                    profileSaver.SaveProfile(playerProfile);

                    DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                    winnerPanel.SetPlayerLevel(1, 1);
                    winnerPanel.SetPlayerName(1, "Computer");
                    winnerPanel.SetPlayerImage(1, null);
                    winnerPanel.SetWinningAmount(1, 0);
                }
                else //computer won
                {
                    SoundManager.instance.PlayLosser();
                    Debug.Log("computer won");
                    winnerPanel.SetPlayerLevel(1, playerProfile.Lvl);
                    winnerPanel.SetPlayerName(1, playerProfile.nm);
                    winnerPanel.SetPlayerImage(1, playerProfile.av);
                    winnerPanel.SetWinningAmount(1, 0);

                    winnerPanel.SetPlayerLevel(0, 1);
                    winnerPanel.SetPlayerName(0, "Computer");
                    winnerPanel.SetPlayerImage(0, null);
                    winnerPanel.SetWinningAmount(0, 50);
                }
            }
            else
            {
                if (thisPlayerTurn) //master client wins (other player losses)
                {
                    SoundManager.instance.PlayWinner();
                    //this is player 1
                    winnerPanel.SetPlayerLevel(0, playerProfile.Lvl);
                    winnerPanel.SetPlayerName(0, playerProfile.nm + "(ME)");
                    winnerPanel.SetPlayerImage(0, playerProfile.av);
                    winnerPanel.SetWinningAmount(0, (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);

                    playerProfile.pD.Gld += (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"];
                    databaseController.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                    UpdateLevelAndXp(true);
                    profileSaver.SaveProfile(playerProfile);

                    //just display info
                    winnerPanel.SetPlayerLevel(1, player2Profile.Lvl);
                    winnerPanel.SetPlayerName(1, player2Profile.nm);
                    winnerPanel.SetPlayerImage(1, player2Profile.av);
                    winnerPanel.SetWinningAmount(1, 0);
                }
                else //other client wins (this player losses)
                {
                    SoundManager.instance.PlayLosser();
                    //this is player 2
                    winnerPanel.SetPlayerLevel(1, playerProfile.Lvl);
                    winnerPanel.SetPlayerName(1, playerProfile.nm + "(ME)");
                    winnerPanel.SetPlayerImage(1, playerProfile.av);
                    winnerPanel.SetWinningAmount(1, 0);

                    UpdateLevelAndXp(false);
                    profileSaver.SaveProfile(playerProfile);

                    winnerPanel.SetPlayerLevel(0, player2Profile.Lvl);
                    winnerPanel.SetPlayerName(0, player2Profile.nm);
                    winnerPanel.SetPlayerImage(0, player2Profile.av);
                    winnerPanel.SetWinningAmount(0, (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);
                }

                //match ended? true if one wins game
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("me", true);
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                //StartCoroutine(WaitForAnyRematch());
            }

            winnerPanel.Show();
            Destroy(board.GetComponent<TileSelector>());
            Destroy(board.GetComponent<MoveSelector>());
        }
        currentPlayer.capturedPieces.Add(pieceToCapture);
        pieces[gridPoint.x, gridPoint.y] = null;

        otherPlayer.pieces.Remove(pieceToCapture);
        gridPoint = new Vector2Int(gridPoint.x + 10, gridPoint.y + 10);
        board.MovePiece(pieceToCapture, gridPoint);
        pieceToCapture.transform.Find("default").gameObject.SetActive(false);

        //if (OfflineMode)
        //{
        //    otherPlayer.pieces.Remove(pieceToCapture);
        //    gridPoint = new Vector2Int(gridPoint.x + 10, gridPoint.y + 10);
        //    board.MovePiece(pieceToCapture, gridPoint);
        //    pieceToCapture.transform.Find("default").gameObject.SetActive(false);
        //}
        //else
        //{
        //    board.RemovePiece(pieceToCapture);
        //}
    }

    public void SwitchTurn()
    {
        if (thisPlayerTurn == false)
        {
            thisPlayerTurn = true;
        }
        else
        {
            thisPlayerTurn = false;
        }

        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
    }

    public void Checkmate()
    {
        SoundManager.instance.PlayKillPiece();
        
        Confirmation.Instance.SetYes(2);
        Confirmation.Instance.SetText("Are you sure you want to Rematch?");

        if (OfflineMode)
        {
            if (thisPlayerTurn) //player won
            {
                SoundManager.instance.PlayWinner();
                Debug.Log("this player turn");
                winnerPanel.SetPlayerLevel(0, playerProfile.Lvl);
                winnerPanel.SetPlayerName(0, playerProfile.nm);
                winnerPanel.SetPlayerImage(0, playerProfile.av);
                winnerPanel.SetWinningAmount(0, 50);

                playerProfile.pD.Gld += 50;
                profileSaver.SaveProfile(playerProfile);

                DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                winnerPanel.SetPlayerLevel(1, 1);
                winnerPanel.SetPlayerName(1, "Computer");
                winnerPanel.SetPlayerImage(1, null);
                winnerPanel.SetWinningAmount(1, 0);
            }
            else //computer won
            {
                SoundManager.instance.PlayWinner();
                Debug.Log("computer won checkmate");
                winnerPanel.SetPlayerLevel(1, playerProfile.Lvl);
                winnerPanel.SetPlayerName(1, playerProfile.nm);
                winnerPanel.SetPlayerImage(1, playerProfile.av);
                winnerPanel.SetWinningAmount(1, 0);

                winnerPanel.SetPlayerLevel(0, 1);
                winnerPanel.SetPlayerName(0, "Computer");
                winnerPanel.SetPlayerImage(0, null);
                winnerPanel.SetWinningAmount(0, 50);
            }
        }
        else
        {
            if (thisPlayerTurn) //master client wins (other player losses)
            {
                SoundManager.instance.PlayWinner();
                //this is player 1
                winnerPanel.SetPlayerLevel(0, playerProfile.Lvl);
                winnerPanel.SetPlayerName(0, playerProfile.nm + "(ME)");
                winnerPanel.SetPlayerImage(0, playerProfile.av);
                winnerPanel.SetWinningAmount(0, (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);

                playerProfile.pD.Gld += (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"];
                databaseController.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                UpdateLevelAndXp(true);
                profileSaver.SaveProfile(playerProfile);

                //just display info
                winnerPanel.SetPlayerLevel(1, player2Profile.Lvl);
                winnerPanel.SetPlayerName(1, player2Profile.nm);
                winnerPanel.SetPlayerImage(1, player2Profile.av);
                winnerPanel.SetWinningAmount(1, 0);
            }
            else //other client wins (this player losses)
            {
                SoundManager.instance.PlayLosser();
                //this is player 2
                winnerPanel.SetPlayerLevel(1, playerProfile.Lvl);
                winnerPanel.SetPlayerName(1, playerProfile.nm + "(ME)");
                winnerPanel.SetPlayerImage(1, playerProfile.av);
                winnerPanel.SetWinningAmount(1, 0);

                UpdateLevelAndXp(false);
                profileSaver.SaveProfile(playerProfile);

                winnerPanel.SetPlayerLevel(0, player2Profile.Lvl);
                winnerPanel.SetPlayerName(0, player2Profile.nm);
                winnerPanel.SetPlayerImage(0, player2Profile.av);
                winnerPanel.SetWinningAmount(0, (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);
            }

            //match ended? true if one wins game
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props.Add("me", true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        winnerPanel.Show();
        Destroy(board.GetComponent<TileSelector>());
        Destroy(board.GetComponent<MoveSelector>());
    }

    private void UpdateLevelAndXp(bool wins) //after match end
    {
        //calculate xp

        int currentXP = playerProfile.pD.XP;
        int currentLevel = playerProfile.Lvl / 5;

        string lobbyName = PhotonNetwork.CurrentLobby.Name;
        int xpEarned;
        int.TryParse(lobbyName, out xpEarned);

        xpEarned /= 10;
        if(wins)
        {
            xpEarned *= 50; //earned xp on win
            playerProfile.TW++;
            databaseController.UpdateMatch(playerProfile.UID,playerProfile.TW,"TW",playerProfile.C);
            playerProfile.TL--;
            databaseController.UpdateMatch(playerProfile.UID, playerProfile.TL, "TL", playerProfile.C);
        }
        else
        {
            xpEarned *= 25; //earned xp on Loss
        }

        currentXP += xpEarned;
        if (currentLevel==0) //level 1 to 4
        {
            if(currentXP>100) //max xp of level.... update level and xp
            {
                playerProfile.Lvl++;
                currentXP -= 100;
                playerProfile.pD.XP = currentXP;

                databaseController.UpdateLevel(playerProfile.UID, playerProfile.Lvl);
            }
            else
            {
                playerProfile.pD.XP=currentXP;
            }

            databaseController.UpdateXP(playerProfile.UID, playerProfile.pD.XP);
        }
        else
        {
            int maxXP = (currentLevel * 100) + 100;
            if(currentXP>maxXP) //update lvl and xp
            {
                playerProfile.Lvl++;
                currentXP -= maxXP;
                playerProfile.pD.XP = currentXP;

                databaseController.UpdateLevel(playerProfile.UID, playerProfile.Lvl);
            }
            else //update xp
            {
                playerProfile.pD.XP = currentXP;
            }

            databaseController.UpdateXP(playerProfile.UID, playerProfile.pD.XP);
        }

    }

    public void SelectPiece(GameObject piece)
    {
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece)
    {
        board.DeselectPiece(piece);
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
    {
        return currentPlayer.pieces.Contains(piece);
    }

    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0)
        {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }

    public Vector2Int GridForPiece(GameObject piece)
    {
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] == piece)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool FriendlyPieceAt(Vector2Int gridPoint)
    {
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null) 
        {
            return false;
        }

        if (otherPlayer.pieces.Contains(piece))
        {
            return false;
        }

        return true;
    }

    IEnumerator CountDownTimer()
    {
        RoutineStopped = false;
        Timer.Instance.StartTimer(myTimer);
        yield return new WaitUntil(()=> !Timer.Instance.IsRunning());

        if(RoutineStopped)
        {
            Debug.Log("routine stopped");
            yield break;
        }
        Debug.Log("after timer stopped");

        if (thisPlayerTurn)
        {
            if(inMoveSelector)
            {
                MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
                moveSelector.CancelMove();
            }
            if(inTileSelector)
            {
                thisPlayerTurn = false; //disable input
            }
            //check if king under check?

            List<Vector2Int> MovesForKing = new List<Vector2Int>();
            GameObject King = null;
            foreach (GameObject Piece in currentPlayer.pieces)
            {
                if (Piece.name.Contains("King"))
                {
                    MovesForKing = MovesForPiece(Piece);
                    King = Piece;
                }
            }

            bool kingUnderCheck = false;

            //disable those moves where king under check
            //get moves of other player
            List<GameObject> OtherPlayerPieces = otherPlayer.pieces;

            Vector2Int KingLocation = GridForPiece(King);
            List<Vector2Int> TempMoves = new List<Vector2Int>();

            foreach (GameObject piece in OtherPlayerPieces)
            {
                List<Vector2Int> Moves = null;
                if (piece.name.Contains("Pawn"))
                {
                    Moves = GameManager.instance.MovesForOtherPiece(piece, true, true);
                }
                else
                {
                    Moves = GameManager.instance.MovesForOtherPiece(piece, false, true);
                }

                foreach (Vector2Int move in Moves)
                {
                    if (KingLocation == move)
                    {
                        kingUnderCheck = true;
                    }
                    foreach (Vector2Int kingmove in MovesForKing)
                    {
                        if (move == kingmove)
                        {
                            //disable this move for the king
                            TempMoves.Add(kingmove);

                            //MovesForKing.Remove(kingmove);
                        }
                    }
                }
            }

            foreach (Vector2Int temp in TempMoves)
            {
                Debug.Log("temp move " + temp.y + " " + temp.x);
                if (MovesForKing.Contains(temp))
                {
                    MovesForKing.Remove(temp);
                }
            }

            if (kingUnderCheck && MovesForKing.Count>0)
            {
                TileSelector tileSelector = FindObjectOfType<TileSelector>();

                int index = Random.Range(0, MovesForKing.Count);

                Vector2Int gridpoint=GridForPiece(King);

                tileSelector.PieceClickedRPC(gridpoint.x, gridpoint.y);
                tileSelector.ExitState(King);

                if (PieceAtGrid(MovesForKing[index]) == null)
                {
                    AddToUndoList(King);
                    AddMovesToUndo(1);

                    Move(King, MovesForKing[index]);
                }
                else
                {
                    AddToUndoList(PieceAtGrid(MovesForKing[index]));
                    AddToUndoList(King);
                    AddMovesToUndo(2);

                    CapturePieceAt(MovesForKing[index]);
                    Move(King, MovesForKing[index]);
                }

                thisPlayerTurn = true;
                MoveSelector moveSelector1 = FindObjectOfType<MoveSelector>();
                moveSelector1.MovePieceRPC(MovesForKing[index].x, MovesForKing[index].y);
                moveSelector1.ExitState();
            }
            else
            {
                //take random turn
                //get pieces that can move 
                List<GameObject> PiecesThatCanMove = currentPlayer.pieces; //index of pieces that can move

                List<int> validIndexes = new List<int>();
                int temp = 0;
                foreach (GameObject piece in PiecesThatCanMove)
                {
                    if (MovesForPiece(piece).Count > 0)
                    {
                        validIndexes.Add(temp);
                    }
                    temp++;
                }

                //move any random piece (select random)
                //generate random number from possible indexes
                int index = Random.Range(0, validIndexes.Count);

                //get gridpoint of this piece
                Vector2Int gridpoint = GridForPiece(PiecesThatCanMove[validIndexes[index]]);

                TileSelector tileSelector = FindObjectOfType<TileSelector>();

                tileSelector.PieceClickedRPC(gridpoint.x, gridpoint.y);
                tileSelector.ExitState(PiecesThatCanMove[validIndexes[index]]);

                //get possible moves of this piece
                List<Vector2Int> PossibleMoves = MovesForPiece(PiecesThatCanMove[validIndexes[index]]);

                int index2 = Random.Range(0, PossibleMoves.Count); //select random move

                if (PieceAtGrid(PossibleMoves[index2]) == null)
                {
                    AddToUndoList(PiecesThatCanMove[validIndexes[index]]);
                    AddMovesToUndo(1);

                    Move(PiecesThatCanMove[validIndexes[index]], PossibleMoves[index2]);
                }
                else
                {
                    AddToUndoList(PieceAtGrid(PossibleMoves[index2]));
                    AddToUndoList(PiecesThatCanMove[validIndexes[index]]);
                    AddMovesToUndo(2);

                    CapturePieceAt(PossibleMoves[index2]);
                    Move(PiecesThatCanMove[validIndexes[index]], PossibleMoves[index2]);
                }

                thisPlayerTurn = true;
                MoveSelector moveSelector1 = FindObjectOfType<MoveSelector>();
                moveSelector1.MovePieceRPC(PossibleMoves[index2].x, PossibleMoves[index2].y);
                moveSelector1.ExitState();
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(5); //wait for 2 more seconds
            if(thisPlayerTurn)
            {
                yield break; //stop coroutine
            }
            else
            {
                //other player did not responded so make this player winner
                winnerPanel.SetPlayerLevel(0, playerProfile.Lvl);
                winnerPanel.SetPlayerName(0, playerProfile.nm + "(ME)");
                winnerPanel.SetPlayerImage(0, playerProfile.av);
                winnerPanel.SetWinningAmount(0, (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);

                playerProfile.pD.Gld += (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"];
                databaseController.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                UpdateLevelAndXp(true);
                profileSaver.SaveProfile(playerProfile);

                winnerPanel.SetPlayerLevel(1, player2Profile.Lvl);
                winnerPanel.SetPlayerName(1, player2Profile.nm);
                winnerPanel.SetPlayerImage(1, player2Profile.av);
                winnerPanel.SetWinningAmount(1, 0);

                infoPanel.SetText("Other Player left the game you won");
                infoPanel.ShowInfoPanel();
                winnerPanel.Show();
            }
        }
    }

    public void NextPlayer()
    {
        if(thisPlayerTurn==false)
        {
            thisPlayerTurn = true;
        }
        else
        {
            thisPlayerTurn = false;
        }

        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;

        if (!OfflineMode)
        {
            //check if king in check if yes then check if king has valid moves if yes then move that king and disable other pieces
            if (thisPlayerTurn)
            {
                EnPassant();
                KingChecker();
            }
            else
            {
                EnPassant();
                OtherPlayerKingChecker();
            }
            
        }

        if (OfflineMode)
        {
            if (!thisPlayerTurn) //computer taking turn
            {
                AiAgent.instance.MakeMove();
            }
            else
            {
                EnPassant();
                KingChecker();
            }
        }
    }

    public void EnPassant()
    {
        EnPassantMoves.Clear();
        PawnLocations.Clear();

        if((OfflineMode || PhotonNetwork.IsMasterClient) && thisPlayerTurn)
        {
            foreach(Vector2Int grid in OtherPlayerEnPeasent)
            {
                foreach(GameObject piece in white.pieces) //white pawm
                {
                    if(piece.name.Contains("Pawn"))
                    {
                        Vector2Int grid2 = GridForPiece(piece);
                        if(grid2.y==grid.y) //same level
                        {
                            if(grid2.x-grid.x==-1 || grid2.x - grid.x == 1) //right
                            {
                                //check if next block is empty
                                Vector2Int grid3 = new Vector2Int(grid.x, grid.y + 1);
                                if(!PieceAtGrid(grid3)) //no piece at grid
                                {
                                    //add move to pawn
                                    EnPassantMoves.Add(grid3);
                                    PawnLocations.Add(grid2);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (OfflineMode || PhotonNetwork.IsMasterClient && !thisPlayerTurn)
        {
            foreach (Vector2Int grid in MyEnPeasent)
            {
                foreach (GameObject piece in black.pieces) //black pawm
                {
                    if (piece.name.Contains("Pawn"))
                    {
                        Vector2Int grid2 = GridForPiece(piece);
                        if (grid2.y == grid.y) //same level
                        {
                            if (grid2.x - grid.x == -1 || grid2.x - grid.x == 1) //right or left
                            {
                                //check if next block is empty
                                Vector2Int grid3 = new Vector2Int(grid.x, grid.y - 1);
                                if (!PieceAtGrid(grid3)) //no piece at grid
                                {
                                    //add move to pawn
                                    EnPassantMoves.Add(grid3);
                                    PawnLocations.Add(grid2);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if(!PhotonNetwork.IsMasterClient && thisPlayerTurn)
        {
            foreach (Vector2Int grid in OtherPlayerEnPeasent)
            {
                foreach (GameObject piece in black.pieces) //black pawm
                {
                    if (piece.name.Contains("Pawn"))
                    {
                        Vector2Int grid2 = GridForPiece(piece);
                        if (grid2.y == grid.y) //same level
                        {
                            if (grid2.x - grid.x == -1 || grid2.x - grid.x == 1) //right
                            {
                                //check if next block is empty
                                Vector2Int grid3 = new Vector2Int(grid.x, grid.y - 1);
                                if (!PieceAtGrid(grid3)) //no piece at grid
                                {
                                    //add move to pawn
                                    EnPassantMoves.Add(grid3);
                                    PawnLocations.Add(grid2);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (!PhotonNetwork.IsMasterClient && !thisPlayerTurn)
        {
            foreach (Vector2Int grid in MyEnPeasent)
            {
                foreach (GameObject piece in white.pieces) //white pawm
                {
                    if (piece.name.Contains("Pawn"))
                    {
                        Vector2Int grid2 = GridForPiece(piece);
                        if (grid2.y == grid.y) //same level
                        {
                            if (grid2.x - grid.x == -1 || grid2.x - grid.x == 1) //right
                            {
                                //check if next block is empty
                                Vector2Int grid3 = new Vector2Int(grid.x, grid.y + 1);
                                if (!PieceAtGrid(grid3)) //no piece at grid
                                {
                                    //add move to pawn
                                    EnPassantMoves.Add(grid3);
                                    PawnLocations.Add(grid2);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void OtherPlayerKingChecker()
    {
        //check if this player's king is still in check if yes then end the game
        GameObject MyKing = null;
        foreach (GameObject piece in otherPlayer.pieces)
        {
            if (piece.name.Contains("King"))
            {
                MyKing = piece;
            }
        }

        List<GameObject> CurrentPlayerPieces = currentPlayer.pieces;

        Vector2Int MyKingLocation = GridForPiece(MyKing);

        foreach (GameObject piece in CurrentPlayerPieces)
        {
            List<Vector2Int> Moves = null;
            Moves = MovesForPiece(piece);

            foreach (Vector2Int move in Moves)
            {
                Debug.Log("move " + move.y + " " + move.x);
                if (MyKingLocation == move)
                {
                    //illegal move game end
                    //checkmate this player lost

                    infoPanel.SetText("checkmate, you lose");
                    infoPanel.ShowInfoPanel();
                    Checkmate();
                    return;
                }
            }
        }

        Debug.Log("king checker");
        //check if king in check if yes then check if king has valid moves if yes then move that king and disable other pieces
        //is my king under Check?
        MovesForKing.Clear();
        GameObject King = null;
        foreach (GameObject Piece in currentPlayer.pieces)
        {
            if (Piece.name.Contains("King"))
            {
                Debug.Log("King found");
                MovesForKing = MovesForPiece(Piece);
                King = Piece;
            }
        }

        if(MovesForKing.Count>0)
        {
            bool kingUnderCheck = false;

            //disable those moves where king under check
            //get moves of other player
            List<GameObject> OtherPlayerPieces = otherPlayer.pieces;

            Vector2Int KingLocation = GridForPiece(King);
            Debug.Log("king loc" + KingLocation.y + " " + KingLocation.x);
            List<Vector2Int> TempMoves = new List<Vector2Int>();

            foreach (GameObject piece in OtherPlayerPieces)
            {
                List<Vector2Int> Moves = null;
                if (piece.name.Contains("Pawn"))
                {
                    Moves = GameManager.instance.MovesForOtherPiece(piece, true, true);
                }
                else
                {
                    Moves = GameManager.instance.MovesForOtherPiece(piece, false, true);
                }

                Debug.Log("Moves for other piece " + GridForPiece(piece).y + " " + GridForPiece(piece).x);
                foreach (Vector2Int move in Moves)
                {
                    Debug.Log("move " + move.y + " " + move.x);
                    if (KingLocation == move)
                    {
                        kingUnderCheck = true;
                    }
                    foreach (Vector2Int kingmove in MovesForKing)
                    {
                        if (move == kingmove)
                        {
                            //disable this move for the king
                            TempMoves.Add(kingmove);
                            //MovesForKing.Remove(kingmove);
                        }
                    }
                }
            }

            foreach (Vector2Int temp in TempMoves)
            {
                if (MovesForKing.Contains(temp))
                {
                    MovesForKing.Remove(temp);
                }
            }

            if (kingUnderCheck && MovesForKing.Count == 0)
            {
                //checkmate this player won

                infoPanel.SetText("Checkmate Other Player king has no valid moves and is under check");
                infoPanel.ShowInfoPanel();
                SwitchTurn();
                Checkmate();
                return;
            }
            else if (kingUnderCheck && MovesForKing.Count > 0)
            {
                //wait for other player turn
                if (myTimer > 0)
                {
                    if (CountdownCoroutine == null)
                    {
                        CountdownCoroutine = StartCoroutine(CountDownTimer());
                    }
                    else
                    {
                        Debug.Log("routine stopped true");
                        RoutineStopped = true;
                        Timer.Instance.StopTimer();
                        StopCoroutine(CountdownCoroutine);
                        CountdownCoroutine = StartCoroutine(CountDownTimer());
                    }
                }

                Debug.Log("king under check with more moves");
                //enable king only for this player
                TileSelector tileSelector = FindObjectOfType<TileSelector>();

                if (!OfflineMode)
                {
                    tileSelector.PieceClickedRPC(KingLocation.x, KingLocation.y);
                }

                board.SelectPiece(King);
                tileSelector.ExitState(King);

                MoveSelector move = FindObjectOfType<MoveSelector>();
                move.CanCancelMove(false);
                move.HighlightRed(King);
                return;
            }
        }
    }

    public void KingChecker()
    {
        //check if other player's king is still in check if yes then end the game
        GameObject OtherKing = null;
        foreach(GameObject piece in otherPlayer.pieces)
        {
            if (piece.name.Contains("King"))
            {
                OtherKing = piece;
            }
        }

        List<GameObject> CurrentPlayerPieces = currentPlayer.pieces;

        Vector2Int OtherKingLocation = GridForPiece(OtherKing);

        foreach (GameObject piece in CurrentPlayerPieces)
        {
            List<Vector2Int> Moves = null;
            Moves = MovesForPiece(piece);

            Debug.Log("Moves for current piece " + GridForPiece(piece).y + " " + GridForPiece(piece).x);
            foreach (Vector2Int move in Moves)
            {
                Debug.Log("move " + move.y + " " + move.x);
                if (OtherKingLocation == move)
                {
                    //illegal move game end
                    //checkmate other player lost

                    infoPanel.SetText("Illegal Move, king is under check and you won");
                    infoPanel.ShowInfoPanel();
                    Checkmate();
                    return;
                }
            }
        }


        Debug.Log("king checker");
        //check if king in check if yes then check if king has valid moves if yes then move that king and disable other pieces
        //is my king under Check?
        MovesForKing.Clear();
        GameObject King = null;
        foreach (GameObject Piece in currentPlayer.pieces)
        {
            if (Piece.name.Contains("King"))
            {
                Debug.Log("King found");
                MovesForKing = MovesForPiece(Piece);
                King = Piece;
            }
        }

        if(MovesForKing.Count>0)
        {
            bool kingUnderCheck = false;

            foreach(Vector2Int move in MovesForKing)
            {
                Debug.Log("King Moves " + move.y + " " + move.x);
            }

            //disable those moves where king under check
            //get moves of other player
            List<GameObject> OtherPlayerPieces = otherPlayer.pieces;

            Vector2Int KingLocation = GridForPiece(King);
            Debug.Log("king loc" + KingLocation.y + " " + KingLocation.x);
            List<Vector2Int> TempMoves = new List<Vector2Int>();

            foreach (GameObject piece in OtherPlayerPieces)
            {
                List<Vector2Int> Moves = null;
                if (piece.name.Contains("Pawn"))
                {
                    Moves = GameManager.instance.MovesForOtherPiece(piece, true, true);
                }
                else
                {
                    Moves = GameManager.instance.MovesForOtherPiece(piece, false, true);
                }

                Debug.Log("Moves for other piece " + GridForPiece(piece).y + " " + GridForPiece(piece).x);
                foreach (Vector2Int move in Moves)
                {
                    Debug.Log("move " + move.y + " " + move.x);
                    if (KingLocation == move)
                    {
                        kingUnderCheck = true;
                    }
                    foreach (Vector2Int kingmove in MovesForKing)
                    {
                        if (move == kingmove)
                        {
                            //disable this move for the king
                            TempMoves.Add(kingmove);
                            //MovesForKing.Remove(kingmove);
                        }
                    }
                }
            }

            foreach (Vector2Int temp in TempMoves)
            {
                if (MovesForKing.Contains(temp))
                {
                    MovesForKing.Remove(temp);
                }
            }

            if (kingUnderCheck && MovesForKing.Count == 0)
            {
                //checkmate this player lost
                infoPanel.SetText("Checkmate your king has no valid moves and is under check");
                infoPanel.ShowInfoPanel();
                SwitchTurn();
                Checkmate();
                return;
            }
            else if (kingUnderCheck && MovesForKing.Count > 0)
            {
                if (!OfflineMode)
                {
                    if (myTimer > 0)
                    {
                        if (CountdownCoroutine == null)
                        {
                            CountdownCoroutine = StartCoroutine(CountDownTimer());
                        }
                        else
                        {
                            Debug.Log("routine stopped true");
                            RoutineStopped = true;
                            Timer.Instance.StopTimer();
                            StopCoroutine(CountdownCoroutine);
                            CountdownCoroutine = StartCoroutine(CountDownTimer());
                        }
                    }
                }
                Debug.Log("king under check with more moves");
                //enable king only for this player
                TileSelector tileSelector = FindObjectOfType<TileSelector>();

                if (!OfflineMode)
                {
                    tileSelector.PieceClickedRPC(KingLocation.x, KingLocation.y);
                }

                board.SelectPiece(King);
                tileSelector.ExitState(King);

                MoveSelector move = FindObjectOfType<MoveSelector>();
                move.CanCancelMove(false);
                move.HighlightRed(King);
                return;
            }

            if(OfflineMode || PhotonNetwork.IsMasterClient)
            {
                if (!kingUnderCheck && rightCastle) //king not under check then check if king can castle?
                {
                    //check if left or right path is free

                    bool right = true;

                    for (int i = 0; i < 2; i++)
                    {
                        if (pieces[5 + i, 0])
                        {
                            right = false;
                        }
                    }

                    if (right) //no piece to the right 
                    {
                        //check if king is free to move on the next location if yes then add move into moves for king
                        //6,0
                        KingLocation = new Vector2Int(6, 0);

                        foreach (GameObject piece in OtherPlayerPieces)
                        {
                            List<Vector2Int> Moves = null;
                            if (piece.name.Contains("Pawn"))
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, true, true);
                            }
                            else
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, false, true);
                            }

                            Debug.Log("Moves for other piece " + GridForPiece(piece).y + " " + GridForPiece(piece).x);
                            foreach (Vector2Int move in Moves)
                            {
                                Debug.Log("move " + move.y + " " + move.x);
                                if (KingLocation == move)
                                {
                                    kingUnderCheck = true;
                                }
                            }
                        }

                        if (!kingUnderCheck)
                        {
                            //add castle move to king moves
                            MovesForKing.Add(KingLocation);
                        }
                    }
                }

                if (!kingUnderCheck && leftCastle) //king not under check then check if king can castle?
                {
                    //check if left or right path is free

                    bool left = true;

                    for (int i = 0; i < 3; i++)
                    {
                        if (pieces[1 + i, 0])
                        {
                            left = false;
                        }
                    }

                    if (left) //no piece to the left 
                    {
                        //check if king is free to move on the next location if yes then add move into moves for king
                        //2,0
                        KingLocation = new Vector2Int(2, 0);

                        foreach (GameObject piece in OtherPlayerPieces)
                        {
                            List<Vector2Int> Moves = null;
                            if (piece.name.Contains("Pawn"))
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, true, true);
                            }
                            else
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, false, true);
                            }

                            Debug.Log("Moves for other piece " + GridForPiece(piece).y + " " + GridForPiece(piece).x);
                            foreach (Vector2Int move in Moves)
                            {
                                Debug.Log("move " + move.y + " " + move.x);
                                if (KingLocation == move)
                                {
                                    kingUnderCheck = true;
                                }
                            }
                        }

                        if (!kingUnderCheck)
                        {
                            Debug.Log("adding left castle");
                            //add castle move to king moves
                            MovesForKing.Add(KingLocation);
                        }
                    }
                }
            }
            else if(!PhotonNetwork.IsMasterClient)
            {
                if (!kingUnderCheck && rightCastle) //king not under check then check if king can castle?
                {
                    //check if left or right path is free

                    bool right = true;

                    for (int i = 0; i < 3; i++)
                    {
                        if (pieces[1 + i, 7])
                        {
                            right = false;
                        }
                    }

                    if (right) //no piece to the right 
                    {
                        //check if king is free to move on the next location if yes then add move into moves for king
                        //6,0
                        KingLocation = new Vector2Int(2, 7);

                        foreach (GameObject piece in OtherPlayerPieces)
                        {
                            List<Vector2Int> Moves = null;
                            if (piece.name.Contains("Pawn"))
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, true, true);
                            }
                            else
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, false, true);
                            }

                            Debug.Log("Moves for other piece " + GridForPiece(piece).y + " " + GridForPiece(piece).x);
                            foreach (Vector2Int move in Moves)
                            {
                                Debug.Log("move " + move.y + " " + move.x);
                                if (KingLocation == move)
                                {
                                    kingUnderCheck = true;
                                }
                            }
                        }

                        if (!kingUnderCheck)
                        {
                            //add castle move to king moves
                            MovesForKing.Add(KingLocation);
                        }
                    }
                }

                if (!kingUnderCheck && leftCastle) //king not under check then check if king can castle?
                {
                    //check if left or right path is free

                    bool left = true;

                    for (int i = 0; i < 2; i++)
                    {
                        if (pieces[5 + i, 7])
                        {
                            left = false;
                        }
                    }

                    if (left) //no piece to the left 
                    {
                        //check if king is free to move on the next location if yes then add move into moves for king
                        //2,0
                        KingLocation = new Vector2Int(6, 7);

                        foreach (GameObject piece in OtherPlayerPieces)
                        {
                            List<Vector2Int> Moves = null;
                            if (piece.name.Contains("Pawn"))
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, true, true);
                            }
                            else
                            {
                                Moves = GameManager.instance.MovesForOtherPiece(piece, false, true);
                            }

                            Debug.Log("Moves for other piece " + GridForPiece(piece).y + " " + GridForPiece(piece).x);
                            foreach (Vector2Int move in Moves)
                            {
                                Debug.Log("move " + move.y + " " + move.x);
                                if (KingLocation == move)
                                {
                                    kingUnderCheck = true;
                                }
                            }
                        }

                        if (!kingUnderCheck)
                        {
                            //add castle move to king moves
                            MovesForKing.Add(KingLocation);
                        }
                    }
                }

            }
        }
    }

    public void SetRightCstle(bool rightCastle)
    {
        this.rightCastle = rightCastle;
    }

    public bool RightCastle()
    {
        return rightCastle;
    }

    public void SetLeftCstle(bool leftCastle)
    {
        this.leftCastle = leftCastle;
    }

    public bool LeftCastle()
    {
        return leftCastle;
    }

    public List<Vector2Int> KingMoves()
    {
        return MovesForKing;
    }

    public void ExitMatch()
    {
        if (OfflineMode)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("leaving room");
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftLobby()
    {
        PlayerPrefs.SetInt("ii", 1);
        if(!winnerPanel.isActive())
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("leaving lobby");
            PhotonNetwork.LeaveLobby();
        }
        else
        {
            if (!winnerPanel.isActive())
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //end the game
        if (PhotonNetwork.IsMasterClient)
        {
            if (!(bool)PhotonNetwork.CurrentRoom.CustomProperties["me"]) //match ended (this player lost)
            {
                SoundManager.instance.PlayWinner();
                //this is player 1
                winnerPanel.SetPlayerLevel(0, playerProfile.Lvl);
                winnerPanel.SetPlayerName(0, playerProfile.nm + "(ME)");
                winnerPanel.SetPlayerImage(0, playerProfile.av);
                winnerPanel.SetWinningAmount(0, (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);

                playerProfile.pD.Gld += (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"];
                databaseController.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                UpdateLevelAndXp(true);
                profileSaver.SaveProfile(playerProfile);

                winnerPanel.SetPlayerLevel(1, player2Profile.Lvl);
                winnerPanel.SetPlayerName(1, player2Profile.nm);
                winnerPanel.SetPlayerImage(1, player2Profile.av);
                winnerPanel.SetWinningAmount(1, 0);

                winnerPanel.HideRematch();
                infoPanel.SetText("Other Player left the game you won");
                infoPanel.ShowInfoPanel();
                winnerPanel.Show();
            }

            ExitMatch();
                
        }
        else //master client disconnected
        {
            if(!(bool)PhotonNetwork.CurrentRoom.CustomProperties["me"])
            {
                SoundManager.instance.PlayWinner();
                //match not ended(other player left so this player wins)
                //this is player 1
                winnerPanel.SetPlayerLevel(0, playerProfile.Lvl);
                winnerPanel.SetPlayerName(0, playerProfile.nm + "(ME)");
                winnerPanel.SetPlayerImage(0, playerProfile.av);
                winnerPanel.SetWinningAmount(0, (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"]);

                playerProfile.pD.Gld += (int)PhotonNetwork.CurrentRoom.CustomProperties["pz"];
                databaseController.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

                UpdateLevelAndXp(true);
                profileSaver.SaveProfile(playerProfile);

                winnerPanel.SetPlayerLevel(1, player2Profile.Lvl);
                winnerPanel.SetPlayerName(1, player2Profile.nm);
                winnerPanel.SetPlayerImage(1, player2Profile.av);
                winnerPanel.SetWinningAmount(1, 0);

                winnerPanel.HideRematch();
                infoPanel.SetText("Other Player left the game you won");
                infoPanel.ShowInfoPanel();
                winnerPanel.Show();
            }
            ExitMatch();
        }
    }

    public void PlayTap()
    {
        SoundManager.instance.PlayTap();
    }
}