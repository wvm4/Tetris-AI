using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Player mainPlayer;
    public Piece trackingPiece;

    public Tilemap tilemap;
    public Vector2Int[] blocks { get; private set; }
    public Vector2Int position { get; private set; }

    private void Awake()
    {
        blocks = new Vector2Int[4];
    }

    private void LateUpdate()
    {
        if (trackingPiece.active)
        {
            Clear();
            Copy();
            Drop();
            Set();
        }
    }

    private void Clear()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            tilemap.SetTile(new Vector3Int(blocks[i].x + position.x, blocks[i].y + position.y, 0), null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < blocks.Length; i++) {
            blocks[i] = trackingPiece.blocks[i];
        }
    }

    private void Drop()
    {
        Vector2Int position = trackingPiece.position;

        int current = position.y;
        int bottom = -10;

        trackingPiece.ClearPiece(trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if (trackingPiece.IsValidLocation(trackingPiece, position)) {
                this.position = position;
            } else {
                break;
            }
        }

        trackingPiece.SetPiece(trackingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            tilemap.SetTile(new Vector3Int(blocks[i].x + position.x, blocks[i].y + position.y, 0), tile);
        }
    }

}