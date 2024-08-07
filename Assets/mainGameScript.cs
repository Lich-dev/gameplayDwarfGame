using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainGameScript : MonoBehaviour
{
    public counterScript counter;
    public Text prestigeLabel;
    public Text prestigeToEarnLabel;
    public genScript[] gen;
    public upgradeScript[] mults;
    private double[] genMultipliers = new double[5];
    private List<int> boughtMultipliers = new List<int>();
    private double gold = 0;
    private double totalEarned = 0;
    private double prestige = 0;
    private int buyAmount = 1;

    private void Start()
    {
        //initialize multipliers
        for (int i = 0; i < genMultipliers.Length; i++)
        {
            genMultipliers[i] = 1;
        }
        for (int i = 0; i < mults.Length; i++)
        {
            mults[i].setIndex(i);
        }
        loadGame();
    }
    private void Update()
    {
        //calculate income
        earnGold(calculateIncome());
        //update counter
        counter.updateCounter(gold);
        //Debug.Log(genMultipliers[1]);
        if (totalEarned < 150000000000d) { 
            prestigeToEarnLabel.text = "Amount left: " + staticScript.getScaledNumber(150000000000d - totalEarned); 
        }
        else
        {
            prestigeToEarnLabel.text = "Prestige to earn: " + staticScript.getScaledNumber(Math.Log(totalEarned, 2));
        }
    }
    public void prestigeButton()
    {
        if(totalEarned < 150000000000d) { return; }
        prestige += Math.Log(totalEarned, 2);
        prestigeLabel.text = staticScript.getScaledNumber(prestige);
        gold = 0;
        totalEarned = 0;
        for (int i = 0; i < gen.Length; i++)
        {
            gen[i].prestigeReset();
        }
        saveGame();
        SceneManager.LoadScene(0);
    }
    public void saveGame() {
        StringBuilder saveData = new StringBuilder();
        //save simple values
        saveData.Append(gold.ToString() + ";");
        saveData.Append(totalEarned.ToString() + ";");
        saveData.Append(prestige.ToString() + ";");
        saveData.Append(buyAmount.ToString() + ";");
        //save multipliers
        saveData.Append(boughtMultipliers.Count.ToString() + ";");
        foreach (double multiplierIndex in boughtMultipliers)
        {
            saveData.Append(multiplierIndex.ToString() + ";");
        }
        //save gens
        foreach (genScript generator in gen)
        {
            saveData.Append(generator.getAmount().ToString() + ";");
        }
        //save time
        saveData.Append(DateTime.Now.ToString());
        //write file
        File.WriteAllText(Application.persistentDataPath + @"/save.xml", saveData.ToString());
    }
    public void loadGame() {
        if (File.Exists(Application.persistentDataPath + @"/save.xml"))
        {
            try
            {
                int offset = 0;
                //load file
                //Debug.Log(File.ReadAllText(Application.persistentDataPath + @"/save.xml"));
                string[] loadedData = File.ReadAllText(Application.persistentDataPath + @"/save.xml").Split(';');
                //load simple values
                gold = double.Parse(loadedData[0]);
                totalEarned = double.Parse(loadedData[1]);
                prestige = double.Parse(loadedData[2]);
                prestigeLabel.text = staticScript.getScaledNumber(prestige);
                buyAmount = int.Parse(loadedData[3]);
                //load multipliers
                int multListLength = int.Parse(loadedData[4]);
                offset = 5;
                for (int i = 0; i < multListLength; i++)//could be foreach
                {
                    int index = int.Parse(loadedData[offset]);
                    mults[index].preOrder();
                    offset++;
                }
                //load gens
                for (int i = 0; i < gen.Length; i++)
                {
                    gen[i].setAmount(int.Parse(loadedData[offset]));
                    offset++;
                }
                //load time
                TimeSpan difference = DateTime.Now - DateTime.Parse(loadedData[loadedData.Length - 1]);
                earnGold(calculateIncome((float)difference.TotalSeconds));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public void wipeSave()
    {
        File.Delete(Application.persistentDataPath + @"/save.xml");
        SceneManager.LoadScene(0);
    }
    public void click()
    {
        earnGold(1);
    }
    public void earnGold(double amount)
    {
        gold += amount;
        totalEarned += amount;
    }
    public void buyMultiplier(int genIndex, double multiplier,int upgradeIndex)
    {
        genMultipliers[genIndex] = genMultipliers[genIndex] * multiplier;
        boughtMultipliers.Add(upgradeIndex);
    }
    private double calculateIncome()
    {
        return calculateIncome(Time.deltaTime);
    }
    private double calculateIncome(float time)
    {
        double income = 0;
        for (int i = 0; i < gen.Length; i++)
        {
            double multiplier = (1 + prestige * 0.1d) * genMultipliers[i];
            income += gen[i].getIncome(multiplier);
        }
        return income * time;
    }
    public double getGold(){return gold;}
    public int getBuyAmount(){return buyAmount;}
    public void spendGold(double amount) { gold -= amount; }
    public void setBuyAmount(int amount)
    {
        buyAmount = amount;
    }
    private void OnApplicationQuit()
    {
        saveGame();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            saveGame();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            saveGame();
        }
    }
}
