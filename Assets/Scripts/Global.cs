using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Global : MonoBehaviour
{
    public VolumeComponent vignette;
    public Volume volume;
    public Lantern lantern;
    
    GameObject[] points;
    GameObject roaming;
    public float maxIndensity = 1f;
    public float minIndensity = 0.5f;
    public float extinctTime;
    public float restoreTime;
    public float splitExtendTime = 2.5f;
    public float maxExtinctTime;
    public bool activateVig;
    public float numOfActiveFire;

    [SerializeField] private float tempIndensity;
    void Start()
    {
        //vignette.intensity = new FloatParameter { value = 1.0f };
        AudioManager.GetInstance().PlayBGM("DarkSecret");
        
        tempIndensity = minIndensity;
        volume = GetComponentInChildren<Volume>();
        vignette = volume.profile.components[1];
        SetVignetteIndensity(tempIndensity);

        roaming = GameObject.Find("RoamingPoints");
        points = GameObject.FindGameObjectsWithTag("RoamingPoint");
        foreach (GameObject i in points)
            i.transform.SetParent(roaming.transform);

        lantern = GameObject.Find("lantern").GetComponent<Lantern>();
        EventCenter.GetInstance().AddEventListener("ExtendTime", ExtendTime);
        EventCenter.GetInstance().AddEventListener("LostGhostEffect", LostGhostEffect);
    }

    private void Update() {
        numOfActiveFire = 0;
        GameObject[] allFires = GameObject.FindGameObjectsWithTag("Fires");
        foreach (GameObject item in allFires)
        {
            TempLights tmp = item.GetComponent<TempLights>();
            if(tmp.lightsUp)
                numOfActiveFire +=1;
        }
        UpdateIntensity();
    }

    void SetVignetteIndensity(float Indensity)
    {
        vignette.parameters[2].SetValue(new FloatParameter(Indensity));
    }

    
    void UpdateIntensity()
    {
        float reducePerSec = (maxIndensity - minIndensity)/extinctTime;
        float restorePerSec = (maxIndensity - minIndensity)/restoreTime;
        float changePerSec = (maxIndensity - minIndensity)/extinctTime;
        if(lantern.inWhiteLight)
            changePerSec = 0f;
        if(lantern.inYellowLight)
            changePerSec = -restorePerSec;
        
        tempIndensity += changePerSec*Time.deltaTime;

        if(tempIndensity > maxIndensity)
            tempIndensity = maxIndensity;
        else if(tempIndensity < minIndensity)
            tempIndensity = minIndensity;
        else
            SetVignetteIndensity(tempIndensity);
    }

    void ExtendTime()
    {
        Debug.Log("SplitEffect");
        extinctTime += splitExtendTime;
        restoreTime += splitExtendTime/5f;
    }

    void LostGhostEffect()
    {
        maxIndensity *= 0.9f;
        minIndensity *= 0.9f;
    }
}
