using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationTrigger : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    public void triggerEvent(string eventName) {
        animator.SetTrigger(eventName);
    }
    private void setBoolTrue(string boolName)
    {
        //Debug.Log("started " + boolName);
        animator.SetBool(boolName, true);
    }
    private void setBoolFalse(string boolName)
    {
        //Debug.Log("stopped "+boolName);
        animator.SetBool(boolName, false);
    }
}
