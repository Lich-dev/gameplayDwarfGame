using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buyAmountScript : MonoBehaviour
{
    public int buyAmount;
    public mainGameScript mainScript;
    public void setBuyAmount()
    {
        mainScript.setBuyAmount(buyAmount);
    }
}
