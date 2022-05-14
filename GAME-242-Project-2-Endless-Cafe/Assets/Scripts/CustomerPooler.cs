using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPooler : MonoBehaviour
{
    [SerializeField] private GameObject customerGO;
    [SerializeField] private Vector3 spawnLocation;
    [SerializeField] private List<int> numOfNewCustomersPerHour;
    private List<int> currentSpawnTimes = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        if (numOfNewCustomersPerHour.Count < (24 - SceneHandler.GetOpeningHour())) {
            Debug.LogWarning("The number of customers requested pepr hour does not match the number of opening hours. Fix this and re-run the scene.");
            this.enabled = false;
            return;
        }
        currentSpawnTimes.Clear();
        if(numOfNewCustomersPerHour[0] > 0)
            PopulateWithCustomers(numOfNewCustomersPerHour[0], SceneHandler.GetOpeningMinute());

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneHandler.GetCurrentMinute() == 0 && currentSpawnTimes.Count == 0){
            if(SceneHandler.GetCurrentHour() >= SceneHandler.GetOpeningHour()){
                int hourIndex = SceneHandler.GetCurrentHour() - SceneHandler.GetOpeningHour();
                if (numOfNewCustomersPerHour[hourIndex] > 0)
                    PopulateWithCustomers(numOfNewCustomersPerHour[hourIndex]);
            }
            
        }
        else if (currentSpawnTimes.Count > 0) {
            while (currentSpawnTimes[0] <= SceneHandler.GetCurrentMinute()) {
                Instantiate(customerGO, spawnLocation, Quaternion.identity, this.transform);
                currentSpawnTimes.RemoveAt(0);
                if (currentSpawnTimes.Count == 0) {
                    break;
                }
            }
        }
    }

    private void PopulateWithCustomers(int numOfCustomers, int startMinute = 0, int endMinute = 59) {
        for (int i = 0; i < numOfCustomers; i++) {
            currentSpawnTimes.Add(Random.Range(startMinute, endMinute));
        }
        currentSpawnTimes.Sort();
        /*for (int i = 0; i < currentSpawnTimes.Count; i++) {
            Debug.Log(i + ": " + currentSpawnTimes[i]);
        }*/
    }
}
