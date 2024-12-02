using UnityEngine;
using TMPro;

public class UpgradeButtons : MonoBehaviour {
    [SerializeField]
    TMP_Text shopMessage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LuckyCharm() {
        UpgradeManager.Instance.LuckyCharm();
        this.shopMessage.text = "Rarity Boosted!";
    }

    public void Pesticide() {
        UpgradeManager.Instance.Pesticide();
        this.shopMessage.text = "Fertilizer Boosted!";
    }

    public void Earthworms() {
        UpgradeManager.Instance.Earthworms();
        this.shopMessage.text = "Fertilizer Boosted!";
    }

    public void MarketConnection() {
        UpgradeManager.Instance.MarketConnection();
        this.shopMessage.text = "Sell Value Boosted!";
    }

    public void Advertisement() {
        UpgradeManager.Instance.Advertisement();
        this.shopMessage.text = "Sell Value Boosted!";
    }

    public void ChemicalInjection() {
        UpgradeManager.Instance.ChemicalInjection();
        this.shopMessage.text = "Yield Doubled!";
    }
}
