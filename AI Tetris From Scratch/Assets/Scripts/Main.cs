using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject player;
    public Player player1;
    public Player player2;
    public float maxLockTime;
    public float autoFallTime;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject gameObject = Instantiate() to save reference to player
        player1 = Instantiate(player, new Vector3(6,0,0), Quaternion.identity).GetComponent<Player>();
        //player2 = Instantiate(player, new Vector3(-8,0,0), Quaternion.identity).GetComponent<Player>();

        player1.main = this;
        //player2.main = this;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
