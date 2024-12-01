using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    [Header("Singleton Manager")]
    public static ShopManager Instance;
    [SerializeField]
    private Money _money;
    [SerializeField]    public List<Button> _cropButtons;
    [SerializeField]    public List<Button> _fertilizerButtons;
    [SerializeField]    public List<Button> _upgradeButtons;
    [SerializeField]    public Button _confirmButton;
    [SerializeField]    public Button _returnButton;
    [SerializeField]    public TMP_Text _purchaseMessage;
    
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

    void InitializeShopItems() {
        //
    }

    public bool CheckPurchase(int cost) {
        if(_money.CurrentMoney >= cost) {
            _money.Buy(cost);
        }
        else{}
        return false;
    }

    public void BuySeed(int seedID) {
        //get item form database using id
        //add crop to inventory
        //if(this.CheckPurchase(cost)) {
            //randomize quality
            //add to inventory
            //change text to say purchase bought
        //}
        //else {
            //change text to say fail
        //}
        Debug.Log("Success");
        this._purchaseMessage.text = "Purchase successful.";
    }

    public void BuyItem(string itemID) {
        //get item form database using id
        //add crop to inventory
        //if(this.CheckPurchase(cost)) {
            //add to inventory
            //change text to say purchase bought
        //}
        //else {
            //change text to say fail
        //}
        Debug.Log("Success");
        this._purchaseMessage.text = "Purchase successful.";
    }

    public int RandomizeQuality() {
        int numHold = Random.Range(1, 100);
        int qualityID = 3;
        if (numHold > 0 && numHold < 10) {
            qualityID = 1;
        }
        else if (numHold > 11 && numHold < 35) {
            qualityID = 2;
        }
        else if (numHold > 36 && numHold < 65) {
            qualityID = 3;
        }
        else if (numHold > 66 && numHold < 90) {
            qualityID = 4;
        }
        else if (numHold > 91 && numHold < 100) {
            qualityID = 5;
        }
        return qualityID;
    }
}
