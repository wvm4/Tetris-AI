using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject asdf = Instantiate() to save reference to player
        Instantiate(player, new Vector3(8,0,0), Quaternion.identity);
        Instantiate(player, new Vector3(-8,0,0), Quaternion.identity);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
