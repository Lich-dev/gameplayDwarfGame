using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class upgradeScript : MonoBehaviour
{
    public double cost;
    public double multiplier;
    public int genIndex;
    public mainGameScript mainScript;
    public Text[] labels;//price, multiplier
    private int upgradeIndex;
    private void Start()
    {
        labels[0].text = staticScript.getScaledNumber(cost);
        labels[1].text = multiplier.ToString() + "x";
    }
    public void buttonPress()
    {
        if (mainScript.getGold() < cost) { return; }
        mainScript.spendGold(cost);
        mainScript.buyMultiplier(genIndex, multiplier,upgradeIndex);
        gameObject.SetActive(false);
    }
    public void setIndex(int index) { this.upgradeIndex = index; }
    public int getIndex() { return upgradeIndex; }
    public void preOrder() { 
        mainScript.buyMultiplier(genIndex, multiplier, upgradeIndex); 
        gameObject.SetActive(false); 
    }
}