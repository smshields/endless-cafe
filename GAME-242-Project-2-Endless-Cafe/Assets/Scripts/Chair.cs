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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (chairState == ChairState.Empty) {
            if (collision.gameObject.CompareTag("Customer"))
            {
                SetState(ChairState.Occupied);
                //sr.color = occupiedColor;
                Debug.Log("Chair now occupied");
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
                Debug.Log("Chair now Needs Cleaning");
            }
        }
    }
}
