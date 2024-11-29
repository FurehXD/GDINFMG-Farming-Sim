using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor.Events; // Required for adding persistent calls
#endif

public class PlotButton : BaseButton
{
    [SerializeField]
    private Enbiggener inventoryEnbiggener;

    private ColorChanger borderColorChanger;
    private CropProducer cropProducerReference;

    private bool readyForHarvest = false;

    public static event Action<PlotButton> OnPlotSelected;

    protected override void Start()
    {
        base.Start();
        this.borderColorChanger = this.GetComponentInChildren<ColorChanger>();
        this.cropProducerReference = this.GetComponentInChildren<CropProducer>();
    }
    public void UnreadyForHarvest()
    {
        this.readyForHarvest = false;
    }
    public void ReadyForHarvest()
    {
        this.readyForHarvest = true;
    }
    protected override void Update()
    {
        base.Update();
#if UNITY_EDITOR
        if (readyForHarvest)
        {
            this.EnableButton();
            // Remove any existing persistent calls to avoid duplication
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, this.Harvest);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, this.OpenInventory);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, this.borderColorChanger.SetColor);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, this.UnreadyForHarvest);

            // Add persistent listeners for the button's OnClick event
            UnityEventTools.AddPersistentListener(this.onClick, this.Harvest);
            UnityEventTools.AddPersistentListener(this.onClick, this.UnreadyForHarvest);
        }
        else
        {
            // Remove any existing persistent calls to avoid duplication
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, this.OpenInventory);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, this.borderColorChanger.SetColor);

            // Add persistent listeners for the button's OnClick event
            UnityEventTools.AddPersistentListener(this.onClick, this.OpenInventory);
            UnityEventTools.AddPersistentListener(this.onClick, this.borderColorChanger.SetColor);
        }
#endif
    }
    protected override void PerformClick()
    {
        base.PerformClick();

        if (!readyForHarvest)
            OnPlotSelected?.Invoke(this);
    }
    public void Harvest()
    {
        if (readyForHarvest)
            this.GetComponentInChildren<Harvester>().HarvestCrop();
    }
    public void OpenInventory()
    {
        if (!readyForHarvest)
        {
            InventoryManager.Instance.OpenInventory();
        }
    }
}
