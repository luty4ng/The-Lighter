using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float hurtSpeed;
    // Start is called before the first frame update

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            EventCenter.GetInstance().EventTrigger<Vector2>("Player Injured", new Vector2(0,hurtSpeed));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Bat" || other.gameObject.tag == "Wolf")
        {
            EventCenter.GetInstance().EventTrigger(other.gameObject.name + " Death");
        }
    }
}
