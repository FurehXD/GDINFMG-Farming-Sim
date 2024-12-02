using System;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {
    [Header("Singleton Manager")]
    public static UpgradeManager Instance;
    [SerializeField] private float rarityBoost;
    public float RarityBoost {
        get { return this.rarityBoost; }
    }
    [SerializeField] private float fertilizerBoost;
    public float FertilizerBoost {
        get { return this.fertilizerBoost; }
    }
    [SerializeField] private int sellValueBoost;
    public int SellValueBoost {
        get { return this.sellValueBoost; }
    }
    [SerializeField] private int yieldAmount;
    public int YieldAmount {
        get { return this.yieldAmount; }
    }


    public static event Action<float> OnLuckyCharmBought;
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("UpgradeManager initialized");
        }
        else {
            Destroy(this.gameObject);
        }
        Debug.Log("UpgradeManager initialized");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.rarityBoost = 0.0f;
        this.fertilizerBoost = 0.0f;
        this.sellValueBoost = 0;
        this.yieldAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LuckyCharm() {
        this.rarityBoost += 0.1f;

        OnLuckyCharmBought?.Invoke(this.rarityBoost);
    }

    public void Pesticide() {
        this.fertilizerBoost += 0.1f;
    }

    public void Earthworms() {
        this.fertilizerBoost += 0.15f;
    }

    public void MarketConnection() {
        this.sellValueBoost += 20;
    }

    public void Advertisement() {
        this.sellValueBoost += 40;
    }

    public void ChemicalInjection() {
        this.yieldAmount = 2;
    }
}
