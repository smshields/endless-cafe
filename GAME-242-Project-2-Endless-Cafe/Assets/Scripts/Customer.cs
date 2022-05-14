using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    //TODO: Store Manager - load on creation
    public StoreManager store;

    //tracks current goal and state of customer
    public customerState state = customerState.entering;
    public customerGoal goal = customerGoal.leaveCafe;

    //Variables indicating likelihood of initial goal assigmnment - hard coding probability ranges for now
    public float initialDrinkGoalChance = 1f; //70%
    public float initialSitGoalChance = 0.3f; //25%
    public float initialRestroomChance = 0.05f; //04%
    public float initialLeaveChance = 0.01f; //01%

    //Variables indicating likelihood of next action after recieving a drink
    public float leaveAfterDrinkChance = 1f; //45%
    public float sitAfterDrinkChance = 0.55f; //45%
    public float restroomAfterDrinkChance = 0.1f; //10%

    //Variables indicating likelihood of next action after using the bathroom
    public float leaveAfterBathroomChance = 1.0f; //33.3%
    public float sitAfterBathroomChance = 0.67f; //33.3%
    public float drinkAfterBathroomChance = 0.33f; //33.3%

    //probability that an order is online or not
    public float onlineChance = 0.05f; //50%

    //Tracks if the customer's drink order was online or not.
    public bool onlineOrder = false;

    //Tracks if a customer has already done an action within the Cafe
    public bool hasPlacedOrder = false;
    public bool hasDrink = false;
    public bool hasUsedBathroom = false;
    public bool hasSatAtTable = false;

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
        if (initialGoal <= this.initialLeaveChance)
        {
            this.goal = customerGoal.leaveCafe;
        }
        else if (initialGoal <= this.initialRestroomChance && !this.hasUsedBathroom)
        {
            this.goal = customerGoal.useRestroom;
            this.store.BathroomLineEnqueue(this);
        }
        else if (initialGoal <= this.initialSitGoalChance)
        {
            this.goal = customerGoal.sitDown;
        }
        else if(initialGoal <= this.initialDrinkGoalChance && !this.hasDrink)
        {
            this.goal = customerGoal.getDrink;
            //Deterrmine if order is online or not
            this.onlineOrder = (Random.Range(0f, 1f) <= this.onlineChance);
            if (this.onlineOrder)
            {
                //if it's online, join pickup area queue.
                this.hasPlacedOrder = true;
                this.store.OrderWaitEnqueue(this);
            }
            else 
            {
                //If it's in person, join order area queue.
                this.store.OrderLineEnqueue(this);
            }

        }

        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //NOTE: Receive drink does not use a co-routine as we assume drink delivery is linear - each drink being made blocks the next.
    public void ReceiveDrink() 
    {
        this.hasDrink = true;
        //Customer can now: sit, leave, use restroom
        float goalAfterDrink = Random.Range(0f, 1f);
        if (goalAfterDrink <= restroomAfterDrinkChance && !this.hasUsedBathroom) {
            this.goal = customerGoal.useRestroom;
            this.store.BathroomLineEnqueue(this);
        }
        else if (goalAfterDrink <= sitAfterDrinkChance && !this.hasSatAtTable) {
            this.goal = customerGoal.sitDown;
        }
        else {
            //Destroy here?
            this.goal = customerGoal.leaveCafe;
        }
    }

    public void UseBathroom() 
    {
        this.hasUsedBathroom = true;
        //Customer can now: sit, get drink, leave
        float goalAfterBathroom = Random.Range(0f, 1f);
        if (goalAfterBathroom <= drinkAfterBathroomChance && !this.hasDrink)
        {
            this.goal = customerGoal.getDrink;
            //Deterrmine if order is online or not
            this.onlineOrder = (Random.Range(0f, 1f) <= this.onlineChance);
            if (this.onlineOrder)
            {
                //if it's online, join pickup area queue.
                this.hasPlacedOrder = true;
                this.store.OrderWaitEnqueue(this);
            }
            else
            {
                //If it's in person, join order area queue.
                this.store.OrderLineEnqueue(this);
            }
        }
        else if (goalAfterBathroom <= sitAfterDrinkChance && !this.hasSatAtTable)
        {
            this.goal = customerGoal.sitDown;
        }
        else
        {
            //Destroy here?
            this.goal = customerGoal.leaveCafe;
        }

    }

    public void PlaceDrinkOrder() 
    {
        //We assume person moves to waiting area after placing order
        this.hasPlacedOrder = true;
        this.store.OrderWaitEnqueue(this);
    }

    //Destroy customer, update store statistics
    public void LeaveCafe() { }
    //Set State

    //State updates
}
