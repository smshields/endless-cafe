using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private bool useSpecificSeed = false;          // If we have a specific seed to test...
    [SerializeField] private int seed = 0;                              // Define this seed here

    [SerializeField] private static float timeScale = 1.0f;
    [SerializeField] private Text currentSeedText;
    [SerializeField] private InputField enteredText;
    [SerializeField] private Animator optionMenuAnimation;
    private static int enteredSeedValue;
    private static bool valueRequested;
    private void Awake()
    {
        if (useSpecificSeed){
            Random.InitState(seed);                                 // Will initiate Random using the given seed if requested for reproducing results
        }
        Debug.Log("Initiating scene with seed: " + Random.seed);
        if (currentSeedText != null) {
            currentSeedText.text = "Current: " + Random.seed.ToString();
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static float GetTimeScale() {
        return timeScale;
    }

    public static int SetTimeScale() {
        switch (timeScale) {
            case 1.0f:
                timeScale = 2.0f;
                break;
            case 2.0f:
                timeScale = 5.0f;
                break;
            case 5.0f:
                timeScale = 10.0f;
                break;
            case 10.0f:
                timeScale = 1.0f;
                break;
        }
        return (int)timeScale;
    }

    public static void ReloadScene() {
        SceneManager.LoadScene(1);
        if (valueRequested) {
            Random.InitState(enteredSeedValue);
        }
        else {
            Random.InitState((int)System.DateTime.Now.Ticks);
        }
        valueRequested = false;
    }

    public void SetEnteredSeedValue() {
        if (enteredText.text != ""){
            enteredSeedValue = int.Parse(enteredText.text);
            valueRequested = true;
        }
        else {
            valueRequested = false;
        }
    }

    public void ToggleOptionsMenu() {
        optionMenuAnimation.SetBool("open", !optionMenuAnimation.GetBool("open"));
    }
}
