using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UpgradeButtons : MonoBehaviour {
    [SerializeField]
    TMP_Text shopMessage;
    [SerializeField]    public List<Button> _upgradeButtons;
    public static event Action<float> OnLuckyCharmBought;
    private float rarityBuffFactor = 1.25f;
    public static event Action<float> OnGrowthTimeUpgrade;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public async void LuckyCharm() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(18);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.LuckyCharm();
            OnLuckyCharmBought?.Invoke(this.rarityBuffFactor);
            this.shopMessage.text = "Rarity Boosted!";
            this._upgradeButtons[0].interactable = false;
            Logger.Instance.LogMessage("Purchase of " + await DataRetriever.Instance.RetrieveCropName(18) + "seed successful.");
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
            Logger.Instance.LogMessage("Purchase failed");
        }    
    }

    public async void Pesticide() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(19);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.Pesticide();
            OnGrowthTimeUpgrade?.Invoke(0.2f);
            this.shopMessage.text = "Fertilizer Boosted!";
            this._upgradeButtons[1].interactable = false;
            Logger.Instance.LogMessage("Purchase of " + await DataRetriever.Instance.RetrieveCropName(19) + "seed successful.");
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
            Logger.Instance.LogMessage("Purchase failed");
        }
    }

    public async void Earthworms() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(20);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.Earthworms();
            OnGrowthTimeUpgrade?.Invoke(0.4f);
            this.shopMessage.text = "Fertilizer Boosted!";
            this._upgradeButtons[2].interactable = false;
            Logger.Instance.LogMessage("Purchase of " + await DataRetriever.Instance.RetrieveCropName(20) + "seed successful.");
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
            Logger.Instance.LogMessage("Purchase failed");
        }
    }

    public async void MarketConnection() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(21);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.MarketConnection();
            this.shopMessage.text = "Sell Value Boosted!";
            this._upgradeButtons[3].interactable = false;
            Logger.Instance.LogMessage("Purchase of " + await DataRetriever.Instance.RetrieveCropName(21) + "seed successful.");
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
            Logger.Instance.LogMessage("Purchase failed");
        }
    }

    public async void Advertisement() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(18);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.Advertisement();
            this.shopMessage.text = "Sell Value Boosted!";
            this._upgradeButtons[4].interactable = false;
            Logger.Instance.LogMessage("Purchase of " + await DataRetriever.Instance.RetrieveCropName(22) + "seed successful.");
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
            Logger.Instance.LogMessage("Purchase failed");
        }
    }

    public async void ChemicalInjection() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(18);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.ChemicalInjection();
            this.shopMessage.text = "Yield Doubled!";
            this._upgradeButtons[5].interactable = false;
            Logger.Instance.LogMessage("Purchase of " + await DataRetriever.Instance.RetrieveCropName(23) + "seed successful.");
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
            Logger.Instance.LogMessage("Purchase failed");
        }
    }
}
