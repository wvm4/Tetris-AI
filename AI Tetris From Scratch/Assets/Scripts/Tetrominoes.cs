using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrominoes : MonoBehaviour
{
    public int colour; //colour of tetromino as index of tiles in player script
    public Vector3Int[] blockLocations = new Vector3Int[3]; //4 blocks per tetromino
}
