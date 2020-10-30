using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed;
    public float accerateRate;
    public bool onRightWall;
    public bool onLeftWall;
    public bool wallDect;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 wallBoxSize;
    [SerializeField] private Vector2 leftOffset;
    [SerializeField] private Vector2 rightOffset;
    void Start()
    {
        EventCenter.GetInstance().AddEventListener<Vector2>(this.gameObject.name + " Move", Move);
        wallDect = true;
    }

    private void Update() {
        onRightWall = Physics2D.OverlapBox((Vector2)this.transform.position + rightOffset, wallBoxSize, 0, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)this.transform.position + leftOffset, wallBoxSize, 0, groundLayer);
    }
    void Move(Vector2 dir)
    {
        if(!onLeftWall && !onRightWall)
            transform.Translate(dir.normalized * Speed * accerateRate * Time.deltaTime);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)this.transform.position + leftOffset, wallBoxSize);
        Gizmos.DrawWireCube((Vector2)this.transform.position + rightOffset, wallBoxSize);

    }
}
