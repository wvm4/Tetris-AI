using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class playerInput : MonoBehaviour
{
    public Piece piece;
    public Player player;
    public KeyCode rotateLeft;
    public KeyCode rotateRight;
    public int nextRotation;
    public int nextMovement;
    public int nextDrop;
    private float horiInput;
    private float vertiInput;
    private float oldHoriInput;
    private float oldVertiInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        oldHoriInput = horiInput;
        oldVertiInput = vertiInput;
        horiInput = Input.GetAxis("Horizontal");
        vertiInput = Input.GetAxis("Vertical");

        if (horiInput > 0 && oldHoriInput <= 0)
        {
            piece.nextMovement = 1;
        } else if (horiInput < 0 && oldHoriInput >= 0){
            piece.nextMovement = -1;
        }

        if (vertiInput > 0 && oldVertiInput <= 0)
        {
            piece.nextDrop = 1;
        }

        if (Input.GetKeyDown(rotateRight))
        {
            piece.nextRotation = 1;
        } else if (Input.GetKeyDown(rotateLeft))
        {
            piece.nextRotation = -1;
        }
        

    }
}
