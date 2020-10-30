using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Split : MonoBehaviour
{
    // Start is called before the first frame update
    public CircleCollider2D circle;
    public PlayerController player;
    public Animator animator;
    private bool canDestroy;

    void Start()
    {
        circle = GetComponent<CircleCollider2D>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        canDestroy = false;
    }

    private void Update() {
        if(animator.GetBool("Destroy"))
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(info.normalizedTime);
            if(info.normalizedTime >= 1.0f && info.IsName("split_destory"))
                Destroy(this.gameObject); 
        }

    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            animator.SetBool("Destroy",true);
        }
    }
    private void OnDestroy() {
        player.AddSplit(1);
    }

}
