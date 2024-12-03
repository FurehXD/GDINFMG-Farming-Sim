using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UpgradeButtons : MonoBehaviour {
    [SerializeField]
    TMP_Text shopMessage;
    [SerializeField]    public List<Button> _upgradeButtons;
    
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
            this.shopMessage.text = "Rarity Boosted!";
            this._upgradeButtons[0].interactable = false;
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
        }    
    }

    public async void Pesticide() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(19);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.Pesticide();
            this.shopMessage.text = "Fertilizer Boosted!";
            this._upgradeButtons[1].interactable = false;
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
        }
    }

    public async void Earthworms() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(20);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.Earthworms();
            this.shopMessage.text = "Fertilizer Boosted!";
            this._upgradeButtons[2].interactable = false;
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
        }
    }

    public async void MarketConnection() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(21);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.MarketConnection();
            this.shopMessage.text = "Sell Value Boosted!";
            this._upgradeButtons[3].interactable = false;
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
        }
    }

    public async void Advertisement() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(18);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.Advertisement();
            this.shopMessage.text = "Sell Value Boosted!";
            this._upgradeButtons[4].interactable = false;
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
        }
    }

    public async void ChemicalInjection() {
        int price = await DataRetriever.Instance.RetrieveCropPurchasingPrice(18);
        if(this.CheckPurchase(price)) {
            UpgradeManager.Instance.ChemicalInjection();
            this.shopMessage.text = "Yield Doubled!";
            this._upgradeButtons[5].interactable = false;
        }
        else {
            this.shopMessage.text = "Not enough money.";
            Debug.LogWarning("Not enough money.");
        }
    }
}
