using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostGhost : MonoBehaviour
{

    [SerializeField] private Animator anim;
    [SerializeField] private Vector2 roamingTarget;
    void Start()
    {
        anim = GetComponent<Animator>();
        EventCenter.GetInstance().AddEventListener(this.gameObject.name + " Death", Death);
    }

    void Death()
    {
        StartCoroutine(DeathProcess());
    }

    IEnumerator DeathProcess()
    {
        anim.SetTrigger("Death");
        EventCenter.GetInstance().EventTrigger<float>("Add Ghost", 1f);
        EventCenter.GetInstance().EventTrigger("LostGhostEffect");
        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }
}
