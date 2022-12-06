using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    //script runs in prefab

    public Tilemap tilemap;
    public Tile[] tiles; //0 = empty, 1 = Blue, 2 = cyan, 3 = green, 4 = orange, 5 = purple, 6 = red, 7 = yellow 8 = ghost
    //2 bags containing 14 pieces
    public TetrominoData[] tetrominoBag1;
    public TetrominoData[] tetrominoBag2;
    public TetrominoData currentPieceData;
    public Piece currentPiece;
    public TetrominoData holdingPiece;
    public Vector2Int startPosition;


    //test vars

    // Start is called before the first frame update
    void Start()
    {


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
        tetrominoBag2 = tetrominoBag2.OrderBy(c => rnd2.Next()).ToArray();
        currentPieceData = tetrominoBag1[0];
        currentPiece.SetPiece(currentPieceData, startPosition);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
