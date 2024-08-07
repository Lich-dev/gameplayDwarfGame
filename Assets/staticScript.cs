using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staticScript : MonoBehaviour
{
    public static int buyAmount = 1;
    public static string[] scale =
    {
        "",
        "k",
        "m",
        "b",
        "t",
        "q",
        "Q",
        "s",
        "S",
        "o",
        "n",
        "d",
        "u",
        "D",
        "T"
    };
    public static string getScaledNumber(double amount)
    {
        if (amount < 1000)
        {
            amount = Math.Round(amount, 1);
            return amount.ToString();
        }
        else
        {
            int scaleIndex = (int)Math.Floor(Math.Log(amount, 1000));
            amount = Math.Round(amount / Math.Pow(1000, scaleIndex), 1);
            return amount.ToString() + scale[scaleIndex];
        }
    }
}
