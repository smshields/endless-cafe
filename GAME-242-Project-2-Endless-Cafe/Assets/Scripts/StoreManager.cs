using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    //Order Line
    public Queue<Customer> orderLine;

    //Order Wait
    public Queue<Customer> orderWait = new Queue<Customer>(); //Queue of customer objects waiting for order
    public GameObject orderWaitGO; //game object for location movement
    public float orderWaitMinInterval = 1f; //shortest time a customer can wait for a order
    public float orderWaitMaxInterval = 5f; //longest time a custoer can wait for an order
    public float orderWaitDuration = 1f; //how long the current wait will be
    public float nextOrderWait = 0f; //time until next order is given to a customer 

    //Bathroom Line
    public Queue<Customer> bathroomLine;

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
        //If order is ready, and customer is waiting, give to customer
        if (Time.time > this.nextOrderWait && this.orderWait.Count > 0) {
            this.OrderWaitDequeue();
            //Set new randomized wait time for next customer.
            this.orderWaitDuration = Random.Range(this.orderWaitMinInterval, this.orderWaitMaxInterval);
            this.nextOrderWait = Time.time + this.orderWaitDuration;
        }
    }

    public int OrderWaitEnqueue(Customer customer) 
    {
        //move customer to order wait
        customer.transform.position = orderWaitGO.transform.position;
        customer.state = Customer.customerState.waitForOrder;
        orderWait.Enqueue(customer);
        Debug.Log("Customer started waiting for order. Total customers waiting for order: " + orderWait.Count);
        return orderWait.Count;
    }

    public int OrderWaitDequeue() 
    {
        //Do not dequeue if no one is waiting.
        if (orderWait.Count > 0) 
        {
            //Call customer "order recieved" method
            Customer customer = orderWait.Dequeue();
            Debug.Log("Customer recieved order. Total customers waiting for order: " + orderWait.Count);
        }
        return orderWait.Count;
    }

    public int OrderLineEnqueue(Customer customer)
    {
        orderLine.Enqueue(customer);
        return orderLine.Count;
    }

    public int BathroomLineEnqueue(Customer customer)
    {
        bathroomLine.Enqueue(customer);
        return bathroomLine.Count;
    }

}
