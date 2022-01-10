using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RBSDatabase : MonoBehaviour
{

    private LookForObjects lookForResources;
    private RBSAnt ant;
    private RBSAntMovement antMovement;
    private Dictionary<string, bool> triggeredRules = new Dictionary<string, bool>();
    private Arbiter arbiter;
    private ExecuteRules executeActions;
    private string ruleToFire = "";
    [SerializeField] private Queen queen;
   
    
    //Datums
    GameObject nearbyWaterSource;
    GameObject nearbyFoodSource = null;
    GameObject nearbyEnemy;
    GameObject targetFood = null;
    float queenEnergy;
    float antEnergy;
    float antHydration;
    bool holdingResource = false;
    bool acquireFoodForQueen = false;
    bool acquireWater = false;
    bool atWater;
    bool dead;
    bool movingToFood = false;

    void Start()
    {
        lookForResources = GetComponent<LookForObjects>();
        ant = GetComponent<RBSAnt>();
        antMovement = GetComponent<RBSAntMovement>();
        InitialiseRules();
        arbiter = new Arbiter();
        arbiter.InitialisePriorities();
        executeActions = new ExecuteRules(antMovement, this, ant);
    }

    void FixedUpdate()
    {
        lookForResources.CheckSurroundingArea();
        UpdateDatabaseEntries();
        AssessRules();
        ruleToFire = arbiter.CrossReferenceTriggeredRules(triggeredRules);
        if (ruleToFire != "")
        {
            FireRule(ruleToFire);
        }
     
    }
    private void FireRule(string rule)
    {
        Type type = typeof(ExecuteRules);
        MethodInfo method = type.GetMethod(rule);
        method.Invoke(executeActions, null);
    }
    private void InitialiseRules()
    {
        triggeredRules.Add("Die", false);
        triggeredRules.Add("DrinkWater", false);
        triggeredRules.Add("EatFood", false);
        triggeredRules.Add("Flee", false);
        triggeredRules.Add("FoodForAnt", false);
        triggeredRules.Add("FoodForQueen", false);
        triggeredRules.Add("RandomMovement", true);
        triggeredRules.Add("ReturnToQueen", false);
        triggeredRules.Add("WaterForAnt", false);
    }
    private void UpdateDatabaseEntries()
    {
        queenEnergy = queen.GetEnergy();
        antEnergy = ant.GetEnergy();
        atWater = ant.GetAtWater();
        antHydration = ant.GetHydration();
        holdingResource = ant.GetHoldingResource();
        nearbyFoodSource = lookForResources.AssessFoodPosition();
       
        nearbyWaterSource = lookForResources.AssessWaterPosition();
        executeActions.SetWaterResource(nearbyWaterSource);
        nearbyEnemy = lookForResources.AssessEnemyPositions();
        dead = ant.GetDead();
        if (nearbyFoodSource != null&&!movingToFood)
        {
            executeActions.SetFoodResource(nearbyFoodSource);
            targetFood = nearbyFoodSource;
        }
    }
    private void AssessRules()
    {
    
        Debug.Log(nearbyFoodSource);
        AssessRule(() => triggeredRules["Die"] = true, () => triggeredRules["Die"] = false, () =>
        (dead));
        AssessRule(() => triggeredRules["DrinkWater"] = true, () => triggeredRules["DrinkWater"] = false, () =>
        (atWater && acquireWater));
        AssessRule(() => triggeredRules["EatFood"] = true, () => triggeredRules["EatFood"] = false, () =>
        (holdingResource && !acquireFoodForQueen));
        AssessRule(() => triggeredRules["Flee"] = true, () => triggeredRules["Flee"] = false, () =>
        (nearbyEnemy != null));
        AssessRule(() => triggeredRules["FoodForAnt"] = true, () => triggeredRules["FoodForAnt"] = false, () =>
        (antEnergy < queenEnergy && targetFood != null && !holdingResource));
        AssessRule(() => triggeredRules["FoodForQueen"] = true, () => triggeredRules["FoodForQueen"] = false, () =>
        (queenEnergy < antEnergy && targetFood != null && !holdingResource ));
        AssessRule(() => triggeredRules["ReturnToQueen"] = true, () => triggeredRules["ReturnToQueen"] = false, () =>
        (holdingResource && acquireFoodForQueen));  
        AssessRule(() => triggeredRules["WaterForAnt"] = true, () => triggeredRules["WaterForAnt"] = false, () =>
        (antHydration < 70 && nearbyWaterSource != null && !holdingResource));
     
    }
    public void AssessRule(Action actionTrue, Action actionFalse, Func<Boolean> condition)
    {
        if (condition()) actionTrue();
        else actionFalse();
    }

    public class ExecuteRules
    {
        private readonly RBSAnt ant;
        private readonly RBSAntMovement antMovement;
        private GameObject foodResource;
        private GameObject waterResource;
        private readonly RBSDatabase RBSDatabase;

        public ExecuteRules(RBSAntMovement _antMovement, RBSDatabase _RBSDatbase, RBSAnt _ant)
        {
            antMovement = _antMovement;
            RBSDatabase = _RBSDatbase;
            ant = _ant;
        }
        public void Die()
        {
            ant.Die();
            if(antMovement.animator.speed>0.01f) antMovement.animator.speed -= 0.02f;
        }
        public void DrinkWater()
        {
            ant.Drink();
            antMovement.animator.speed = 0f;
        }
        public void EatFood()
        {
            ant.Eat();
        }
        public void Flee()
        {
            antMovement.Flee();
            RBSDatabase.targetFood = null;
            RBSDatabase.nearbyFoodSource = null;
        }
        public void FoodForAnt()
        {
            antMovement.SetResourceToMoveTowards(foodResource);
            foodResource.GetComponent<Food>().SetSoughtAfter(true);
            antMovement.MoveTowardsResource();
            RBSDatabase.acquireFoodForQueen = false;
            RBSDatabase.movingToFood = true;
        }
        public void FoodForQueen()
        {
            antMovement.SetResourceToMoveTowards(foodResource);
            foodResource.GetComponent<Food>().SetSoughtAfter(true);
            antMovement.MoveTowardsResource();
            RBSDatabase.acquireFoodForQueen = true;
            RBSDatabase.movingToFood = true;
        }
        public void RandomMovement()
        {
            antMovement.Movement(true);
            antMovement.SetReturningToQueen(false);
            RBSDatabase.movingToFood = false;
        }
        public void ReturnToQueen()
        {
            ant.HoldObject();
            antMovement.SetReturningToQueen(true);
            antMovement.ReturnToQueen();
            RBSDatabase.movingToFood = false;
        }
        public void WaterForAnt()
        {
            antMovement.SetResourceToMoveTowards(waterResource);
            antMovement.MoveTowardsResource();
            RBSDatabase.acquireWater = true;
            RBSDatabase.movingToFood = false;
        }
        public void SetFoodResource(GameObject _foodResource)
        {
            foodResource = _foodResource;
        }
        public void SetWaterResource(GameObject _waterResource)
        {
            waterResource = _waterResource;
        }
    }
}
