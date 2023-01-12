using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Main : MonoBehaviour
{
    public GameObject player1OBJ;
    public GameObject player2OBJ;
    public Player player1;
    public Player player2;
    public float maxLockTime;
    public float autoFallTime;

    public float timeStarted;
    public float timeMultiplier;
    public float speedMultiplier;
    public float flattenMultiplier;

    public System.Random rnd1;
    public System.Random rnd2;

    public TileBase garbageTile;

    public bool episodeAlreadyStarted;

    // Start is called before the first frame update
    void Start()
    {
        timeStarted = Time.time;




        //GameObject gameObject = Instantiate() to save reference to player
        player1 = Instantiate(player1OBJ, this.transform.position + new Vector3(-10,0,0), Quaternion.identity).GetComponent<Player>();
        player2 = Instantiate(player2OBJ, this.transform.position + new Vector3(8,0,0), Quaternion.identity).GetComponent<Player>();

        player1.main = this;
        player2.main = this;



        player1.opponent = player2;
        player2.opponent = player1;

        if (player1.isAI)
        {
            player1.AI.main = this;
        }
        if (player2.isAI)
        {
            player2.AI.main = this;
        }


        if (!episodeAlreadyStarted)
        {
            episodeAlreadyStarted = true;
            ResetPlayers();
        }

    }

    public void ResetPlayers()
    {
        player1.ResetClass();
        player2.ResetClass();
        player1.piece.gameOver = false;
        player2.piece.gameOver = false;

        timeStarted = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerSpeed(player1, player2);
    }

    public void UpdatePlayerSpeed(Player player1, Player player2)
    {
        float timeRunning = Time.time - timeStarted;

        //timeMultiplier = 0.2f + 1/Mathf.Log(timeRunning/2 + 2f, 2f);
        //timeMultiplier -= (1 / flattenMultiplier * timeRunning) * (timeRunning / speedMultiplier);
        timeMultiplier = (speedMultiplier / (timeRunning + (speedMultiplier * flattenMultiplier) / flattenMultiplier));


        player1.piece.timeMultiplier = timeMultiplier;
        player2.piece.timeMultiplier = timeMultiplier;
    }


}
