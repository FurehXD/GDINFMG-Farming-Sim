using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubCropStorageCellButton : BaseButton
{
    private CropStorageCellButton parentCreator;

    private bool hasBeenInitialized = false;
    private QualityData quality;
    public QualityData Quality {  get { return quality; } }

    private Crop cropItStores;

    private CropProducer cropProducerReference;

    private int quantity = 1;
    private TextMeshProUGUI quantityTMPDisplay;

    public void Initialize(QualityData quality, Crop storingCrop, CropStorageCellButton parentCreator)
    {
        this.quality = quality; 
        this.cropItStores = storingCrop;
        this.parentCreator = parentCreator;

        this.GetComponentInChildren<IconApplier>().ApplyIcon(this.cropItStores.CropIcon);
        QualitySetter qualityIndicatorComponent = this.GetComponentInChildren<QualitySetter>();
        qualityIndicatorComponent.SetQuality(this.quality.QualityID);

        this.hasBeenInitialized = true; 
    }
    protected override void PerformClick()
    {
        base.PerformClick();

        this.parentCreator.SendConfirmation();
    }
    protected override void Start()
    {
        base.Start();

        if (!this.hasBeenInitialized)
            Debug.LogError(this.name + "  was not initialized!");

        this.quantityTMPDisplay = this.GetComponentInChildren<TextMeshProUGUI>();
    }
    protected override void Update()
    {
        base.Update();
        //this.cropItStores.CropID = this.cropIDItStores;
        this.quantityTMPDisplay.text = "x" + this.quantity;

        if (this.quantity == 0)
            this.interactable = false;
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
    public void StoreBoughtItem()
    {
        this.quantity++;
    }
}
