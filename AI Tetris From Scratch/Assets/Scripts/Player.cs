using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using Unity.Mathematics;

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
    public TetrominoData currentPieceData;
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

        //copying tetromino amounts to bag 2
        tetrominoBag2 = tetrominoBag1;

        //randomizing both bag orders
        System.Random rnd1 = new System.Random();
        System.Random rnd2 = new System.Random();

        tetrominoBag1 = tetrominoBag1.OrderBy(c => rnd1.Next()).ToArray();
        tetrominoBag2 = tetrominoBag2.OrderBy(c => rnd1.Next()).ToArray();
        currentPieceData = tetrominoBag1[0];
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

        //print lines that should be cleared (for testing)

        //for (int i = 0; i < linesToClear.Length; i++)
        //{
        //    if (linesToClear[i])
        //    {
        //        print(linesToClear[i].ToString() + " " + i.ToString());

        //    }
        //}

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

    //piece queue, shifting/shuffling piece bags
    public void NextPiece()
    {
        //count dropped pieces
        pieceCounter++;
        //shift primary bag
        tetrominoBag1 = ShiftTetrominoBag(tetrominoBag1);
        //set current piece to first piece from bag
        currentPieceData = tetrominoBag1[0];

        piece.LoadNextPiece(currentPieceData);

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
