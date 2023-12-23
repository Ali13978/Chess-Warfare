using UnityEngine;

public class Board : MonoBehaviour
{
    public Material selectedMaterial;

    private Material tempMaterial;

    public GameObject AddPiece(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Geometry.GridPoint(col, row);
        GameObject newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform);
        if(newPiece.name.Contains("Black"))
        {
            newPiece.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        }
        return newPiece;
    }

    public void RemovePiece(GameObject piece)
    {
        Destroy(piece);
    }

    public void MovePiece(GameObject piece, Vector2Int gridPoint)
    {
        SoundManager.instance.PlayMovePiece();
        piece.transform.position = Geometry.PointFromGrid(gridPoint);
    }

    public void SelectPiece(GameObject piece)
    {
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();

        tempMaterial = renderers.material;
        renderers.material = selectedMaterial;
    }

    public void DeselectPiece(GameObject piece)
    {
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();

        renderers.material = tempMaterial;
    }
}
