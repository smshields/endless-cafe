using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private bool useSpecificSeed = false;          // If we have a specific seed to test...
    [SerializeField] private int seed = 0;                              // Define this seed here
    private void Awake()
    {
        if (useSpecificSeed){
            Random.InitState(seed);                                 // Will initiate Random using the given seed if requested for reproducing results
        }
        Debug.Log("Initiating scene with seed: " + Random.seed);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
