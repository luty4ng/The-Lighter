using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public LayerMask targetLayer;

    [Space]
    public Vector2 detectBoxSize;
    public Vector2 detectBoxOffset;
    public float detectRadius;
    public float attackDistance;
    public float moveSpeed;
    public float chaseSpeed;
    public float accerateRate;

    public bool beDetected;
    public bool detected;


    [Space]
    public float waitTime;
    protected GameObject target;
    
    public Vector2 getDirection(Vector2 goal)
    {
        Vector2 direction;
        Vector2 obj = this.transform.position; 
        direction = goal - obj;
        return direction;
    }

    public float getDistance(Vector2 direction)
    {
        return Mathf.Sqrt(Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2));
    }


    public virtual void attack()
    {

    }

    public virtual void detect()
    {

    }
    public void move(Vector2 dir, float Speed)
    {

        transform.Translate(dir.normalized * Speed * accerateRate * Time.deltaTime);
        if(dir.normalized.x<0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    


}
