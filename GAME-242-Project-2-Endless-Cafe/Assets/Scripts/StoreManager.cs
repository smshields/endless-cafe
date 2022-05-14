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
    public bool bathroomVacant = true; //indicates if the bathroom is currently occupied or not.
    public Customer customerInBathroom; //current customer in bathroom
    public float bathroomWaitMinInterval = 3f; //shortest time a customer can USE the bathroom
    public float bathroomWaitMaxInterval = 15f; //longest time a cusomer can USE the bathroom

    //Seating
    public Dictionary<int, Customer> seatingWait;

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
    }

    //Add customer to order waiting
    public int OrderWaitEnqueue(Customer customer) 
    {
        //move customer to order wait
        customer.transform.position = orderWaitGO.transform.position;
        customer.state = Customer.customerState.waitForOrder;
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
}
