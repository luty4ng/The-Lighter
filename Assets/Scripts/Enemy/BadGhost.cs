using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadGhost : BaseEnemy
{
    // Start is called before the first frame update
    private Vector2 PointA;
    private Vector2 PointB;
    private GameObject players;
    [SerializeField] private bool canMove;
    [SerializeField] private Animator anim;

    [SerializeField] private Vector2 roamingTarget;
    private void OnEnable() {
        canMove = true;
        PointA = transform.Find("PointA").position;
        PointB = transform.Find("PointB").position;
    }
    void Start()
    {
        roamingTarget = PointA;
        players = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        EventCenter.GetInstance().AddEventListener(this.gameObject.name + " Death", Death);
        
    }

    // Update is called once per frame
    void Update()
    {
        roaming();

    }



    void roaming()
    {
        if(canMove)
            move(getDirection(roamingTarget), moveSpeed);
        if(getDistance(getDirection(roamingTarget)) <= 0.1)
        {
            if(roamingTarget == PointA)
                roamingTarget = PointB;
            else
                roamingTarget = PointA;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            EventCenter.GetInstance().EventTrigger<Vector2>("Player Injured", getDirection(players.transform.position).normalized);
        }
    }

    void Death()
    {
        StartCoroutine(DeathProcess());
    }

    IEnumerator DeathProcess()
    {
        canMove = false;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }
}
