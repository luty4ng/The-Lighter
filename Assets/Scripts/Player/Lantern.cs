using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Lantern : MonoBehaviour
{
    public float followSpeed;
    public float attackForce;
    public float controlForce;
    public float accerateRate;
    public float maxDistance;

    public bool statusCheck;
    public bool isFollowingLight;
    public bool inYellowLight;
    public bool inWhiteLight;
    
    public PlayerController player;
    public Rigidbody2D rb;
    public GameObject left;
    public GameObject right;

    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 pos; 
    [SerializeField] private ParticleSystem lanternParticle;
    private Vector2 globalDir;
    void Start () {
		//followSpeed = 3.0f;
        isFollowingLight = false;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        rb = this.GetComponent<Rigidbody2D>();
        left = GameObject.Find("FlowingPosLeft");
        right = GameObject.Find("FlowingPosRight");
        EventCenter.GetInstance().AddEventListener<Vector2>("Attack", attackmove);
        EventCenter.GetInstance().AddEventListener("FlushRb", flushSpeedToZero);
        EventCenter.GetInstance().AddEventListener<Vector3>("FollowWhiteLight", ChangePos);
        
	}
	public void ChangePos(Vector3 targetPos)
    {
        pos = targetPos;
        isFollowingLight = true;
    }

	void Update () {
        FollowRotate();
        FollowMove();
    }
    
    void flushSpeedToZero()
    {
        rb.velocity = Vector2.zero;    
    }

    void attackmove(Vector2 dir)
    {
        
        rb.velocity = Vector2.zero;
        if(!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            dir = new Vector2(player.faceDir, dir.y);
        //Camera.main.transform.DOComplete();
        //Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        globalDir = dir.normalized;
        rb.velocity += attackForce*dir.normalized;  
        DOVirtual.Float(12, 0, .8f, RigidbodyDrag);  
        lanternParticle.Play();
        if(isFollowingLight)
            isFollowingLight = false;
        

    }
    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    

    private void FollowRotate()
    {
        if(!isFollowingLight)
        {
            if(player.faceDir == 1)
                pos = left.transform.position;
            else
                pos = right.transform.position;
        }
        Vector3 obj = this.transform.position; 
        direction = pos - obj;
        direction.z = 0f;
    }

    private void FollowMove()
    {
        float distance = Mathf.Sqrt(Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2));
        //Debug.Log(distance);
        if(!player.isAttacking)
            if(Mathf.Abs(direction.x)>0.01 || Mathf.Abs(direction.y)>0.0)
                transform.Translate(direction.normalized * followSpeed * distance * accerateRate * Time.deltaTime);

    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Bat" || other.gameObject.tag == "Wolf")
        {
            if(player.isAttacking)
            {
                EventCenter.GetInstance().EventTrigger<Vector2>(other.gameObject.name + " Hurt", globalDir);
                player.setConnect(other.gameObject.transform);
            }
        }

        if(other.gameObject.tag == "Rock")
        {
            if(player.isAttacking)
            {
                Debug.Log(other.gameObject.name + " Move");
                EventCenter.GetInstance().EventTrigger<Vector2>(other.gameObject.name + " Move", new Vector2(this.rb.velocity.normalized.x,0));
                player.setConnect(other.gameObject.transform);
            }
        }

        if(other.gameObject.tag == "YellowLight")
        {
            inYellowLight = true;
        }

        if(other.gameObject.tag == "WhiteLight")
        {
            inWhiteLight = true;
        }


        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "BadGhost" || other.gameObject.tag == "LostGhost")
        {
            if(player.isAttacking)
                EventCenter.GetInstance().EventTrigger(other.gameObject.name + " Death");
        }

        
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "YellowLight")
        {
            inYellowLight = false;
        }

        if(other.gameObject.tag == "WhiteLight")
        {
            inWhiteLight = false;
        }
    }
    
}
