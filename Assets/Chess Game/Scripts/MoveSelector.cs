using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveSelector : MonoBehaviour
{
    public GameObject moveLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject attackLocationPrefab;

    private GameObject tileHighlight;
    private GameObject movingPiece;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;

    private bool canCanelMove = true;

    private PhotonView photonView;

    private bool ORight, OLeft;

    void Start ()
    {
        this.enabled = false;
        tileHighlight = Instantiate(tileHighlightPrefab, Geometry.PointFromGrid(new Vector2Int(0, 0)),
            Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);

        photonView = gameObject.GetComponent<PhotonView>();
    }

    void Update ()
    {
        //if this player turn else wait for rpc
        if(GameManager.instance.isThisPlayerTurn())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 point = hit.point;
                Vector2Int gridPoint = Geometry.GridFromPoint(point);
                
                if (Input.GetMouseButtonDown(0))
                {
                    // Reference Point 2: check for valid move location
                    if (!moveLocations.Contains(gridPoint))
                    {
                        //cancel move
                        
                        if(canCanelMove)
                        {
                            CancelMove();
                            if (!GameManager.instance.isOfflineMode())
                            {
                                photonView.RPC("CancelMove", RpcTarget.Others);
                            }
                        }
                        return;
                    }

                    tileHighlight.SetActive(true);
                    tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);

                    if (!GameManager.instance.isOfflineMode())
                    {
                        photonView.RPC("OtherPlayerCastle", RpcTarget.Others, GameManager.instance.RightCastle(), GameManager.instance.LeftCastle());
                        photonView.RPC("MovePiece", RpcTarget.Others, gridPoint.x, gridPoint.y);
                    }

                    if(GameManager.instance.isOfflineMode() || PhotonNetwork.IsMasterClient)
                    {
                        if(movingPiece.name.Contains("Pawn"))
                        {
                            //check if moving two moves
                            Vector2Int grid2 = GameManager.instance.GridForPiece(movingPiece);
                            if(gridPoint.y-grid2.y ==2) //yes
                            {
                                GameManager.instance.MyEnPeasent.Add(gridPoint);
                            }
                            else
                            {
                                if (GameManager.instance.EnPassantMoves.Contains(gridPoint) && GameManager.instance.PawnLocations.Contains(grid2))
                                {
                                    Vector2Int grid3 = new Vector2Int(gridPoint.x, gridPoint.y - 1);
                                    if (GameManager.instance.OtherPlayerEnPeasent.Contains(grid3))
                                    {
                                        GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(gridPoint));
                                        GameManager.instance.AddToUndoList(movingPiece);
                                        GameManager.instance.AddMovesToUndo(2);

                                        GameManager.instance.CapturePieceAt(grid3);
                                        GameManager.instance.Move(movingPiece, gridPoint);
                                        ExitState();
                                        return;
                                    }
                                }
                            }
                        }

                        if (GameManager.instance.RightCastle() || GameManager.instance.LeftCastle())
                        {
                            //Debug.Log("disable both castle");
                            if (movingPiece.name.Contains("Rook"))
                            {
                                Vector2Int grid2 = GameManager.instance.GridForPiece(movingPiece);
                                if (grid2.y == 0 && grid2.x == 7)
                                {
                                    GameManager.instance.SetRightCstle(false);
                                }
                                else if (grid2.y == 0 && grid2.x == 0)
                                {
                                    GameManager.instance.SetLeftCstle(false);
                                }
                            }
                            else if (movingPiece.name.Contains("King"))
                            {
                                if (gridPoint.x == 6 && gridPoint.y == 0) //right castle
                                {
                                    GameManager.instance.AddToUndoList(movingPiece); //king
                                    GameManager.instance.AddMovesToUndo(3);
                                    GameManager.instance.Move(movingPiece, gridPoint);
                                    //move castle
                                    Vector2Int grid2 = new Vector2Int(7, 0);
                                    Vector2Int grid1 = new Vector2Int(5, 0);

                                    GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2)); //rook

                                    GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                                }
                                else if (gridPoint.x == 2 && gridPoint.y == 0) //left castle
                                {
                                    GameManager.instance.AddToUndoList(movingPiece);
                                    GameManager.instance.AddMovesToUndo(3);
                                    GameManager.instance.Move(movingPiece, gridPoint);
                                    //move castle
                                    Vector2Int grid2 = new Vector2Int(0, 0);
                                    Vector2Int grid1 = new Vector2Int(3, 0);

                                    GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2));

                                    GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                                }

                                GameManager.instance.AddToUndoCastleList(GameManager.instance.RightCastle());
                                GameManager.instance.AddToUndoCastleList(GameManager.instance.LeftCastle());

                                GameManager.instance.SetRightCstle(false);
                                GameManager.instance.SetLeftCstle(false);

                                ExitState();
                                return;
                            }

                        }
                    }
                    else if(!GameManager.instance.isOfflineMode() && !PhotonNetwork.IsMasterClient)
                    {
                        if (movingPiece.name.Contains("Pawn"))
                        {
                            //check if moving two moves
                            Vector2Int grid2 = GameManager.instance.GridForPiece(movingPiece);
                            if (gridPoint.y - grid2.y == -2) //yes
                            {
                                Debug.Log("other player pawn -2");
                                GameManager.instance.MyEnPeasent.Add(gridPoint);
                            }
                            else
                            {
                                if (GameManager.instance.EnPassantMoves.Contains(gridPoint) && GameManager.instance.PawnLocations.Contains(grid2))
                                {
                                    Vector2Int grid3 = new Vector2Int(gridPoint.x, gridPoint.y + 1);
                                    if (GameManager.instance.OtherPlayerEnPeasent.Contains(grid3))
                                    {
                                        GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(gridPoint));
                                        GameManager.instance.AddToUndoList(movingPiece);
                                        GameManager.instance.AddMovesToUndo(2);

                                        GameManager.instance.CapturePieceAt(grid3);
                                        GameManager.instance.Move(movingPiece, gridPoint);
                                        ExitState();
                                        return;
                                    }
                                }
                            }
                        }

                        if (GameManager.instance.RightCastle() || GameManager.instance.LeftCastle())
                        {
                            //Debug.Log("disable both castle");
                            if (movingPiece.name.Contains("Rook"))
                            {
                                Vector2Int grid2 = GameManager.instance.GridForPiece(movingPiece);
                                if (grid2.y == 7 && grid2.x == 0)
                                {
                                    GameManager.instance.SetRightCstle(false);
                                }
                                else if (grid2.y == 7 && grid2.x == 7)
                                {
                                    GameManager.instance.SetLeftCstle(false);
                                }
                            }
                            else if (movingPiece.name.Contains("King"))
                            {
                                if (gridPoint.x == 2 && gridPoint.y == 7) //right castle
                                {
                                    GameManager.instance.AddToUndoList(movingPiece); //king
                                    GameManager.instance.AddMovesToUndo(3);
                                    GameManager.instance.Move(movingPiece, gridPoint);
                                    //move castle
                                    Vector2Int grid2 = new Vector2Int(0, 7);
                                    Vector2Int grid1 = new Vector2Int(3, 7);

                                    GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2)); //rook

                                    GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                                }
                                else if (gridPoint.x == 6 && gridPoint.y == 7) //left castle
                                {
                                    GameManager.instance.AddToUndoList(movingPiece);
                                    GameManager.instance.AddMovesToUndo(3);
                                    GameManager.instance.Move(movingPiece, gridPoint);
                                    //move castle
                                    Vector2Int grid2 = new Vector2Int(7, 7);
                                    Vector2Int grid1 = new Vector2Int(5, 7);

                                    GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2));

                                    GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                                }

                                GameManager.instance.AddToUndoCastleList(GameManager.instance.RightCastle());
                                GameManager.instance.AddToUndoCastleList(GameManager.instance.LeftCastle());

                                GameManager.instance.SetRightCstle(false);
                                GameManager.instance.SetLeftCstle(false);

                                ExitState();
                                return;
                            }

                        }
                    }

                    if (GameManager.instance.PieceAtGrid(gridPoint) == null)
                    {
                        GameManager.instance.AddToUndoList(movingPiece);
                        GameManager.instance.AddMovesToUndo(1);
                        
                        GameManager.instance.Move(movingPiece, gridPoint);
                        
                    }
                    else
                    {
                        GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(gridPoint));
                        GameManager.instance.AddToUndoList(movingPiece);
                        GameManager.instance.AddMovesToUndo(2);

                        GameManager.instance.CapturePieceAt(gridPoint);
                        GameManager.instance.Move(movingPiece, gridPoint);
                    }
                    // Reference Point 3: capture enemy piece here later
                    ExitState();
                }
            }
            else
            {
                tileHighlight.SetActive(false);
            }
        }
    }

    [PunRPC]
    public void CancelMove()
    {
        GameManager.instance.SetInMoveSelector(false);
        this.enabled = false;

        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }

        GameManager.instance.DeselectPiece(movingPiece);
        TileSelector selector = FindObjectOfType<TileSelector>();
        selector.EnterState();
    }

    public void CanCancelMove(bool canCancel)
    {
        this.canCanelMove = canCancel;
    }


    public void EnterState(GameObject piece)
    {
        GameManager.instance.SetInMoveSelector(true);
        movingPiece = piece;
        this.enabled = true;

        Debug.Log("enter state " + piece.name);
        if(piece.name.Contains("King"))
        {
            moveLocations = GameManager.instance.KingMoves();
        }
        else if(piece.name.Contains("Pawn"))
        {
            moveLocations = GameManager.instance.MovesForPiece(movingPiece);
            Vector2Int grid = GameManager.instance.GridForPiece(piece);
            //add en passant if any
            for(int i=0;i<GameManager.instance.PawnLocations.Count;i++)
            {
                if(GameManager.instance.PawnLocations[i]==grid)
                {
                    moveLocations.Add(GameManager.instance.EnPassantMoves[i]);
                    break;
                }
            }
        }
        else
        {
            moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        }
        
        locationHighlights = new List<GameObject>();

        if (moveLocations.Count == 0)
        {
            Debug.Log("cancel move");
            CancelMove();
        }

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if (GameManager.instance.PieceAtGrid(loc))
            {
                highlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            else
            {
                highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            locationHighlights.Add(highlight);
        }
    }

    public void HighlightRed(GameObject piece)
    {
        GameObject highlight;

        highlight= Instantiate(attackLocationPrefab, Geometry.PointFromGrid(GameManager.instance.GridForPiece(piece)), Quaternion.identity, gameObject.transform);
        locationHighlights.Add(highlight);
    }

    public void ExitState()
    {
        canCanelMove = true;
        GameManager.instance.SetInMoveSelector(false);
        this.enabled = false;
        TileSelector selector = FindObjectOfType<TileSelector>();
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        selector.EnterState();
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
        GameManager.instance.NextPlayer();
    }

    public void ExitStateAI()
    {
        this.enabled = false;
        TileSelector selector = FindObjectOfType<TileSelector>();
        tileHighlight.SetActive(false);
        movingPiece = null;
        selector.EnterState();
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
        GameManager.instance.NextPlayer();
    }

    public void MovePieceRPC(int x, int y)
    {
        photonView.RPC("MovePiece", RpcTarget.Others, x, y);
    }

    [PunRPC]
    private void OtherPlayerCastle(bool right,bool left)
    {
        ORight = right;
        OLeft = left;
    }

    [PunRPC]
    private void MovePiece(int x,int y)
    {
        Vector2Int gridPoint = new Vector2Int(x, y);

        if (!PhotonNetwork.IsMasterClient)
        {
            if (movingPiece.name.Contains("Pawn"))
            {
                //check if moving two moves
                Vector2Int grid2 = GameManager.instance.GridForPiece(movingPiece);
                if (gridPoint.y - grid2.y == 2) //yes
                {
                    GameManager.instance.OtherPlayerEnPeasent.Add(gridPoint);
                }
                else
                {
                    if (GameManager.instance.EnPassantMoves.Contains(gridPoint) && GameManager.instance.PawnLocations.Contains(grid2))
                    {
                        Vector2Int grid3 = new Vector2Int(gridPoint.x, gridPoint.y - 1);
                        if (GameManager.instance.MyEnPeasent.Contains(grid3))
                        {
                            GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(gridPoint));
                            GameManager.instance.AddToUndoList(movingPiece);
                            GameManager.instance.AddMovesToUndo(2);

                            GameManager.instance.CapturePieceAt(grid3);
                            GameManager.instance.Move(movingPiece, gridPoint);
                            ExitState();
                            return;
                        }
                    }
                }
            }

            if (ORight || OLeft)
            {
                if (movingPiece.name.Contains("King"))
                {
                    if (gridPoint.x == 6 && gridPoint.y == 0) //right castle
                    {
                        GameManager.instance.AddToUndoList(movingPiece); //king
                        GameManager.instance.AddMovesToUndo(3);
                        GameManager.instance.Move(movingPiece, gridPoint);
                        //move castle
                        Vector2Int grid2 = new Vector2Int(7, 0);
                        Vector2Int grid1 = new Vector2Int(5, 0);

                        GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2)); //rook

                        GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                        ExitState();
                        return;
                    }
                    else if (gridPoint.x == 2 && gridPoint.y == 0) //left castle
                    {
                        GameManager.instance.AddToUndoList(movingPiece);
                        GameManager.instance.AddMovesToUndo(3);
                        GameManager.instance.Move(movingPiece, gridPoint);
                        //move castle
                        Vector2Int grid2 = new Vector2Int(0, 0);
                        Vector2Int grid1 = new Vector2Int(3, 0);

                        GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2));

                        GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                        ExitState();
                        return;
                    }
                }

            }
        }
        else
        {
            if (movingPiece.name.Contains("Pawn"))
            {
                //check if moving two moves
                Vector2Int grid2 = GameManager.instance.GridForPiece(movingPiece);
                if (gridPoint.y - grid2.y == -2) //yes
                {
                    Debug.Log("other player pawn -2");
                    GameManager.instance.OtherPlayerEnPeasent.Add(gridPoint);
                }
                else
                {
                    if (GameManager.instance.EnPassantMoves.Contains(gridPoint) && GameManager.instance.PawnLocations.Contains(grid2))
                    {
                        Vector2Int grid3 = new Vector2Int(gridPoint.x, gridPoint.y + 1);
                        if (GameManager.instance.MyEnPeasent.Contains(grid3))
                        {
                            GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(gridPoint));
                            GameManager.instance.AddToUndoList(movingPiece);
                            GameManager.instance.AddMovesToUndo(2);

                            GameManager.instance.CapturePieceAt(grid3);
                            GameManager.instance.Move(movingPiece, gridPoint);
                            ExitState();
                            return;
                        }
                    }
                }
            }

            if (ORight || OLeft)
            {
                if (movingPiece.name.Contains("King"))
                {
                    if (gridPoint.x == 2 && gridPoint.y == 7) //right castle
                    {
                        GameManager.instance.AddToUndoList(movingPiece); //king
                        GameManager.instance.AddMovesToUndo(3);
                        GameManager.instance.Move(movingPiece, gridPoint);
                        //move castle
                        Vector2Int grid2 = new Vector2Int(0, 7);
                        Vector2Int grid1 = new Vector2Int(3, 7);

                        GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2)); //rook

                        GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                        ExitState();
                        return;
                    }
                    else if (gridPoint.x == 6 && gridPoint.y == 7) //left castle
                    {
                        GameManager.instance.AddToUndoList(movingPiece);
                        GameManager.instance.AddMovesToUndo(3);
                        GameManager.instance.Move(movingPiece, gridPoint);
                        //move castle
                        Vector2Int grid2 = new Vector2Int(7, 7);
                        Vector2Int grid1 = new Vector2Int(5, 7);

                        GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(grid2));

                        GameManager.instance.Move(GameManager.instance.PieceAtGrid(grid2), grid1);
                        ExitState();
                        return;
                    }
                }
            }
        }

        if (GameManager.instance.PieceAtGrid(gridPoint) == null)
        {
            GameManager.instance.AddToUndoList(movingPiece);
            GameManager.instance.AddMovesToUndo(1);
            GameManager.instance.Move(movingPiece, gridPoint);
        }
        else
        {
            GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(gridPoint));
            GameManager.instance.AddToUndoList(movingPiece);
            GameManager.instance.AddMovesToUndo(2);

            GameManager.instance.CapturePieceAt(gridPoint);
            GameManager.instance.Move(movingPiece, gridPoint);
        }

        ExitState();
    }
}
