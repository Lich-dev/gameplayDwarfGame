using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class genScript : MonoBehaviour
{
    public double basePrice;
    public double exponent;
    public double baseIncome;
    private int currentAmount = 0;
    public mainGameScript mainGameScript;
    public Text[] labels;//owned amount, cost, amount to buy, gps
    public List<Animator> animators = new List<Animator>();
    public void buyGen()
    {
        double gold = mainGameScript.getGold();
        int buyAmount = calculateAmount(gold);
        double cost = calculateCost(buyAmount);

        if(cost - 0.01 > gold) { return; }
        mainGameScript.spendGold(cost);
        if (animators.Count>0)
        {
            int spawnAmount = buyAmount;
            for (int i = 0; i < animators.Count; i++)
            {
                Debug.Log(i+" "+spawnAmount);
                if (spawnAmount<=0){break;}
                animators[0].SetTrigger("Enter");
                animators.RemoveAt(0);
                spawnAmount--;
            }
        }
        currentAmount += buyAmount;
        labels[0].text = currentAmount.ToString();
        
    }
    public void Update()
    {
        //update amount to buy label
        double gold = mainGameScript.getGold();
        int buyAmount = calculateAmount(gold);
        labels[2].text = buyAmount.ToString();
        //update cost label
        double cost = Math.Round(calculateCost(buyAmount),1);
        labels[1].text = staticScript.getScaledNumber(cost);
    }
    private int calculateAmount(double gold)
    {
        int buyAmount = mainGameScript.getBuyAmount();
        if (buyAmount == -1)
        {
            return (int)Math.Floor(Math.Log((gold * (exponent - 1) / (basePrice * Math.Pow(exponent, currentAmount))) + 1, exponent));
        }
        else { return buyAmount; }
    }
    private double calculateCost(int buyAmount)
    {
        return basePrice * (Math.Pow(exponent, currentAmount) * (Math.Pow(exponent, buyAmount) - 1) / (exponent - 1));
    }
    public double getIncome(double multiplier)
    {
        double income = baseIncome * currentAmount * multiplier;
        labels[3].text = staticScript.getScaledNumber(income) +" GPS";
        return income;
    }
    public void prestigeReset()
    {
        currentAmount = 0;
    }
    public int getAmount() { return currentAmount; }
    public void setAmount(int amount) { 
        currentAmount = amount;
        labels[0].text = currentAmount.ToString();
        if (amount>animators.Count)
        {
            amount = animators.Count;
        }
        for (int i = 0; i < amount; i++)
        {
            animators[0].SetTrigger("Enter");
            animators.RemoveAt(0);
        }
    }
}