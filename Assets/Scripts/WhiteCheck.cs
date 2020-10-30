using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class WhiteCheck : MonoBehaviour
{
    public Light2D light;
    public CircleCollider2D coll;
    void Start()
    {
        light = GetComponent<Light2D>();
        coll = GetComponent<CircleCollider2D>();
        coll.isTrigger = true;
        coll.radius = light.pointLightOuterRadius;
    }


}
