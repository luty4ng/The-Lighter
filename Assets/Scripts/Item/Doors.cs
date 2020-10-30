using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    // Start is called before the first frame update
    private TempLights[] fires;
    private BoxCollider2D coll;
    private bool allLightsUp;
    public Animator animator;
    public string corrFires;
    private GameObject startPoint;
    void Start()
    {
        GameObject corrFire = GameObject.Find(corrFires);
        fires = corrFire.transform.GetComponentsInChildren<TempLights>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        startPoint = GameObject.Find("StartPoint");
        
    }

    // Update is called once per frame
    void Update()
    {
        allLightsUp = true;
        foreach (TempLights light in fires)
        {
            //TempLights light = i.GetComponent<TempLights>();
            if(!light.lightsUp)
            {
                allLightsUp = false;
                break;
            }
        }

        if(allLightsUp)
        {
            Debug.Log("Door Open");
            animator.SetTrigger("Open");
            EventCenter.GetInstance().EventTrigger(corrFires + " AllLightsUp");
            coll.isTrigger = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name == "Player")
        {
            startPoint.transform.position = this.transform.position;
        }
    }
}
