using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    private bool useSpecificSeed = false;          // If we have a specific seed to test...
    private int seed = 0;                              // Define this seed here

    [SerializeField] private static float timeScale = 1.0f;
    [SerializeField] private Text currentSeedText;
    [SerializeField] private InputField enteredText;
    [SerializeField] private Animator optionMenuAnimation;
    [SerializeField] private Text timeText;
    [SerializeField] private int openingHour = 6;
    [SerializeField] private int openingMinute = 30;
    [SerializeField] private int closingHour = 0;
    [SerializeField] private int closingMinute = 0;
    private static int timeHour = 0;
    private static int timeMinute = 0;
    private float nextUpdateTime;
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
        timeHour = openingHour;
        timeMinute = openingMinute;
        nextUpdateTime = Time.time + (1.0f / timeScale);
        timeText.text = TimeToString();
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime) {
            timeMinute += 1;
            if (timeMinute >= 60) {
                timeMinute = 0;
                timeHour += 1;
                if (timeHour >= 24)
                    timeHour = 0;
            }
            nextUpdateTime = Time.time + (1.0f / timeScale);
            timeText.text = TimeToString();
        }
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
        SceneManager.LoadScene(0);
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

    public string TimeToString() {
        string hstr = timeHour.ToString();
        string mstr = timeMinute.ToString();
        if (timeHour < 10)
            hstr = "0" + hstr;
        if (timeMinute < 10)
            mstr = "0" + mstr;
        return hstr + ":" + mstr;
    }

    public static int GetCurrentHour() {
        return timeHour;
    }

    public static int GetCurrentMinute() {
        return timeMinute;
    }
}
