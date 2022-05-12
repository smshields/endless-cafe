using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    private Color emptyColor = Color.white;
    private Color occupiedColor = new Color(1.0f, 0.1921569f, 0.1921569f,1.0f);
    private Color needsCleaningColor = new Color(1.0f, 0.509804f, 0.2509804f, 1.0f);

    public enum ChairState { 
        Empty,
        Occupied,
        NeedsCleaning,
        NullChair               // If something goes wrong and the chair shouldn't exist; it will default to Null
    }

    ChairState chairState = ChairState.NullChair;
    [SerializeField] private bool hasOutlet;
    private SpriteRenderer sr;

    [Tooltip("Minimum clean time in minutes")][SerializeField] private float minCleanTime = 1.0f;
    [Tooltip("Maximum clean time in minutes")][SerializeField] private float maxCleanTime = 20.0f;
    [Tooltip("Fixed time in seconds between rolls to check for cleaning")] [SerializeField] private float intervalCleanTime = 1.0f;
    private float nextCleanCheck = 0.0f;

    [Tooltip("Chance that seat will be cleaned in the next tick")] [SerializeField] private float cleanChance = 0.4f;
    
    

    private void OnEnable()
    {
        chairState = ChairState.Empty;
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null) {
            Debug.Log("No sprite renderer found on chair object. Disabling this object.");
            gameObject.SetActive(false);
        }
        sr.color = emptyColor;

        
    }

    private void Update()
    {
        if (chairState == ChairState.NeedsCleaning && Time.time >= nextCleanCheck) {
            float cleanCheck = Random.Range(0.0f, 1.0f);
            if (cleanCheck <= cleanChance) {
                SetState(ChairState.Empty);
                ChairManager.unoccupiedChairs.Add(this);
                //Debug.Log("Unoccupied count: " + ChairManager.unoccupiedChairs.Count);
            }
            else {
                nextCleanCheck = Time.time + intervalCleanTime;
            }
        }
    }

    public ChairState SetState(ChairState newState) {
        chairState = newState;
        switch (newState) {
            case ChairState.Empty:
                chairState = newState;
                sr.color = emptyColor;
                break;
            case ChairState.Occupied:
                chairState = newState;
                sr.color = occupiedColor;
                break;
            case ChairState.NeedsCleaning:
                chairState = newState;
                sr.color = needsCleaningColor;
                break;
            case ChairState.NullChair:
                Debug.Log("You're trying to NULL a chair out of existence. Are you sure you want to do this?");
                break;
        }

        return chairState;
    }

    public ChairState GetState() {
        return chairState;
    }

    public bool IsUnoccupied() {
        if (chairState == ChairState.Empty)
            return true;
        return false;
    }

    public bool HasOutlet() {
        return hasOutlet;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (chairState == ChairState.Empty) {
            if (collision.gameObject.CompareTag("Customer"))
            {
                SetState(ChairState.Occupied);
                //Debug.Log("Chair now occupied");
                ChairManager.unoccupiedChairs.Remove(this);
                //Debug.Log("Unoccupied count: " + ChairManager.unoccupiedChairs.Count);
            }
        }
        else {
            Debug.Log("Chair is not empty. Cannot occupy new customer.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (chairState == ChairState.Occupied) {
            if (collision.gameObject.CompareTag("Customer")) {
                SetState(ChairState.NeedsCleaning);
                //Debug.Log("Chair now Needs Cleaning");
                nextCleanCheck = Time.time + intervalCleanTime;
            }
        }
    }
}
