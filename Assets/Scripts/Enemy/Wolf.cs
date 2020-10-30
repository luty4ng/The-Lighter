using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : BaseEnemy
{

    public float shiftTime;
    public float hurtSpeed;
    public bool canMove;
    private bool canHurt;
    public bool isDead;
    private GameObject players;

    private WolfCollision wolfcoll;

    private Animator animator;
    private void OnEnable() {
        isDead = false;
        detected = false;
        canMove = true;
        canHurt = true;
    }
    void Start()
    {
        roaming();
        target = GameObject.Find("Player");
        players = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        wolfcoll = GetComponent<WolfCollision>();
        EventCenter.GetInstance().AddEventListener<Vector2>(this.gameObject.name + " Hurt", Hurt);
        EventCenter.GetInstance().AddEventListener(this.gameObject.name + " Death", Death);
    }

    // Update is called once per frame
    void Update()
    {
        detect();
        animator.SetBool("Move", detected);
        if(detected && !isDead)
        {
            if(getDistance(getDirection((Vector2)target.transform.position)) > attackDistance && canMove)
                move(getDirection((Vector2)target.transform.position), chaseSpeed);
                
        }
    }

    void roaming()
    {
        if(!detected)
        {
            Debug.Log("Wolf Roaming");
            shiftTime = Random.Range(2.0f, 4.0f);
            InvokeRepeating("shift", 0, shiftTime);
        }  
    }

    void shift()
    {
        if(!detected)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            detectBoxOffset.x = -detectBoxOffset.x;
        }
    }

    public override void detect()
    {
        detected = Physics2D.OverlapBox((Vector2)this.transform.position + detectBoxOffset, detectBoxSize, 0, targetLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)this.transform.position + detectBoxOffset, detectBoxSize);
        // Gizmos.DrawWireSphere(PointA, 1);
        // Gizmos.DrawWireSphere(PointB, 1);
    }
    
    public void Hurt(Vector2 dir)
    {
        if(canHurt)
        {
            if(wolfcoll.onGround && dir.y < 0.1)
                dir.y=0;
            if(!wolfcoll.onWall)
                StartCoroutine(HurtProcess(dir));
        }
    }
    private void Death()
    {
        Debug.Log("Wolf Death");
        players.GetComponent<PlayerController>().setConnectToNull();
        animator.SetTrigger("Death");
        isDead = true;
        canMove = false;
        canHurt = false;
        rb.simulated = false;
        Invoke("DisableObj",.4f);
    }

    private void DisableObj()
    {
        this.gameObject.SetActive(false);
    }
    IEnumerator HurtProcess(Vector2 dir)
    {
        canMove = false;
        move(dir, hurtSpeed);
        yield return new WaitForSeconds(.3f);
        canMove = true;
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            EventCenter.GetInstance().EventTrigger<Vector2>("Player Injured", getDirection(players.transform.position).normalized);
        }

    }
}
