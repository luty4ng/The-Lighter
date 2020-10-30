using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class TempLights : MonoBehaviour
{
    public string targetRoom;
    public float fadeSpeed;
    public float waitTime;
    public bool requireTarget = true;
    public bool canFade;
    private bool allUp;
    [SerializeField] private float maxOuterRadius;
    [SerializeField] private float maxInnerRadius;
    public Light2D fireLight;
    public Light2D coreLight;
    public bool lightsUp;

    
    [SerializeField] private float distance;
    [SerializeField] private PlayerController player;
    private float updateTime;

    
    void Start()
    {
        if(requireTarget)
            targetRoom = transform.parent.gameObject.name;
        coreLight.enabled = false;
        canFade = true;
        allUp = false;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        fireLight = GetComponentInChildren<Light2D>();
        maxOuterRadius = fireLight.pointLightOuterRadius;
        maxInnerRadius = fireLight.pointLightInnerRadius;
        fireLight.pointLightOuterRadius = 0f;
        fireLight.pointLightInnerRadius = 0f;
        EventCenter.GetInstance().AddEventListener(targetRoom + " AllLightsUp", AllUp);       
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if(canFade && Time.time - updateTime > 2.0f)
            FireFade();

        CheckLight();
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Lantern")
        {
            fireLight.pointLightOuterRadius = maxOuterRadius;
            fireLight.pointLightInnerRadius = maxInnerRadius;
            lightsUp = true;
            canFade = false;
        }
    }

    /*
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Lantern")
        {
            StartCoroutine(DisableFade());
        }
    }
    */
    private void FireFade()
    {
        if(fireLight.pointLightOuterRadius > 0.1)
            fireLight.pointLightOuterRadius = fireLight.pointLightOuterRadius * fadeSpeed;
        if(fireLight.pointLightInnerRadius > 0.02)
            fireLight.pointLightInnerRadius = fireLight.pointLightInnerRadius * fadeSpeed;    
        if(fireLight.pointLightInnerRadius <= 0.02 && fireLight.pointLightOuterRadius <= 0.1)
            lightsUp = false;
    }

    IEnumerator AbleFade()
    {
        yield return new WaitForSeconds(waitTime);
        if(!allUp)
            canFade = true;
    }

    void AllUp()
    {
        canFade = false;
        allUp = true;
        coreLight.enabled = true;
        fireLight.pointLightOuterRadius = maxOuterRadius;
        fireLight.pointLightInnerRadius = maxInnerRadius;
        lightsUp = true;
    }

    void CheckLight()
    {
        if(lightsUp)
        {
            Vector2 dir = GameObject.Find("lantern").transform.position - coreLight.gameObject.transform.position;
            distance = Math.Abs(Mathf.Sqrt(dir.x*dir.x + dir.y*dir.y));
            if(distance <= fireLight.pointLightOuterRadius)
            {
                canFade = false;
                fireLight.pointLightOuterRadius = maxOuterRadius;
                fireLight.pointLightInnerRadius = maxInnerRadius;
                updateTime = Time.time;
            }
            else
            {
                canFade = true;
            }
        }
    }
}
