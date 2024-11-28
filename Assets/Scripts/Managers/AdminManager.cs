using UnityEngine;
using System;

public class AdminManager : MonoBehaviour {
    [Header("Singleton Manager")]
    public static AdminManager Instance;
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
}
