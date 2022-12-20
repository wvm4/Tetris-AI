using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class playerInput : MonoBehaviour
{
    public Piece piece;
    public Player player;
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
            piece.nextRotation = 1;
        } else if (horiInput < 0 && oldHoriInput >= 0){
            piece.nextRotation = 2;
        }
        
        
    }
}
