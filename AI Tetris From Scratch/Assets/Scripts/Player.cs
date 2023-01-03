using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using Unity.Mathematics;
using JetBrains.Annotations;

public class Player : MonoBehaviour
{
    //script runs in prefab

    public Tilemap tilemap;
    public int[,] field; //playing field as intsd (bool) for ai input
    public Vector2Int fieldSize; //x and y size of field
    public Vector2Int fieldOffset; //offset from tilemap to field var
    public TetrominoData[] tetrominoBag1; //first bag of tetrominoes
    public TetrominoData[] tetrominoBag2; //second bag
    public int pieceCounter; //counts how many pieces have been dropped from first bag, max 7
    //public TetrominoData currentPieceData;
    public bool pieceBeingHeld; 
    public Piece piece;
    public TetrominoData holdingPiece; //piece being held
    public Vector2Int startPosition;
    public RectInt bounds;
    public Main main;



    // Start is called before the first frame update
    void Start()
    {
        field = new int[fieldSize.x, fieldSize.y];
        
        //assigning correct block locations from Data to tetrominoes
        for (int i = 0; i < tetrominoBag1.Length; i++)
        {
            tetrominoBag1[i].Initialize();
        }

        pieceBeingHeld = false;

        //copying tetromino amounts to bag 2
        tetrominoBag2 = tetrominoBag1;

        //randomizing both bag orders
        System.Random rnd1 = new System.Random();
        System.Random rnd2 = new System.Random();

        tetrominoBag1 = tetrominoBag1.OrderBy(c => rnd1.Next()).ToArray();
        tetrominoBag2 = tetrominoBag2.OrderBy(c => rnd1.Next()).ToArray();
        TetrominoData currentPieceData = tetrominoBag1[0];

        //display pieces in queue
        for (int i = 0; i < 4; i++)
        {
            ClearDisplayPiece(i + 1);
            DisplayPiece(tetrominoBag1[i], i + 1);
        }

        piece.LoadFirstPiece(currentPieceData, startPosition);
        pieceCounter = 1;
    }

    public void CheckForClearedLines()
    {
        bool[] linesToClear = new bool[field.GetLength(1)];

        for (int i = 0; i < field.GetLength(1); i++)
        {
            linesToClear[i] = true;

            for (int j = 0; j < field.GetLength(0); j++)
            {
                Vector3Int tileToCheck = new Vector3Int(j + fieldOffset.x, i + fieldOffset.y, 0);
                if (!tilemap.HasTile(tileToCheck))
                {
                    linesToClear[i] = false; 
                    break;
                }
            }
        }


        ClearLines(linesToClear);
    }

    public void ClearLines(bool[] lines)
    {
        int linesCleared = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < fieldSize.x; j++)
            {
                Vector3Int position = new Vector3Int(j + fieldOffset.x, i + fieldOffset.y, 0);
                TileBase tileColor = tilemap.GetTile(position);

                tilemap.SetTile(position, null);
                tilemap.SetTile(position + linesCleared * Vector3Int.down, tileColor);
            }

            if (lines[i])
            {
                linesCleared++;
            } 
        }
    }

    public void HoldPiece()
    {
        if (!pieceBeingHeld)
        {
            pieceBeingHeld = true;
            holdingPiece = tetrominoBag1[0];
            NextPiece();
            DisplayPiece(holdingPiece, 0);
            return;
        }
        ClearDisplayPiece(0);
        TetrominoData tempPiece = tetrominoBag1[0];
        tetrominoBag1[0] = holdingPiece;
        TetrominoData currentPieceData = tetrominoBag1[0];
        holdingPiece = tempPiece;
        piece.LoadNextPiece(currentPieceData);
        DisplayPiece(holdingPiece, 0);

    }

    //display a piece on the screen: 0 = holding piece, 1,2,3,4 = nextpieces
    public void DisplayPiece(TetrominoData piece, int queuePos)
    {
        Vector3Int displayPosition;

        switch(queuePos)
        {
            case 0:
                displayPosition = new Vector3Int(-10, 10, 0);
                break; 
            case 1:
                displayPosition = new Vector3Int(8, 10, 0);
                break;
            case 2:
                displayPosition = new Vector3Int(8, 7, 0);
                break;
            case 3:
                displayPosition = new Vector3Int(8, 4, 0);
                break;
            case 4:
                displayPosition = new Vector3Int(8, 1, 0);
                break;
            default:
                return;
        }


        //set all tiles on the tilemap containing the piece to correct color tile
        foreach (Vector2Int block in piece.blocks)
        {
            Vector3Int blockPosition = new Vector3Int(block.x + displayPosition.x, block.y + displayPosition.y, 0);
            tilemap.SetTile(blockPosition, piece.tile);

            
        }
    }

    //clear piece displayed
    public void ClearDisplayPiece(int queuePos)
    {
        Vector3Int displayPosition;

        switch (queuePos)
        {
            case 0:
                displayPosition = new Vector3Int(-10, 10, 0);
                break;
            case 1:
                displayPosition = new Vector3Int(10, 10, 0);
                break;
            case 2:
                displayPosition = new Vector3Int(10, 7, 0);
                break;
            case 3:
                displayPosition = new Vector3Int(10, 4, 0);
                break;
            case 4:
                displayPosition = new Vector3Int(10, 1, 0);
                break;
            default:
                return;
        }

        displayPosition += new Vector3Int(-3, -3, 0);
        //set all tiles on the tilemap containing the piece to correct color tile
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {

                tilemap.SetTile(new Vector3Int(displayPosition.x + i, displayPosition.y + j, 0), null);
            }
        }
    }

    //piece queue, shifting/shuffling piece bags
    public void NextPiece()
    {
        //count dropped pieces
        pieceCounter++;

        //shift primary bag
        tetrominoBag1 = ShiftTetrominoBag(tetrominoBag1);
        //set current piece to first piece from bag
        TetrominoData currentPieceData = tetrominoBag1[0];
        piece.LoadNextPiece(currentPieceData);

        //display pieces in queue
        for (int i = 0; i < 4; i++)
        {
            ClearDisplayPiece(i + 1);
            DisplayPiece(tetrominoBag1[i], i + 1);
        }

        if (pieceCounter == 7)
        {
            //switch bags and shuffle bag2
            TetrominoData[] tempBag = tetrominoBag1;
            tetrominoBag1 = tetrominoBag2;
            tetrominoBag2 = tempBag;
            tetrominoBag2 = ShuffleTetrominoBag(tetrominoBag2);
            pieceCounter = 0;
        }


    }

    public TetrominoData[] ShiftTetrominoBag(TetrominoData[] bag)
    {
        TetrominoData[] tempBag = bag;

        for (int i = 0; i < bag.Length; i++)
        {
            if (i == bag.Length - 1)
            {
                tempBag[i] = bag[0];
            } else
            {
                tempBag[i] = bag[i + 1];
            }
        }
        
        return tempBag;
    }

    public TetrominoData[] ShuffleTetrominoBag(TetrominoData[] bag)
    {
        System.Random rnd1 = new System.Random();
        bag = bag.OrderBy(c => rnd1.Next()).ToArray();
        return bag;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
