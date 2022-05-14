using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    //Order Line
    public Queue<Customer> orderLine = new Queue<Customer>(); //Queue of customer objects waiting to place an order
    public GameObject orderLineGO; //game object for location movement
    public bool cashierAvailable = true; //indicates if the line is open for a new customer
    public float orderTimeMinInterval = 2f; //shortest time a customer can take to place an order
    public float orderTimeMaxInterval = 7f; //longest time a customer can take to place an order
    public Customer customerPlacingOrder; //current customer placing order

    //Order Wait
    public Queue<Customer> orderWait = new Queue<Customer>(); //Queue of customer objects waiting for order
    public GameObject orderWaitGO; //game object for location movement
    public float orderWaitMinInterval = 1f; //shortest time a customer can wait for a order
    public float orderWaitMaxInterval = 5f; //longest time a custoer can wait for an order
    public float orderWaitDuration = 1f; //how long the current wait will be
    public float nextOrderWait = 0f; //time until next order is given to a customer 

    //Bathroom Line
    public Queue<Customer> bathroomLine = new Queue<Customer>(); //Queue of customer objects waiting for order 
    public GameObject bathroomLineGO;
    public GameObject bathroomGO;
    public bool bathroomVacant = true; //indicates if the bathroom is currently occupied or not.
    public Customer customerInBathroom; //current customer in bathroom
    public float bathroomWaitMinInterval = 3f; //shortest time a customer can USE the bathroom
    public float bathroomWaitMaxInterval = 15f; //longest time a cusomer can USE the bathroom

    //Seating
    public Queue<Customer> seatingWait = new Queue<Customer>(); // Queue of customer objects waiting for order
    public GameObject seatingWaitGO;
    public float sittingMinInterval = 5f; // Shortest time a customer can sit
    public float sittingMaxInterval = 30f; //Longest time a customer can sit
    public float outletBonusMin = 5f; // Additional time spent if there is an outlet
    public float outletBonusMax = 60f; // Additional time spent if there is an outlet
    public bool seatsAvailable = true;

    // Start is called before the first frame update
    void Start()
    {
        //initialize first wait time
        this.orderWaitDuration = Random.Range(this.orderWaitMinInterval, this.orderWaitMaxInterval);
        this.nextOrderWait = Time.time + this.orderWaitDuration;
    }

    // Update is called once per frame
    void Update()
    {
        //Orders are delivered linearly and blocking with a randomized range of wait times specified above.
        if (Time.time > this.nextOrderWait && this.orderWait.Count > 0) {
            this.OrderWaitDequeue();
            //Set new randomized wait time for next customer.
            this.orderWaitDuration = Random.Range(this.orderWaitMinInterval, this.orderWaitMaxInterval);
            this.nextOrderWait = Time.time + this.orderWaitDuration;
        }

        //if the bathroom is vacant and someone is in line, dequeue them and put them into the bathroom
        if (this.bathroomVacant && this.bathroomLine.Count > 0) {
            this.BathroomLineDequeue();
        }

        //if there is a cashier available and someone is in line, dequeue them and put them into the bathroom
        if (this.cashierAvailable && this.orderLine.Count > 0) {
            this.OrderLineDequeue();
        }

        //if there are seats available and someone is waiting for one, dequeue them and let them sit down.
        this.seatsAvailable = ChairManager.GetUnoccupiedChairs().Count > 0;
        if(this.seatsAvailable && this.seatingWait.Count > 0){
            this.SeatingWaitDequeue();    
        }

        
    }

    //Add customer to sitting waiting
    public int SeatingWaitEnqueue(Customer customer)
    {
        //move customer to sitting wait
        //add to queue for seating
        this.seatingWait.Enqueue(customer);
        Debug.Log("Customer started waiting for seating. Total customers waiting for order: " + orderWait.Count);
        return this.seatingWait.Count;
    }

    //Remove a customer from sitting waiting to table
    public int SeatingWaitDequeue()
    {
        List<Chair> unoccupiedChairs = ChairManager.GetUnoccupiedChairs();
        //Do not dequeue if no chairs are available
        if(unoccupiedChairs.Count <= 0)
        {
            Debug.Log("Attempted Dequeue when no chairs are available.");
            return this.seatingWait.Count;
        }
            
        //Do not dequeue if no one is waiting
        if (this.seatingWait.Count > 0) 
        {
            Customer customer = this.seatingWait.Dequeue();
            int seatIndex = Random.Range(0, unoccupiedChairs.Count-1);
            Chair seat = unoccupiedChairs[seatIndex];
            //update customer with seating

            StartCoroutine(this.SitDown(customer, seat));
            Debug.Log("Customer sat at a seat. Total customers waiting for a seat: " + seatingWait.Count);

        }
        return this.seatingWait.Count;
    }

    //Add customer to order waiting
    public int OrderWaitEnqueue(Customer customer) 
    {
        //move customer to order wait
        customer.transform.position = orderWaitGO.transform.position;
        customer.state = Customer.customerState.waitForDrink;
        //add to queue for order waiting
        orderWait.Enqueue(customer);
        Debug.Log("Customer started waiting for order. Total customers waiting for order: " + orderWait.Count);
        return orderWait.Count;
    }
    //Remove a customerr from order waiting line
    public int OrderWaitDequeue() 
    {
        //Do not dequeue if no one is waiting.
        if (orderWait.Count > 0) 
        {
            Customer customer = orderWait.Dequeue();
            //update customer with drink, tell them to pursue next goal
            customer.ReceiveDrink();
            Debug.Log("Customer recieved order. Total customers waiting for order: " + orderWait.Count);

        }
        return orderWait.Count;
    }

    public int OrderLineEnqueue(Customer customer)
    {
        //TODO: move customer to order line wait
        customer.state = Customer.customerState.waitForOrder;
        orderLine.Enqueue(customer);
        Debug.Log("Customer in line to place an order. Total customers waiting to place an order: " + orderLine.Count);
        return orderLine.Count;
    }

    public int OrderLineDequeue() 
    {
        //Do not dequeue if no one is waiting
        if (orderLine.Count > 0)
        {
            Customer customer = orderLine.Dequeue();
            StartCoroutine(PlaceDrinkOrder(customer));
            //update customer with order, tell them to pursue next goal
            Debug.Log("Customer placed order. Total customers waiting to place order: " + orderLine.Count);
        }
        else 
        {
            Debug.Log("Attempted to dequeue empty order line.");
        }
        return orderLine.Count;
    }

    public int BathroomLineEnqueue(Customer customer)
    {
        //TODO: move customer to bathroom wait position
        customer.state = Customer.customerState.waitForBathroom;
        bathroomLine.Enqueue(customer);
        Debug.Log("Customer in line for the bathroom. Total customers  waiting for bathroom: " + bathroomLine.Count);
        return bathroomLine.Count;
    }

    public int BathroomLineDequeue() {
        //Do not dequeue if no one is waiting
        if (bathroomLine.Count > 0 && this.bathroomVacant)
        {
            Customer customer = bathroomLine.Dequeue();
            StartCoroutine(UseBathroom(customer));
            Debug.Log("Customer in bathroom. Total customers waiting for bathroom: " + bathroomLine.Count);
        }
        else 
        {
            Debug.Log("Attempted to dequeue empty bathroom line.");
        }
        return bathroomLine.Count;
    }

    IEnumerator PlaceDrinkOrder(Customer customer) {
        this.cashierAvailable = false;
        this.customerPlacingOrder = customer;
        float timeToPlaceOrder = Random.Range(this.orderTimeMinInterval, this.orderTimeMaxInterval);
        customer.state = Customer.customerState.placingOrder;
        yield return new WaitForSeconds(timeToPlaceOrder);
        Debug.Log("Customer finished placing order.");
        this.cashierAvailable = true;
        this.customerPlacingOrder = null;
        customer.PlaceDrinkOrder();
    }

    IEnumerator UseBathroom(Customer customer) {
        this.bathroomVacant = false;
        this.customerInBathroom = customer;
        float timeInBathroom = Random.Range(this.bathroomWaitMinInterval, this.bathroomWaitMaxInterval);
        yield return new WaitForSeconds(timeInBathroom);
        Debug.Log("Customer left bathroom.");
        this.bathroomVacant = true;
        this.customerInBathroom = null;
        customer.UseBathroom();
    }

    IEnumerator SitDown(Customer customer, Chair chair) {
        //TODO: move customer to table


        //wait some time
        float timeAtTable = Random.Range(sittingMinInterval, sittingMaxInterval);
        //if table has an outlet, increase time
        if(chair.HasOutlet()){
            timeAtTable += Random.Range(outletBonusMin, outletBonusMax);
        }
        Debug.Log("Customer took a seat.");
        //make customer leave or use restroom
        yield return new WaitForSeconds(timeAtTable);
        Debug.Log("Customer left seat.");
        customer.LeaveSeat();
    }
}
