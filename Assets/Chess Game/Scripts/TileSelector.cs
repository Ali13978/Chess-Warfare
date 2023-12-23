using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileSelector : MonoBehaviour
{
    public GameObject tileHighlightPrefab;

    private GameObject tileHighlight;

    private PhotonView photonView;

    void Start ()
    {
        Vector2Int gridPoint = Geometry.GridPoint(0, 0);
        Vector3 point = Geometry.PointFromGrid(gridPoint);
        tileHighlight = Instantiate(tileHighlightPrefab, point, Quaternion.identity, gameObject.transform);
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
                    GameObject selectedPiece = GameManager.instance.PieceAtGrid(gridPoint);
                    if (GameManager.instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
                    {
                        if(!GameManager.instance.isOfflineMode())
                        {
                            photonView.RPC("PieceClicked", RpcTarget.Others, gridPoint.x, gridPoint.y);
                        }
                        GameManager.instance.SelectPiece(selectedPiece);
                        // Reference Point 1: add ExitState call here later
                        ExitState(selectedPiece);
                    }
                }
            }
            else
            {
                tileHighlight.SetActive(false);
            }
        }
    }

    public void EnterState()
    {
        enabled = true;
        GameManager.instance.SetInTileSelector(true);
    }

    public void ExitState(GameObject movingPiece)
    {
        GameManager.instance.SetInTileSelector(false);
        this.enabled = false;
        tileHighlight.SetActive(false);
        MoveSelector move = FindObjectOfType<MoveSelector>();
        move.EnterState(movingPiece);
    }

    public void PieceClickedRPC(int x,int y)
    {
        photonView.RPC("PieceClicked", RpcTarget.Others, x, y);
    }

    [PunRPC]
    private void PieceClicked(int x,int y)
    {
        Vector2Int gridPoint = new Vector2Int(x, y);
        GameObject selectedPiece = GameManager.instance.PieceAtGrid(gridPoint);
        if (GameManager.instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
        {
            GameManager.instance.SelectPiece(selectedPiece);

            ExitState(selectedPiece);
        }
    }
}
