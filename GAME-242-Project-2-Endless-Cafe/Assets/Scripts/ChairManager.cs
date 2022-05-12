using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairManager : MonoBehaviour
{
    [SerializeField] private GameObject chairsParentGO;
    public static List<Chair> unoccupiedChairs = new List<Chair>();
    
    // Start is called before the first frame update
    void Start(){
        Chair[] chairList = chairsParentGO.transform.GetComponentsInChildren<Chair>();
        foreach (Chair c in chairList) {
            unoccupiedChairs.Add(c);
        }
    }

    public static List<Chair> GetUnoccupiedChairs() {
        return unoccupiedChairs;
    }
}
