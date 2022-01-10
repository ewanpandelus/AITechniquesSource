using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arbiter 
{
    private Dictionary<string, (int, bool)> priorityRules = new Dictionary<string, (int, bool)>();


 
    public string CrossReferenceTriggeredRules(Dictionary<string, bool> triggeredRules)
    {
        foreach (var item in triggeredRules)
        {
            if (item.Value == true) priorityRules[item.Key] = (priorityRules[item.Key].Item1, true);
            else priorityRules[item.Key] = (priorityRules[item.Key].Item1, false);
        }
        return Arbitrate();
    }
    private string Arbitrate()
    {
        int min = 10;
        string ruleToFire = "";
        foreach (var item in priorityRules)
        {
            if (item.Value.Item2 == true &&  item.Value.Item1 < min)
            {
                min = item.Value.Item1;
                ruleToFire = item.Key;
            }
        }
        return ruleToFire;
    }

    public void InitialisePriorities()
    {
        priorityRules.Add("Die", (-1, false));
        priorityRules.Add("DrinkWater", (1, false));
        priorityRules.Add("EatFood", (1, false));
        priorityRules.Add("Flee", (0, false));
        priorityRules.Add("FoodForQueen", (1, false));
        priorityRules.Add("FoodForAnt", (3, false));
        priorityRules.Add("RandomMovement", (4, false));
        priorityRules.Add("ReturnToQueen", (1,false));
        priorityRules.Add("WaterForAnt", (2, false));   
    }
}
