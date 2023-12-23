using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAgent : MonoBehaviour
{
    public static AiAgent instance;
    bool timerRunning = false;
    private Material myPieceMaterial;

    void Awake()
    {
        instance = this;
    }

    public void SetMyMaterial(Material AMaterial)
    {
        myPieceMaterial = AMaterial;
    }
    
    public void MakeMove() //called when ai agent is taking turn
    {
        StartCoroutine(ArtificialTimer());
        //get pieces that can move 
        List<GameObject> PiecesThatCanMove = GameManager.instance.currentPlayer.pieces; //current player pieces

        List<int> validIndexes = new List<int>();
        int temp = 0;

        foreach(GameObject piece in PiecesThatCanMove)
        {
            if(GameManager.instance.MovesForPiece(piece).Count>0)
            {
                validIndexes.Add(temp);
            }
            temp++;
        }

        EvaluateMove(validIndexes, PiecesThatCanMove);
    }

    IEnumerator ArtificialTimer()
    {
        timerRunning = true;
        yield return new WaitForSeconds(2f);
        timerRunning = false;
    }

    IEnumerator MovePiece(GameObject Apiece,Vector2Int gridpoint)
    {
        yield return new WaitUntil(() => timerRunning == false);

        if (GameManager.instance.PieceAtGrid(gridpoint) == null) //jha move krna
        {
            GameManager.instance.AddToUndoList(Apiece);
            GameManager.instance.AddMovesToUndo(1);

            GameManager.instance.Move(Apiece, gridpoint);
        }
        else
        {
            //calculate EnPeasent
            if(Apiece.name.Contains("Pawn"))
            {
                Vector2Int grid2 = GameManager.instance.GridForPiece(Apiece);
                if(grid2.y-gridpoint.y==2) //pawn moved two moves 
                {
                    GameManager.instance.OtherPlayerEnPeasent.Add(gridpoint);
                }
                else if(grid2.y-gridpoint.y==1) //pawn moved one move
                {
                    GameManager.instance.OtherPlayerEnPeasent.Remove(GameManager.instance.GridForPiece(Apiece));
                }
            }

            GameManager.instance.AddToUndoList(GameManager.instance.PieceAtGrid(gridpoint));
            GameManager.instance.AddToUndoList(Apiece);
            GameManager.instance.AddMovesToUndo(2);

            GameManager.instance.CapturePieceAt(gridpoint);
            GameManager.instance.Move(Apiece, gridpoint);
        }

        TileSelector selector = FindObjectOfType<TileSelector>();
        selector.ExitState(Apiece);

        MoveSelector moveSelector = FindObjectOfType<MoveSelector>();
        moveSelector.ExitStateAI();

        MeshRenderer renderers = Apiece.GetComponentInChildren<MeshRenderer>();
        renderers.material = myPieceMaterial;
    }

    private void EvaluateMove(List<int> validIndexes, List<GameObject> PiecesThatCanMove)
    {
        //check if other player's king is still in check if yes then end the game
        GameObject OtherKing = null;
        foreach (GameObject piece in GameManager.instance.otherPlayer.pieces)
        {
            if (piece.name.Contains("King"))
            {
                OtherKing = piece;
            }
        }

        List<GameObject> CurrentPlayerPieces = GameManager.instance.currentPlayer.pieces;

        Vector2Int OtherKingLocation = GameManager.instance.GridForPiece(OtherKing);

        foreach (GameObject piece in CurrentPlayerPieces)
        {
            List<Vector2Int> Moves = null;
            if (piece.name.Contains("Pawn"))
            {
                Moves = GameManager.instance.MovesForPiece(piece);
            }
            else
            {
                Moves = GameManager.instance.MovesForPiece(piece);
            }

            foreach (Vector2Int move in Moves)
            {
                Debug.Log("move " + move.y + " " + move.x);
                if (OtherKingLocation == move)
                {
                    //illegal move game end
                    //checkmate other player lost
                    Debug.Log("illegal");
                    InfoPanel infoPanel = FindObjectOfType<InfoPanel>();
                    infoPanel.SetText("illegal move your king is under check you lose");
                    infoPanel.ShowInfoPanel();
                    //GameManager.instance.SwitchTurn();
                    GameManager.instance.Checkmate();
                    return;
                }
            }
        }

        //is my king under Check?
        List <Vector2Int> MovesForKing=new List<Vector2Int>();
        GameObject King=null;
        foreach(GameObject Piece in PiecesThatCanMove)
        {
            if(Piece.name.Contains("King"))
            {
                MovesForKing = GameManager.instance.MovesForPiece(Piece);
                King = Piece;
                Debug.Log("ai king");
            }
        }

        if(MovesForKing.Count>0)
        {
            bool kingUnderCheck = false;

            //disable those moves where king under check
            //get moves of other player
            List<GameObject> OtherPlayerPieces = GameManager.instance.otherPlayer.pieces;

            Vector2Int KingLocation = GameManager.instance.GridForPiece(King);
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

            foreach (Vector2Int move in MovesForKing)
            {
                Debug.Log(move.y + " " + move.x);
            }

            if (kingUnderCheck && MovesForKing.Count == 0)
            {
                //checkmate AI Agent lose
                InfoPanel infoPanel = FindObjectOfType<InfoPanel>();
                infoPanel.SetText("Checkmate Other Player king has no valid moves and is under check");
                infoPanel.ShowInfoPanel();
                GameManager.instance.SwitchTurn();
                GameManager.instance.Checkmate();
                return;
            }
            else if (kingUnderCheck && MovesForKing.Count > 0)
            {
                //move king to the safe position
                //MovesForKing

                int index = Random.Range(0, MovesForKing.Count);
                StartCoroutine(MovePiece(King, MovesForKing[index]));
                return;
            }
        }
        

        //Give numbers to pieces
        List<int> MyPieceValues = new List<int>();

        for(int i=0;i<validIndexes.Count;i++)
        {
            if(PiecesThatCanMove[validIndexes[i]].name.Contains("Pawn"))
            {
                MyPieceValues.Add(10);
            }
            else if (PiecesThatCanMove[validIndexes[i]].name.Contains("Knight"))
            {
                MyPieceValues.Add(25);
            }
            else if (PiecesThatCanMove[validIndexes[i]].name.Contains("Bishop"))
            {
                MyPieceValues.Add(35);
            }
            else if (PiecesThatCanMove[validIndexes[i]].name.Contains("Rook"))
            {
                MyPieceValues.Add(50);
            }
            else if (PiecesThatCanMove[validIndexes[i]].name.Contains("Queen"))
            {
                MyPieceValues.Add(90);
            }
            else if (PiecesThatCanMove[validIndexes[i]].name.Contains("King"))
            {
                MyPieceValues.Add(900);
            }
        }

        //check if other player's pieces are getting killed by this player's pieces

        //first get all possible moves

        List <List<Vector2Int>> AllMovesofMoves = new List<List<Vector2Int>>();

        //All Scores of possible moves
        List<List<int>> AllMoveValues = new List<List<int>>(); //list of list

        for(int i=0;i<validIndexes.Count;i++)
        {
            List<Vector2Int> PossibleMoves = GameManager.instance.MovesForPiece(PiecesThatCanMove[validIndexes[i]]);
            AllMovesofMoves.Add(PossibleMoves);

            List<int> moveValues = new List<int>();
            //check if any piece is getting killed in these moves
            for(int j=0;j<PossibleMoves.Count;j++)
            {
                if(GameManager.instance.PieceAtGrid(PossibleMoves[j])!=null)
                {
                    //a piece is getting killed remember this piece score and current piece that can kill plus its value
                    GameObject APiece = GameManager.instance.PieceAtGrid(PossibleMoves[j]);
                    if (APiece.name.Contains("Pawn"))
                    {
                        moveValues.Add(MyPieceValues[i] - 100);
                    }
                    else if (APiece.name.Contains("Knight"))
                    {
                        moveValues.Add(MyPieceValues[i] - 250);
                    }
                    else if (APiece.name.Contains("Bishop"))
                    {
                        moveValues.Add(MyPieceValues[i] - 350);
                    }
                    else if (APiece.name.Contains("Rook"))
                    {
                        moveValues.Add(MyPieceValues[i] - 500);
                    }
                    else if (APiece.name.Contains("Queen"))
                    {
                        moveValues.Add(MyPieceValues[i] - 900);
                    }
                    else if (APiece.name.Contains("King"))
                    {
                        moveValues.Add(MyPieceValues[i] - 9000);
                    }
                }
                else
                {
                    //if nothing getting killed then its value is highest
                    moveValues.Add(MyPieceValues[i]);
                }
            }
            AllMoveValues.Add(moveValues);
        }

        //lowest value moves are best
        //get this lowest value from allmovevalues list
        int indexofPiece=0; //lowest
        int indexofMove=0; //lowest
        int lowestValue = 10000;

        for(int i=0;i<AllMoveValues.Count;i++)
        {
            List<int> templist = AllMoveValues[i];
            
            for(int j=0;j<templist.Count;j++)
            {
                if(lowestValue>=templist[j])
                {
                    lowestValue = templist[j];
                    indexofPiece = i;
                    indexofMove = j;
                }
            }
        }

        //if 2 or more piece moves have same score then select random from them so it does not look same everytime
        List<int> SamePieceIndexes = new List<int>();
        List<int> MoveIndexes = new List<int>();
        for(int i=0;i<AllMoveValues.Count;i++)
        {
            List<int> templist = AllMoveValues[i];

            for (int j = 0; j < templist.Count; j++)
            {
                if (lowestValue == templist[j])
                {
                    SamePieceIndexes.Add(i);
                    MoveIndexes.Add(j);
                }
            }
        }

        List<Vector2Int> moves = null;

        if (SamePieceIndexes.Count>1) //means 2 or more pieces have same values or 2 or more moves
        {
            indexofPiece = Random.Range(0, SamePieceIndexes.Count);
            indexofMove = MoveIndexes[indexofPiece];
            indexofPiece = SamePieceIndexes[indexofPiece];
            moves = AllMovesofMoves[indexofPiece];
        }
        else
        {
            moves = AllMovesofMoves[indexofPiece];
        }

        //Check if my pieces are getting killed (we can save those who can move i.e piecethatcanmove list)
        //protect the highest first i.e king queen and so on 
        //set enemy piece value

        List<GameObject> MyPiecesGettingKilled = new List<GameObject>();
        List<Vector2Int> PositionsWhereGettingKilled = new List<Vector2Int>(); //so we should not move on these places

        foreach (GameObject EnemyPiece in GameManager.instance.otherPlayer.pieces)
        {
            Debug.Log("enemy piece "+ EnemyPiece.name);

            List<Vector2Int> PossibleMoves = GameManager.instance.MovesForOtherPiece(EnemyPiece,false,false);
            if (PossibleMoves.Count > 0)
            {
                Debug.Log("can move");
                //check if any piece is getting killed by enemy
                foreach (Vector2Int mov in PossibleMoves)
                {
                    if (GameManager.instance.PieceAtGrid(mov) != null) //piece getting killed
                    {
                        PositionsWhereGettingKilled.Add(mov);
                        //our piece is getting killed store its score so we can save it
                        GameObject APiece = GameManager.instance.PieceAtGrid(mov);
                        MyPiecesGettingKilled.Add(APiece);
                    }
                }
            }
        }


        List<int> LowestValues = new List<int>();
        List<GameObject> PieceICanSave = new List<GameObject>();

        for(int i=0;i<MyPiecesGettingKilled.Count;i++)
        {
            //can we save this piece?
            if(GameManager.instance.MovesForPiece(MyPiecesGettingKilled[i]).Count>0)
            {
                PieceICanSave.Add(MyPiecesGettingKilled[i]);

                //we can save this piece
                if (MyPiecesGettingKilled[i].name.Contains("Pawn"))
                {
                    LowestValues.Add(-100);
                }
                else if (MyPiecesGettingKilled[i].name.Contains("Knight"))
                {
                    LowestValues.Add(-250);
                }
                else if (MyPiecesGettingKilled[i].name.Contains("Bishop"))
                {
                    LowestValues.Add(-350);
                }
                else if (MyPiecesGettingKilled[i].name.Contains("Rook"))
                {
                    LowestValues.Add(-500);
                }
                else if (MyPiecesGettingKilled[i].name.Contains("Queen"))
                {
                    LowestValues.Add(-900);
                }
            }
        }

        int enemyLowestIndex = 0;
        int enemyLowest = 10000;
        for(int i=0;i<LowestValues.Count;i++)
        {
            if(enemyLowest>LowestValues[i])
            {
                enemyLowest = LowestValues[i];
                enemyLowestIndex = i;
            }
        }

        if (enemyLowest < lowestValue)
        {
            //get moves of piece
            List<Vector2Int> PossibleMoves = GameManager.instance.MovesForPiece(PieceICanSave[enemyLowestIndex]);
            List<Vector2Int> IcanMovetoThis = PossibleMoves;
            for (int i = 0; i < PositionsWhereGettingKilled.Count; i++)
            {
                for (int j = 0; j < PossibleMoves.Count; j++)
                {
                    if (PossibleMoves[j] == PositionsWhereGettingKilled[i])
                    {
                        IcanMovetoThis.Remove(PossibleMoves[j]);
                    }
                }
            }

            int index = Random.Range(0, IcanMovetoThis.Count);
            //move piece to safe position

            StartCoroutine(MovePiece(PieceICanSave[enemyLowestIndex], IcanMovetoThis[index]));
            return;
        }
        else
        {
            //move the piece
            Debug.Log("move type 3");
            StartCoroutine(MovePiece(PiecesThatCanMove[validIndexes[indexofPiece]],moves[indexofMove]));
            return;
        }
    }
}