using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    private Color emptyColor = Color.white;
    private Color occupiedColor = Color.red;
    private Color needsCleaningColor = Color.yellow;

    public enum ChairState { 
        Empty,
        Occupied,
        NeedsCleaning,
        NullChair               // If something goes wrong and the chair shouldn't exist; it will default to Null
    }

    ChairState chairState = ChairState.NullChair;
    [SerializeField] private bool hasOutlet;
    private SpriteRenderer sr;

    private void OnEnable()
    {
        chairState = ChairState.Empty;
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null) {
            Debug.Log("No sprite renderer found on chair object. Disabling this object.");
            gameObject.SetActive(false);
        }
    }

    private ChairState UpdateState(ChairState newState) {
        chairState = newState;
        switch (newState) {
            case ChairState.Empty:
                chairState = newState;
                //sr.color = Color.
                break;
            case ChairState.Occupied:
                chairState = newState;
                break;
            case ChairState.NeedsCleaning:
                chairState = newState;
                break;
            case ChairState.NullChair:
                Debug.Log("You're trying to NULL a chair out of existence. Are you sure you want to do this?");
                break;
        }

        return chairState;
    }
}
