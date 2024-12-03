using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    [Header("Singleton Manager")]
    public static ShopManager Instance;
    [SerializeField]    public List<Button> _cropButtons;
    [SerializeField]    public List<Button> _fertilizerButtons;
    [SerializeField]    public Button _confirmButton;
    [SerializeField]    public Button _returnButton;
    [SerializeField]    public TMP_Text _purchaseMessage;
    
    void Awake() {
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
        if(Money.Instance.CurrentMoney >= cost) {
            Money.Instance.Buy(cost);
            return true;
        }
        else{
            return false;
        }
    }

    public async void BuySeed(int seedID) {
        //get item form database using id
        //add crop to inventory
        Debug.Log("Attempting to buy thingy.");
        int cropPrice = await DataRetriever.Instance.RetrieveCropPurchasingPrice(seedID);
        if(this.CheckPurchase(cropPrice)) {
            InventoryManager.Instance.StoreBoughtItem(seedID, this.RandomizeQuality());
            Debug.Log("Success");
            this._purchaseMessage.text = "Purchase of " + await DataRetriever.Instance.RetrieveCropName(seedID) + " seed successful.";
            Logger.Instance.LogMessage("Purchase of " + await DataRetriever.Instance.RetrieveCropName(seedID) + " seed successful.");
        }
        else {
            Debug.Log("Fail");
            this._purchaseMessage.text = "Purchase failure, not enough money.";
            Logger.Instance.LogMessage("Purchase failed");
        }
        
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

    public QualityData RandomizeQuality() {
        int numHold = Random.Range(1, 100);
        QualityData quality = new QualityData(0, "", 0);
        if (numHold > 0 && numHold < 5) {
            quality = new QualityData(1, "Poor", 0.4f);
        }
        else if (numHold > 6 && numHold < 25) {
            quality = new QualityData(2, "Normal", 0.8f);
        }
        else if (numHold > 26 && numHold < 75) {
            quality = new QualityData(3, "Good", 1.0f);
        }
        else if (numHold > 76 && numHold < 95) {
            quality = new QualityData(4, "Excellent", 1.2f);
        }
        else if (numHold > 96 && numHold < 100) {
            quality = new QualityData(5, "Amazing", 1.5f);
        }
        return quality;
    }
}
