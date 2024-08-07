using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mineScript : MonoBehaviour
{
    public mainGameScript mainScript;
    public Animator animator;
    public void buttonPress()
    {
        mainScript.click();
        animator.SetTrigger("Pressed");
    }
}
