using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : BaseEnemy
{
    // Start is called before the first frame update
    public Animator animator;
    private Vector2 PointA;
    private Vector2 PointB;
    public float attachTime;
    public float hurtSpeed;
    public float tempTime;
    public bool canMove;
    public bool isDead;
    private bool canHurt;
    
    private GameObject players;
    [SerializeField] private Vector2 roamingTarget;
    private void OnEnable() {
        isDead = false;
        canHurt = true;
        canMove = true;
        PointA = transform.Find("PointA").position;
        PointB = transform.Find("PointB").position;
        detected = false;
        rb.simulated = true;
        
    }
    void Start()
    {
        attachTime = 0f;
        target = GameObject.Find("Player");
        roamingTarget = PointA;
        players = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        EventCenter.GetInstance().AddEventListener<Vector2>(this.gameObject.name + " Hurt", Hurt);
        EventCenter.GetInstance().AddEventListener(this.gameObject.name + " Death", Death);
    }

    // Update is called once per frame
    void Update()
    {
        detect();
        roaming();
        if(detected && !isDead)
        {
            if(getDistance(getDirection((Vector2)target.transform.position)) > attackDistance && canMove)
                move(getDirection((Vector2)target.transform.position), chaseSpeed);
                
        }
        tempTime = Time.time;
        if(tempTime < waitTime + attachTime)
        {
            Debug.Log("Bat Waiting");
            canMove = false;
        }
        else{
            canMove = true;
        }

    }



    void roaming()
    {
        Debug.Log("Roaming");
        if(!detected && !isDead)
        {
            move(getDirection(roamingTarget), moveSpeed);
        }

        if(getDistance(getDirection(roamingTarget)) <= 0.1)
        {
            if(roamingTarget == PointA)
                roamingTarget = PointB;
            else
                roamingTarget = PointA;
        }
    }

    public override void detect()
    {
        detected = Physics2D.OverlapCircle((Vector2)this.transform.position, detectRadius, targetLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)this.transform.position, detectRadius);
        // Gizmos.DrawWireSphere(PointA, 1);
        // Gizmos.DrawWireSphere(PointB, 1);
    }
    
    public void Hurt(Vector2 dir)
    {
        if(canHurt)
            StartCoroutine(HurtProcess(dir));
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
            attachTime = Time.time;
            EventCenter.GetInstance().EventTrigger<Vector2>("Player Injured", getDirection(players.transform.position).normalized);
        }

    }

    private void Death()
    {
        Debug.Log("Wolf Death");
        rb.simulated = false;
        players.GetComponent<PlayerController>().setConnectToNull();
        animator.SetTrigger("Death");  
        isDead = true;
        canMove = false;
        canHurt = false;
        
        Invoke("DisableObj",.4f);
    }

    private void DisableObj()
    {
        this.gameObject.SetActive(false);
    }

}
