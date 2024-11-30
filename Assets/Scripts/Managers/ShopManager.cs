using System.Collections.Generic;
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
}
