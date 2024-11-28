using UnityEngine;
using System;

public class TimeManager : MonoBehaviour {
    [Header("Singleton Manager")]
    public static TimeManager Instance;
    void OnAwake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextDay() {
        //
    }

    public void NextSeason() {
        //
    }
}
