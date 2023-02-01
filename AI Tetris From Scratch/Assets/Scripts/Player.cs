using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using Unity.Mathematics;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Unity.MLAgents;

public class Script : MonoBehaviour
{


}

public class Player : MonoBehaviour
{
    //script runs in prefab

    public Tilemap tilemap;
    public Ghost ghost;
    public int[,] field; //playing field as ints (bool) for ai input
    public int highestPieceHeight; //highest piece of tetris tower, input for ai to know opponent board state
    public Vector2Int fieldSize; //x and y size of field
    public Vector2Int fieldOffset; //offset from tilemap to field var
    public TetrominoData[] tetrominoBagOriginal; //originally sorted bag
    public TetrominoData[] tetrominoBag1; //first bag of tetrominoes
    public TetrominoData[] tetrominoBag2; //second bag
    public TetrominoData currentPieceData; //current piece
    public int pieceCounter; //counts how many pieces have been dropped from first bag, max 7
    //public TetrominoData currentPieceData;
    bool pieceBeingHeld; 
    public Piece piece;
    public TetrominoData holdingPiece; //piece being held
    public Vector2Int startPosition;
    public RectInt bounds;
    public Main main;
    public Player opponent;
    public int garbageRowsToAdd;
    bool lastClearWasTetris;

    public bool firstLoop;

    public bool isAI;
    public TetrisAI AI;
    public float AIStepTime;
    public float clearLineRewardMultiplier;
    public int linesClearedTotal;
    public int[] boardHeightMap;

    public Vector2Int[] data;



    // Start is called before the first frame update


    public void ResetClass()
    {
        if (firstLoop)
        {
            AI.statsRecorder = Academy.Instance.StatsRecorder;
            field = new int[fieldSize.x, fieldSize.y];
            boardHeightMap = new int[fieldSize.x];
            tetrominoBagOriginal = tetrominoBag1;
            //assigning correct block locations from Data to tetrominoes

            for (int i = 0; i < tetrominoBag1.Length; i++)
            {
                tetrominoBag1[i].Initialize();
            }

            //copying tetromino amounts to bag 2

            tetrominoBag2 = tetrominoBag1;
            firstLoop = false;
        }


        tilemap.ClearAllTiles();
        ghost.tilemap.ClearAllTiles();



        tetrominoBag1 = tetrominoBagOriginal;
        tetrominoBag2 = tetrominoBag1;

        pieceBeingHeld = false;


        //randomizing both bag orders
        System.Random rnd1 = new System.Random();
        System.Random rnd2 = new System.Random();

        tetrominoBag1 = tetrominoBag1.OrderBy(c => rnd1.Next()).ToArray();
        tetrominoBag2 = tetrominoBag2.OrderBy(c => rnd2.Next()).ToArray();
        TetrominoData currentPieceData = tetrominoBag1[0];

        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                field[i, j] = 0;
            }
        }

        //display pieces in queue
        for (int i = 0; i < 4; i++)
        {
            ClearDisplayPiece(i + 1);
            DisplayPiece(tetrominoBag1[i], i + 1);
        }

        piece.LoadNextPiece(currentPieceData, startPosition);

        pieceCounter = 1;

        linesClearedTotal = 0;

        piece.ResetVars();

    }

    public void CheckForClearedLines()
    {
        SetFieldVar();

        //set board height map

        //for (int i = 0; i < fieldSize.x; i++)
        //{

        //    for (int j = 0; j < fieldSize.y; j++)
        //    {
        //        if (field[i, fieldSize.y - j - 1] == 1)
        //        {
        //            boardHeightMap[i] = fieldSize.y - j;
        //            break;
        //        }
        //    }

        //}

        bool[] linesToClear = new bool[field.GetLength(1)];
        int[] editedRows = new int[piece.blocks.Length];

        for (int i = 0; i < piece.blocks.Length; i++)
        {
            editedRows[i] = piece.position.y + piece.blocks[i].y - fieldOffset.y;

        }

        //foreach (int row in editedRows)
        //{
        //    print("Edited row " + ": " + row.ToString());
        //}

        for (int i = 0; i < field.GetLength(1); i++)
        {
            linesToClear[i] = true;
            int blocksThisRow = 10;

            for (int j = 0; j < field.GetLength(0); j++)
            {
                Vector3Int tileToCheck = new Vector3Int(j + fieldOffset.x, i + fieldOffset.y, 0);
                if (!tilemap.HasTile(tileToCheck))
                {
                    blocksThisRow--;
                    linesToClear[i] = false; 
                }
            }
            if (blocksThisRow - 7 > 0 && editedRows.Contains(i))
            {
                //print("row: " + i + "reward: " + AI.NormalizeObservationValue(3 * (blocksThisRow - 6), 20 * 9, 0).ToString());
                //print("blocksThisRow reward: " + 15 * AI.NormalizeObservationValue((blocksThisRow - 6), 20 * 3, 0));
                AI.AddReward( 1/12 * (blocksThisRow - 7) * (blocksThisRow - 7));
            }

        }


        ClearLines(linesToClear);
    }

    public void ClearLines(bool[] lines)
    {





        int linesCleared = 0;



        for (int i = 0; i < fieldSize.y; i++)
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

        linesClearedTotal += linesCleared;



        switch(linesCleared)
        {
            case 0:
                opponent.garbageRowsToAdd += 0;
                lastClearWasTetris = false;
                break;
            case 1:
                opponent.garbageRowsToAdd += 0;
                if (piece.trainingMode)
                {
                    //print("clear reward: " + clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 1, 14, 0));
                    AI.AddReward(clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared, 16, 0));
                }
                lastClearWasTetris = false;
                break;
            case 2:
                opponent.garbageRowsToAdd += 1;
                if (piece.trainingMode)
                {
                    //print("clear reward: " + clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 2, 14, 0));
                    AI.AddReward(clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 3, 16, 0));
                }
                lastClearWasTetris = false;
                break;
            case 3:
                opponent.garbageRowsToAdd += 2;
                if (piece.trainingMode)
                {
                    //print("clear reward: " + clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 4, 14, 0));
                    AI.AddReward(clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 7, 16, 0));
                }
                lastClearWasTetris = false;
                break;
            case 4:
                if (lastClearWasTetris)
                {
                    opponent.garbageRowsToAdd += 6;
                    if (piece.trainingMode)
                    {
                        //print("clear reward: " + clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 8, 14, 0));
                        AI.AddReward(clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 12, 16, 0));
                    }
                }
                opponent.garbageRowsToAdd += 4;
                if (piece.trainingMode)
                {
                    //print("clear reward: " + clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 6, 14, 0));
                    AI.AddReward(clearLineRewardMultiplier * AI.NormalizeObservationValue(linesCleared + 10, 16, 0));
                }
                lastClearWasTetris = true;
                break;
        }
        
    }

    public void SetFieldVar()
    {
        try
        {
            for (int i = 0; i < fieldSize.y; i++)
            {
                for (int j = 0; j < fieldSize.x; j++)
                {
                    Vector3Int position = new Vector3Int(j + fieldOffset.x, i + fieldOffset.y, 0);
                    if (tilemap.HasTile(position))
                    {
                        field[j, i] = 1;
                    }
                    else
                    {
                        field[j, i] = 0;
                    }
                }

            }
        } catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
        
    }

    public void ReceiveGarbage()
    {
        int rows = garbageRowsToAdd;

        //move tiles up
        for (int i = 0; i < fieldSize.y + 1; i++)
        {
            for (int j = 0; j < fieldSize.x + 1; j++)
            {
                Vector3Int position = new Vector3Int(j + fieldOffset.x, fieldSize.y - i + fieldOffset.y, 0);

                if (tilemap.HasTile(position))
                {
                    TileBase tile = tilemap.GetTile(position);

                    tilemap.SetTile(position, null);
                    tilemap.SetTile(position + new Vector3Int(0, rows, 0), tile);
                }
            }

        }

        //fill bottom rows with garbage
        System.Random rand = new System.Random();

        for (int i = 0; i < rows; i++)
        {
            int column = rand.Next(0, 9);

            for (int j = 0; j < fieldSize.x; j++)
            {
                Vector3Int position = new Vector3Int(j + fieldOffset.x, i + fieldOffset.y, 0);

                if (j != column)
                {
                    tilemap.SetTile(position, main.garbageTile);
                }
            }

        }
        CheckGameOver();

        SetFieldVar();
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
        piece.LoadNextPiece(currentPieceData, startPosition);
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
                displayPosition = new Vector3Int(7, 10, 0);
                break;
            case 2:
                displayPosition = new Vector3Int(7, 6, 0);
                break;
            case 3:
                displayPosition = new Vector3Int(7, 2, 0);
                break;
            case 4:
                displayPosition = new Vector3Int(7, -2, 0);
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
                displayPosition = new Vector3Int(7, 10, 0);
                break;
            case 2:
                displayPosition = new Vector3Int(7, 6, 0);
                break;
            case 3:
                displayPosition = new Vector3Int(7, 2, 0);
                break;
            case 4:
                displayPosition = new Vector3Int(7, -2, 0);
                break;
            default:
                return;
        }

        displayPosition += new Vector3Int(-2, -2, 0);
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
        currentPieceData = tetrominoBag1[0];
        piece.LoadNextPiece(currentPieceData, startPosition);

        //display pieces in queue

        for (int i = 1; i < 5; i++)
        {
            ClearDisplayPiece(i);
            DisplayPiece(tetrominoBag1[i], i);
        }

        if (pieceCounter == tetrominoBag1.Length)
        {
            //switch bags and shuffle bag2
            TetrominoData[] tempBag = tetrominoBag1;
            tetrominoBag1 = tetrominoBag2;
            tetrominoBag2 = tempBag;
            tetrominoBag2 = ShuffleTetrominoBag(tetrominoBag2);
            pieceCounter = 0;
        }

        //testing field var

        //for (int i = 0; i < field.GetLength(1); i++)
        //{
        //    for (int j = 0; j < field.GetLength(0); j++)
        //    {
        //        print("x: " + j + ", y: " + i + " = " + field[j, i]);
        //    }
        //}

    }

    public void CheckGameOver()
    {
        for (int i = 0; i < field.GetLength(1); i++)
        {
            bool blocksThisRow = false;

            for (int j = 0; j < field.GetLength(0); j++)
            {
                if (field[j, i] == 1)
                {
                    blocksThisRow = true;
                }
            }

            if (!blocksThisRow)
            {
                highestPieceHeight = i - 1;
                break;
            }
        }


        if (highestPieceHeight > 19)
        {
            GameOver();
        }
        //print(NormalizeObservationValue(19 - highestPieceHeight, 50 * 10, 0));
        //AI.AddReward(AI.NormalizeObservationValue(19 - highestPieceHeight, 50 * 10, 0));

    }



    public void GameOver()
    {
        piece.gameOver = true;

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

}
