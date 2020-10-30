using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public CollisionController coll;
    public GameObject visual;
    public Animator anim;
    public BetterJumping betterJump;
    private Lantern lantern;
    [Space]
    [Header("Stats")]

    public float jumpForce;
    public float TicForce;
    public float speed;
    public float dashSpeed;
    public float injuredSpeed;
    public float slideSpeed;
    public float ticLerp;
    public float attackLerp;
    public float invincTime;
    public float lanternJumpMulti;
    public float lastSlideTime;

    [SerializeField] private float numOfGhost;
    [SerializeField] private float numOfSplit;
    public double Health;


    [Space]
    [Header("Booleans")]
    public bool isAttacking;
    public bool isDashing;
    public bool isSliding;
    public bool isTicing;
    public bool isHurting;
    public bool isInvinc;

    [Space]
    [Header("Connector")]
    [SerializeField] private Transform connector;

    [Space]
    public bool hasDashed;
    public bool hasTiced;
    public bool canlanternJump;
    public bool onGround;

    
    [Space]
    public float faceDir;

    [Header("Partical")]
    [SerializeField] private Vector2 lastPos;
    [SerializeField] private ParticleSystem dashParticle;
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private ParticleSystem TicParticle;
    [SerializeField] private ParticleSystem slideParticleRight;
    [SerializeField] private ParticleSystem slideParticleLeft;
    // Start is called before the first frame update
    void Start()
    {
        lantern = GameObject.Find("lantern").GetComponent<Lantern>();
        coll = this.GetComponent<CollisionController>();
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponentInChildren<Animator>();
        betterJump = this.GetComponent<BetterJumping>();
        visual = GameObject.Find("Visual");
        EventCenter.GetInstance().AddEventListener<Vector2>("Player Injured", Injured);
        EventCenter.GetInstance().AddEventListener<float>("Add Ghost", AddGhost);
    }

    void OnEnable() {
        
        faceDir = 1;
        Health = 3.0;
        canlanternJump = true;

        //invincTime = 0.5f;
        hasDashed = false;
        hasTiced = false;
        isSliding = false;
        isDashing = false;
        isTicing = false;
        isHurting = false;
        isInvinc = false;

    }
    public void AddSplit(float value)
    {
        numOfSplit += value;
        EventCenter.GetInstance().EventTrigger("ExtendTime");
    }
    public void AddGhost(float value)
    {
        numOfGhost += value;
    }

    public void RemoveGhost(float value)
    {
        numOfGhost -= value;
    }

    public void RemoveSplit(float value)
    {
        numOfGhost -= value;
        
    }

    public void setConnect(Transform location)
    {
        //Instantiate(connectLight, transform.position, transform.rotation);
        connector = location;
    }
    public void setConnectToNull()
    {
        connector = null;
        //Destroy(connectLight,1f);
    }

    void Switch(ref Transform target)
    {
        Vector2 tmp = target.position;
        target.position = this.transform.position;
        this.transform.position = tmp;
        setConnectToNull();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Lantern")
        {
            if(Input.GetButtonDown("Jump") && canlanternJump && !coll.onGround)
            {
                anim.SetTrigger("Jump");
                if(!lantern.isFollowingLight)
                    canlanternJump = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                StartCoroutine(disableBetterJump(.3f));
                Jump(lanternJumpMulti*jumpForce);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
      	float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");  
        Vector2 pos = new Vector2(x,y);
        Vector2 dir = new Vector2(xRaw, yRaw);
        lastPos = GameObject.Find("StartPoint").transform.position;

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", y);
        anim.SetFloat("VelocityY", rb.velocity.y);
        anim.SetBool("isHurting", isHurting);

        

        if(xRaw != 0 && !isAttacking && !isDashing)  
        {   
            if(xRaw > 0)
                faceDir = 1;
            else
                faceDir = -1;
            visual.transform.localScale = new Vector3(faceDir, 1, 1);
        }

        
        //Debug.Log(Time.deltaTime);
        checkGround();
        checkVelocity();
        if(lantern.isFollowingLight)
            canlanternJump = true;
        Run(pos);

        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isDashing", isDashing);  
        anim.SetBool("isAttacking", isAttacking); 

        if(Input.GetKeyDown(KeyCode.C) && connector!=null)
            Switch(ref connector);

        if(Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("Jump");
            if(coll.onGround)
                Jump(jumpForce);
            if(!coll.onGround && coll.onWall)
                Tic();
        }


        if(Input.GetKeyDown(KeyCode.Z) && !hasDashed)
        {
            /*
            if(dir.y!=0 || dir.x>=0 || dir.x<0)
                Dash(dir);
            */
            DashParallel(dir);
        }

        if(!coll.onGround && coll.onWall)
        {
            if(Input.GetKey(coll.wallSide==-1?KeyCode.LeftArrow:KeyCode.RightArrow) && !isTicing)
                Slide();
            if(Input.GetKeyUp(coll.wallSide==-1?KeyCode.LeftArrow:KeyCode.RightArrow))
                isSliding = false;
        }


        if(Input.GetKeyDown(KeyCode.X))
        {

            if(Input.GetKey(KeyCode.UpArrow))
                StartCoroutine(attack("AttackUp",1));
            else if(Input.GetKey(KeyCode.DownArrow))
                StartCoroutine(attack("AttackDown",-1));
            else
                StartCoroutine(attack("Attack",0));
        }
    }
    

    void Run(Vector2 pos)
    {
        /*
        if (isAttacking)
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(pos.x * speed/2, 0)), attackLerp * Time.deltaTime);
            */
        //if(isDashing && rb.velocity.y < 0)
            //rb.velocity = new Vector2(rb.velocity.x, 0);
        if(isTicing)
            return;

        if (!isDashing && !isSliding && !hasTiced && !isAttacking && !isHurting)
            rb.velocity = new Vector2(pos.x*speed, rb.velocity.y);
        if(hasTiced)
        {
            Debug.Log(ticLerp * Time.deltaTime);
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(pos.x * speed, rb.velocity.y)), ticLerp * Time.deltaTime);
        }
    }

    void Jump(float force)
    {     
        Debug.Log("Jump");
        rb.velocity = new Vector2(rb.velocity.x, force);
        jumpParticle.Play();
    }

    
    IEnumerator attack(string type, float vertical)
    {
        float tmpDir = faceDir;
        if(vertical!=0)
        {
            if(!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                tmpDir = 0;
        }
        
        Vector2 lantern_dir = new Vector2(tmpDir, vertical);
        EventCenter.GetInstance().EventTrigger<Vector2>("Attack", lantern_dir);
        // 短暂禁用移动、跳跃和其他移动技能
        isAttacking = true;
        // 如果按住方向键则向面朝方向移动小段距离
        anim.SetTrigger(type);
        //sword.GetComponent<SwordController>().attack();
        yield return new WaitForSeconds(.35f);
        isAttacking = false;
        EventCenter.GetInstance().EventTrigger("FlushRb");
    }

    void checkVelocity()
    {
        if(!isSliding)
            lastSlideTime = Time.time;
        if(!isDashing && !isSliding)
            rb.gravityScale = 3;
        if(isSliding)
            isDashing = false;
        if(coll.onGround || !coll.onWall)
            isSliding = false;
        if(rb.velocity.y<=0)
        {
            rb.gravityScale = 3;
            GetComponent<BetterJumping>().enabled = true;
        }
        if(isTicing)
        {
            if(faceDir==1)
                slideParticleRight.Play();
            else
                slideParticleLeft.Play();
        }
        else
        {
            slideParticleLeft.Stop();
            slideParticleRight.Stop();
        }
    }
    void checkGround()
    {
        if(coll.onGround && !onGround)
        {
            onGround = true;
            hasDashed = false;
            hasTiced = false;
            canlanternJump = true;
            jumpParticle.Play();
        }
        if(!coll.onGround && onGround)
            onGround = false;
    }
    void Slide()
    {   
        Debug.Log("Sliding");
        isSliding = true;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x,-1*slideSpeed);
    }

    void Tic()
    {
        Debug.Log("Tic");
        if(faceDir == coll.wallSide)
        {
            faceDir = -faceDir;
            visual.transform.localScale = new Vector3(faceDir, 1, 1);
        }
        StartCoroutine(TicProcess(0));
        StartCoroutine(TicProcess(.1f));
        StartCoroutine(disableBetterJump(.3f));
        rb.velocity = new Vector2(-coll.wallSide*TicForce,TicForce);
        hasTiced = true;

    }

    void Dash(Vector2 dir)
    {
        Debug.Log("Dashing");
        rb.velocity = Vector2.zero;
        if(!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            dir = new Vector2(faceDir, dir.y);

        hasDashed = true;
        rb.velocity += dashSpeed*dir.normalized;    
        StartCoroutine(DashProcess());
    }

    void DashParallel(Vector2 dir)
    {
        Debug.Log("Dashing");
        rb.velocity = Vector2.zero;
        dir = new Vector2(faceDir, 0f);

        hasDashed = true;
        rb.velocity += dashSpeed*dir.normalized;    
        StartCoroutine(DashProcess());
    }

    IEnumerator DashProcess()
    {
        StartCoroutine(DashFlush());
        isDashing = true;
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);
        if(!Input.GetKey(KeyCode.DownArrow))
            GetComponent<BetterJumping>().enabled = false;
        dashParticle.Play();
        yield return new WaitForSeconds(.26f);
        dashParticle.Stop();
        GetComponent<BetterJumping>().enabled = true;
        isDashing = false;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    IEnumerator TicProcess(float value)
    {
        isTicing = true;
        yield return new WaitForSeconds(value);
        isTicing = false;
    }


    IEnumerator DashFlush()
    {
        yield return new WaitForSeconds(.2f);
        if(coll.onGround)
            hasDashed = false;
    }



    void Injured(Vector2 contact)
    {
        if(!isInvinc)
        {
            Debug.Log("Hurt");
        
            anim.SetTrigger("Hurt");
             
            // rb.velocity = contact*injuredSpeed;
            transform.Translate(contact * injuredSpeed * Time.deltaTime);
            Health -= 1.0;
            isInvinc = true;
            StartCoroutine(DisableOthers());
            Invoke("Recover",invincTime);
        }
        
        if(Health <= 0.0)
        {
            StartCoroutine(Death());
            Invoke("Resurrection", 2f);
            Health = 5.0f;
        }
    }

    IEnumerator Death()
    {
        anim.SetTrigger("Death");
        isHurting = true;
        yield return new WaitForSeconds(.35f);
        this.gameObject.SetActive(false);
    }
    void Resurrection()
    {
        //Health = 5.0;
        this.gameObject.SetActive(true);
        this.gameObject.transform.position = lastPos;
    }

    IEnumerator DisableOthers()
    {
        isHurting = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(.3f);
        isHurting = false;
    }
    void Recover()
    {
        isInvinc = false;
    }


    private void OnCollisionEnter2D(Collision2D other) {
        float xdir = other.contacts[0].normal.x;
        float ydir = other.contacts[0].normal.y;
        if(other.gameObject.tag == "Wolf")
        {
            if(ydir>=0 && this.isActiveAndEnabled)
            {
                StartCoroutine(disableBetterJump(.3f));
                rb.velocity = new Vector2(rb.velocity.x, jumpForce*ydir);
            }
        }

    }


    IEnumerator disableBetterJump(float distime)
    {
        betterJump.enabled = false;
        yield return new WaitForSeconds(distime);
        betterJump.enabled = true;
    }
    
}
