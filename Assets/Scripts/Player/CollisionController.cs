using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask groundLayer;
    public LayerMask itemLayer;

    public int wallSide;
    
    [Header("Boolean")]
    public bool onGround;
    public bool onRightWall;
    public bool onLeftWall;
    public bool onWall;
    public bool onRebound;
    

    [Header("Gizmos")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private Vector2 wallBoxSize;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onRightWall = Physics2D.OverlapBox((Vector2)this.transform.position + rightOffset, wallBoxSize, 0, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)this.transform.position + leftOffset, wallBoxSize, 0, groundLayer);
        onWall = onRightWall||onLeftWall ? true : false;
        onGround = Physics2D.OverlapBox((Vector2)this.transform.position + bottomOffset, boxSize, 0, groundLayer) || Physics2D.OverlapBox((Vector2)this.transform.position + bottomOffset, boxSize, 0, itemLayer);
        wallSide = onRightWall ? 1 : -1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };
        Gizmos.DrawWireCube((Vector2)this.transform.position + bottomOffset, boxSize);
        Gizmos.DrawWireCube((Vector2)this.transform.position + leftOffset, wallBoxSize);
        Gizmos.DrawWireCube((Vector2)this.transform.position + rightOffset, wallBoxSize);

    }
}
