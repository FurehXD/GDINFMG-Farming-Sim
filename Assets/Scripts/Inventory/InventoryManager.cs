using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private VerticalLayoutGroup inventoryLayoutGroup;

    private PlotButton selectedPlotReference;
    public PlotButton SelectedPlotReference { get {  return selectedPlotReference; } }

    private CropProducer cropProducerReference;
    public CropProducer CropProducerReference { get { return cropProducerReference; } }

    public static event Action OnInventoryOpened;
    public static event Action OnInventoryClosed;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);

        this.inventoryLayoutGroup = this.GetComponentInChildren<VerticalLayoutGroup>();
    }
    private void OnEnable()
    {
        PlotButton.OnPlotSelected += this.LogSelectedPlot;
        CropProducer.OnProductionStarted += this.CloseInventory;
    }
    private void OnDisable()
    {
        PlotButton.OnPlotSelected -= this.LogSelectedPlot;
        CropProducer.OnProductionStarted -= this.CloseInventory;
    }
    public void OpenInventory()
    {
        this.GetComponent<Enbiggener>().Enbiggen();
        OnInventoryOpened?.Invoke();
    }
    public void CloseInventory()
    {
        this.GetComponent<Enbiggener>().ResetSize();
        OnInventoryClosed?.Invoke();
    }
    public void LogSelectedPlot(PlotButton selectedPlot)
    {
        Debug.Log(selectedPlot.gameObject.name + " logged!");

        this.selectedPlotReference = selectedPlot;
        this.cropProducerReference = this.selectedPlotReference.GetComponent<CropProducer>();

        if (this.cropProducerReference == null)
            this.cropProducerReference = this.selectedPlotReference.transform.parent.GetComponent<CropProducer>();
    }
}
