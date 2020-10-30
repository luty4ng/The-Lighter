using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlushRock : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController player;
    public Lantern lantern;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        lantern = GameObject.Find("lantern").GetComponent<Lantern>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player")
        {
            if(!player.canlanternJump && !player.coll.onGround)
                player.canlanternJump = true;
        }
    }
}
