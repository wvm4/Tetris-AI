using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    //script runs in prefab

    public Tilemap tilemap;
    public Tile[] tiles; //0 = ghost, 1 = Blue, 2 = cyan, 3 = green, 4 = orange, 5 = purple, 6 = red, 7 = yellow
    

    //test vars
    public Vector3Int[] tileLocation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Vector3Int i in tileLocation)
        {
            tilemap.SetTile(i, tiles[1]);
        }
    }
}
