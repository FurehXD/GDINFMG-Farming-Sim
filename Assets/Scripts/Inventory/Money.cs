using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    public static Money Instance;

    private TextMeshProUGUI moneyDisplay;
    [SerializeField]
    [Range(0, 999)]
    private int startingMoney = 0;

    private int currentMoney = 0;
    public int CurrentMoney { get { return this.currentMoney; } }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }
    private void Start()
    {
        this.currentMoney = this.startingMoney;
        this.moneyDisplay = this.GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        this.SetText(this.currentMoney);
    }
    private void SetText(int amount)
    {
        this.moneyDisplay.text = "$" + amount;
    }
    public void Buy(int cost)
    {
        this.currentMoney -= cost;

        if(this.currentMoney < 0)
            this.currentMoney = 0;
    }
    public void Sell(int earnings)
    {
        this.currentMoney += (earnings + UpgradeManager.Instance.SellValueBoost) * UpgradeManager.Instance.YieldAmount;
    }
}
