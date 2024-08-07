using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class counterScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    string[] scale =
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
    public void updateCounter(double gold)
    {
        text.text = "<sprite=0> " + staticScript.getScaledNumber(gold);
    }
}
