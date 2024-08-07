using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace gameplayAlgorithm
{
    internal class Program
    {
        enum scale{ 
            k,m,b,t,q,Q,s,o
        }
        static List<multiplier> multipliers;
        static double[,] genPricing;
        static StringBuilder timeS;
        static StringBuilder moneyS;
        static StringBuilder prestigeS;
        static StringBuilder[] generatorsS;
        static StringBuilder[] genPricesS;
        static StringBuilder[] multipliersS;
        static StringBuilder[] earningS;
        static StringBuilder happinessS;
        static StringBuilder[] extraS;
        static double happiness;
        static readonly float boredom = 0.15f;
        static readonly float prestigeMult = 0.1f;
        static double money;
        static double totalEarned;
        static double prestige;
        static List<double>[] income;
        static void Main(string[] args)
        {
            timeS = new StringBuilder();
            moneyS = new StringBuilder();
            prestigeS = new StringBuilder();
            generatorsS = new StringBuilder[5];
            genPricesS = new StringBuilder[5];
            multipliersS = new StringBuilder[5];
            earningS = new StringBuilder[5];
            happinessS = new StringBuilder();
            extraS = new StringBuilder[2];

            for (int i = 0; i < generatorsS.Length; i++)
            { generatorsS[i] = new StringBuilder(); }
            for (int i = 0; i < genPricesS.Length; i++)
            { genPricesS[i] = new StringBuilder(); }
            for (int i = 0; i < multipliersS.Length; i++)
            { multipliersS[i] = new StringBuilder(); }
            for (int i = 0; i < extraS.Length; i++)
            { extraS[i] = new StringBuilder(); }
            for (int i = 0; i < earningS.Length; i++)
            { earningS[i] = new StringBuilder(); }

            timeS.Append("Time (s):;");
            moneyS.Append("Player gold:;");
            prestigeS.Append("Prestige:;");
            for (int i = 0; i < generatorsS.Length; i++)
            { generatorsS[i].Append("Gen " + (i + 1) + " count:;"); }
            for (int i = 0; i < genPricesS.Length; i++)
            { genPricesS[i].Append("Gen " + (i + 1) + " price:;"); }
            for (int i = 0; i < multipliersS.Length; i++)
            {multipliersS[i].Append("Multiplier " + (i + 1) + ":;"); }
            for (int i = 0; i < earningS.Length; i++)
            { earningS[i].Append("Gen " + (i + 1) + "earnings:;"); }
            happinessS.Append("Happiness:;");
            extraS[0].Append("Boredness line:;");
            extraS[0].Append("Prestige multiplier:");

            //setting initial values
            float patience = 0.85f;
            prestige = 0;
            genPricing = new double[5,2];
            resetValues();

            //writing intial values
            extraS[1].Append(boredom + ";");
            extraS[1].Append(prestigeMult + ";");

            int buyDelay = 3;
            for (int d = 0; d < 2; d++)//2 days of playtime
            {
                for (int t = 1; t < 2880; t++)//4 hours of a day*10s
                {
                    //quitting
                    /*if (happiness < boredom) { break; }
                    else */if (happiness > 5) { happiness = 5; }
                    //time
                    timeS.Append(t + "0;");
                    //boredom tick
                    happiness *= patience;
                    happinessS.Append(happiness + ";");
                    //calculate income
                    double before = money;
                    for (int j = 0; j < income.Length; j++)
                    {
                        double burst = income[j][0];
                        for (int i = 1; i < income[j].Count; i++)
                        {
                            burst *= income[j][i]*((prestige*prestigeMult)+1);
                        }
                        money += burst;
                        totalEarned += burst;
                        if (j == 0) { continue; }
                        earningS[j - 1].Append(burst + ";");
                    }
                    double change = money - before;
                    if (t == 1440 && d == 0) { 
                        Console.WriteLine(change);
                        //double debugIncome = change * 30/1000;
                        double debugIncome = totalEarned;
                        int scaleIndex = 0;
                        while (debugIncome > 1000)
                        {
                            debugIncome = Math.Round(debugIncome/1000);
                            scaleIndex++;
                        }
                        Console.WriteLine(debugIncome+Enum.GetName(typeof(scale),scaleIndex));
                        Console.ReadLine();
                    }
                    ///buy stuff
                    if (t == 60)
                    {
                        buyDelay = 30; //buy every 5 minutes
                        patience = 0.96f;
                    }
                    else if (t == 360)
                    {
                        buyDelay = 180;//30mins
                        patience = 0.996f;
                    }
                    else if (t == 1 && d == 1)
                    {
                        buyDelay = 3;//30s
                        patience = 0.95f;
                    }
                    if (t % buyDelay == 0)
                    {
                        //buy multipliers
                        if (multipliers.Count > 0 && multipliers[0].price < money)
                        {
                            money -= multipliers[0].price;
                            income[multipliers[0].generator + 1].Add(multipliers[0].boost);
                            happiness += 2;
                            multipliers.RemoveAt(0);
                        }
                        //buy generators
                        while (true)//buy as many as possible
                        {
                            //find the most effective generator
                            int selected = -1;
                            double effectiveness = 0;
                            for (int j = 0; j < 5; j++)
                            {
                                if (genPricing[j, 0] > money)
                                {
                                    continue;
                                }
                                double newEf = income[j + 1][0] / genPricing[j, 0];
                                for (int k = 2; k < income[j + 1].Count; k++)
                                {
                                    newEf *= income[j + 1][k];
                                }
                                if (newEf > effectiveness)
                                {
                                    effectiveness = newEf;
                                    selected = j;
                                }
                            }
                            //buy the most effective generator
                            if (selected == -1)
                            {
                                break;
                            }
                            else
                            {
                                money -= genPricing[selected, 0];
                                genPricing[selected, 0] *= genPricing[selected, 1];
                                income[selected + 1][1]++;
                                //happiness calculation
                                double genPower = 0;
                                if (change == income[0][0]) { genPower = 1; }
                                else
                                {
                                    genPower = effectiveness * genPricing[selected, 0] / (change - income[0][0]);
                                    if (genPower > 1) { genPower = 1; }
                                }
                                //double freshness = income[selected + 1][1]; //or lack there of
                                //if (freshness > 30) { freshness = 30; }
                                happiness += (1 + genPower) /*/ freshness*/;
                            }
                        }
                    }
                    //write generators
                    for (int i = 0; i < generatorsS.Length; i++)
                    { generatorsS[i].Append(income[i + 1][1] + ";"); }
                    //write multipliers
                    for (int i = 0; i < multipliersS.Length; i++)
                    {
                        double amount = 1;
                        for (int j = 2; j < income[i + 1].Count; j++)
                        {
                            amount *= Math.Round(income[i + 1][j]);
                        }
                        multipliersS[i].Append(amount + ";");
                    }
                    //write money
                    money = Math.Round(money);
                    moneyS.Append(money + ";");

                    //write prestige
                    prestigeS.Append(prestige+";");

                    for (int i = 0; i < genPricesS.Length; i++)
                    { genPricesS[i].Append(genPricing[i, 0] + ";"); }
                }//end of day
                if (d<1)
                {
                    //add prestige
                    prestige = Math.Log(totalEarned,2);

                    //reset values
                    resetValues();
                }

            }
            StringBuilder export = new StringBuilder();
            export.AppendLine(timeS.ToString());
            export.AppendLine(moneyS.ToString());
            export.AppendLine(prestigeS.ToString());
            foreach (StringBuilder generator in generatorsS)
            {
                export.AppendLine(generator.ToString());
            }
            foreach (StringBuilder generator in genPricesS)
            {
                export.AppendLine(generator.ToString());
            }
            foreach (StringBuilder multiplier in multipliersS)
            {
                export.AppendLine(multiplier.ToString());
            }
            foreach (StringBuilder earning in earningS)
            {
                export.AppendLine(earning.ToString());
            }
            export.AppendLine(happinessS.ToString());
            foreach (StringBuilder extra in extraS)
            {
                export.AppendLine(extra.ToString());
            }

            File.WriteAllText("E:\\UHKREMOTE\\datasheet.csv",export.ToString());
        }
        class multiplier
        {
            public double price; public double boost; public int generator;
            public multiplier(double price, double boost, int generator) {
                this.price = price;
                this.boost = boost; 
                this.generator = generator;
            }
        }
        static void resetValues()
        {
            ///multipliers
            multipliers = new List<multiplier>();

            multipliers.Add(new multiplier(350000, 25, 0));//x25 boost for 350k for gen 0
            multipliers.Add(new multiplier(600000, 20, 1));
            multipliers.Add(new multiplier(1500000, 15, 2));
            multipliers.Add(new multiplier(3500000, 10, 3));
            multipliers.Add(new multiplier(8000000, 7, 4));
            ///////////////////////////////mmtttuuu
            multipliers.Add(new multiplier(19000000, 50, 0));
            multipliers.Add(new multiplier(43000000, 35, 1));
            multipliers.Add(new multiplier(90000000, 30, 2));
            multipliers.Add(new multiplier(180000000, 20, 3));
            multipliers.Add(new multiplier(320000000, 15, 4));
            ///////////////////////////////mmmtttuuu
            multipliers.Add(new multiplier(620000000d, 17, 0));
            multipliers.Add(new multiplier(1100000000d, 13, 1));
            multipliers.Add(new multiplier(1800000000d, 10, 2));
            multipliers.Add(new multiplier(30000000000d, 10, 3));
            multipliers.Add(new multiplier(100000000000d, 10, 4));
            ///////////////////////////////bbbmmmtttuuu
            multipliers.Add(new multiplier(450000000000d, 7, 0));
            multipliers.Add(new multiplier(900000000000d, 10, 1));
            multipliers.Add(new multiplier(2000000000000d, 10, 2));
            multipliers.Add(new multiplier(14000000000000d, 10, 3));
            multipliers.Add(new multiplier(26000000000000d, 10, 4));
            ///////////////////////////////qtttbbbmmmtttuuu
            multipliers.Add(new multiplier(200000000000000d, 10, 0));
            multipliers.Add(new multiplier(600000000000000d, 10, 1));
            multipliers.Add(new multiplier(160000000000000d, 10, 2));
            multipliers.Add(new multiplier(800000000000000d, 10, 3));
            multipliers.Add(new multiplier(2500000000000000d, 10, 4));
            ///generators
            //base price
            genPricing[0, 0] = 20;//100 clicks to get
            genPricing[1, 0] = 200;//5 mins to get
            genPricing[2, 0] = 800;//10
            genPricing[3, 0] = 5500;//15
            genPricing[4, 0] = 80000;//30
            //price increase
            genPricing[0, 1] = 1.02;
            genPricing[1, 1] = 1.025;
            genPricing[2, 1] = 1.03;
            genPricing[3, 1] = 1.035;
            genPricing[4, 1] = 1.04;

            //setting initial values
            happiness = 0.5f;
            money = 0;
            totalEarned = 0;

            //writing intial values
            timeS.Append("0;");
            moneyS.Append("0;");
            prestigeS.Append("0;");
            for (int i = 0; i < generatorsS.Length; i++)
            { generatorsS[i].Append("0;"); }
            for (int i = 0; i < genPricesS.Length; i++)
            { genPricesS[i].Append(genPricing[i, 0] + ";"); }
            for (int i = 0; i < earningS.Length; i++)
            { earningS[i].Append("0;"); }
            for (int i = 0; i < multipliersS.Length; i++)
            { multipliersS[i].Append("1;"); }
            happinessS.Append(happiness + ";");

            
            income = new List<double>[6];
            for (int i = 0; i < income.Length; i++)
            { income[i] = new List<double>(); }
            income[0].Add(25);//0.5*5clicks*10s
            income[1].Add(1);//(how much is produced every 10 seconds)
            income[2].Add(5);
            income[3].Add(20);
            income[4].Add(100);
            income[5].Add(500);
            for (int i = 1; i < income.Length; i++)
            { income[i].Add(0); }//amount of generators

        }
    }
}
