using UnityEngine;

public class ShopManager : MonoBehaviour {
    [Header("Singleton Manager")]
    public static ShopManager Instance;
    
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

    public bool CheckPurchase(int cost) {
        //if(money >= cost) {}
        //return true;
        //else{}
        return false;
    }
}
