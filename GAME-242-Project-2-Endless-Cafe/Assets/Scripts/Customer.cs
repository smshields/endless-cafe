using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    //TODO: Store Manager 
    public StoreManager store;

    //tracks current goal and state of customer
    public customerState state = customerState.entering;
    public customerGoal goal = customerGoal.leaveCafe;

    //Variables indicating likelihood of initial goal assigmnment - hard coding probability ranges for now
    public float initialDrinkGoalChance = 1f; //70%
    public float initialSitGoalChance = 0.3f; //25%
    public float initialRestroomChance = 0.05f; //04%
    public float initialLeaveChance = 0.01f; //01%

    //probability that an order is online or not
    public float onlineChance = 0.05f; //50%

    //Tracks if the customer's drink order was online or not.
    public bool onlineOrder = false;

    //references to customer's goal at any given time
    public enum customerGoal { 
        getDrink,
        sitDown,
        useRestroom,
        leaveCafe
    }

    //current status of customer
    public enum customerState { 
        waitForRestroom, //line outside of restroom
        waitForOrder, //line to order
        waitForDrink, //line to recieve drink
        waitForTable, //line to sit at table
        entering, //customer is entering
        inRestroom, //customer is in restroom
        sitting, //customer is sitting at table
        leaving //customer is leaving
    }


    // Start is called before the first frame update
    void Start()
    {
        //Determine what the customer is at the restaurant for
        float initialGoal = Random.Range(0f, 1f);
        Debug.Log("initial goal chance was: " + initialGoal);
        if (initialGoal <= initialLeaveChance)
        {
            this.goal = customerGoal.leaveCafe;
        }
        else if (initialGoal <= initialRestroomChance)
        {
            this.goal = customerGoal.useRestroom;
        }
        else if (initialGoal <= initialSitGoalChance)
        {
            this.goal = customerGoal.sitDown;
        }
        else 
        {
            this.goal = customerGoal.getDrink;
            //Deterrmine if order is online or not
            this.onlineOrder = (Random.Range(0f, 1f) <= this.onlineChance);
            //if it's online, join pickup area set.
            this.store.OrderWaitEnqueue(this);
        }

        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Set State

    //State updates
}
