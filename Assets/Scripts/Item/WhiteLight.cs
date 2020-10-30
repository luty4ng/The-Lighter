using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteLight : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController player;
    private void Start() {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(player.isAttacking && other.gameObject.tag == "Lantern")
        {
            EventCenter.GetInstance().EventTrigger<Vector3>("FollowWhiteLight",this.transform.position);
        }
    }
}
