using System;
using TMPro;
using UnityEditor.Events;
using UnityEngine;

public class CropStorageCellButton : BaseButton
{
    [SerializeField]
    [Header("CropStorageCellButton")]
    [Range(1, 50)]
    private int cropIDItStores;
    public int CropIDItStores {  get { return this.cropIDItStores; } set { this.cropIDItStores = value; } }

    private Crop cropItStores;

    private int quantity = 1;
    private TextMeshProUGUI quantityTMPDisplay;

    private BaseButton selectedPlotReference;
    private CropProducer cropProducerReference;

    protected override void OnEnable()
    {
        base.OnEnable();
        InventoryManager.OnInventoryOpened += this.EnableButton;
        InventoryManager.OnInventoryClosed += this.DisableButton;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        InventoryManager.OnInventoryOpened -= this.EnableButton;
        InventoryManager.OnInventoryClosed -= this.DisableButton;
    }
    protected override void Start()
    {
        base.Start();
        this.interactable = false;
        this.cropItStores = this.GetComponentInChildren<Crop>();
        this.quantityTMPDisplay = this.GetComponentInChildren<TextMeshProUGUI>();

#if UNITY_EDITOR
        //// Remove any existing persistent calls to avoid duplication
        //UnityEditor.Events.UnityEventTools.RemovePersistentListener(myButton.onClick, ExampleMethod1);
        //UnityEditor.Events.UnityEventTools.RemovePersistentListener(myButton.onClick, ExampleMethod2);

        //// Add persistent listeners for the button's OnClick event
        //UnityEventTools.AddPersistentListener(myButton.onClick, ExampleMethod1);
        //UnityEventTools.AddPersistentListener(myButton.onClick, ExampleMethod2);
#endif
    }
    public void ConsumeSeed()
    {
        this.quantity--;

        if (this.quantity < 0) 
            this.quantity = 0;
    }
    public void SignalStartProduction()
    {
        this.cropProducerReference = InventoryManager.Instance.CropProducerReference;

        this.cropProducerReference.StartProduction(this.cropItStores);
    }
    protected override void Update()
    {
        base.Update();
        this.cropItStores.CropID = this.cropIDItStores;
        this.quantityTMPDisplay.text = "x" + this.quantity;

        if (this.quantity == 0)
            this.interactable = false;
    }
}
